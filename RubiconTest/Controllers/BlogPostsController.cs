using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RubiconTest;
using RubiconTest.Models;
using RubiconTest.Request;
using RubiconTest.Response;

namespace RubiconTest.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly RubiconDbContext _context;

        public BlogPostsController(RubiconDbContext context)
        {
            _context = context;
        }

        // GET: api/BlogPosts
        [HttpGet]
        public BlogPostListResponse GetBlogPosts([FromQuery] string tag)
        {
            var blogsQuery = _context.BlogPosts
                           .Include(bp => bp.BlogPostTags)
                                .ThenInclude(t => t.Tag)
                                .AsQueryable();

            if (!String.IsNullOrEmpty(tag))
            {
                blogsQuery = blogsQuery.Where(a => a.BlogPostTags
                                                    .Any(t => t.Tag.Name
                                                                    .ToLower()
                                                                    .Contains(tag)));
            }

            var blogs = blogsQuery
                           .Select(bp => new BlogPostResponse
                           {
                               Body = bp.Body,
                               Description = bp.Description,
                               Slug = bp.Slug,
                               Title = bp.Title,
                               CreatedAt = bp.CreatedAt,
                               UpdatedAt = bp.UpdatedAt,
                               BlogPostTags = bp.BlogPostTags
                                                     .Select(bpt => bpt.Tag.Name)
                                                     .ToList()

                           }).ToList();

            var blogsCount = blogs.Count;
            return new BlogPostListResponse
            {
                PostsCount = blogsCount,
                BlogPosts = blogs
            };
        }

        // GET: api/BlogPosts/5
        [HttpGet("{id}")]

        public async Task<IActionResult> GetBlogPost([FromRoute] int id)
        {


            var blogPost = await _context.BlogPosts

                           .Include(bp => bp.BlogPostTags)
                                .ThenInclude(t => t.Tag)
                            .Where(bp => bp.ID == id)
                           .Select(bp => new BlogPostResponse
                           {
                               Body = bp.Body,
                               Description = bp.Description,
                               Slug = bp.Slug,
                               Title = bp.Title,
                               CreatedAt = bp.CreatedAt,
                               UpdatedAt = bp.UpdatedAt,
                               BlogPostTags = bp.BlogPostTags
                                                     .Select(bpt => bpt.Tag.Name)
                                                     .ToList()
                           })
                           .SingleOrDefaultAsync();



            if (blogPost == null)
            {
                return NotFound();
            }

            return Ok(blogPost);
        }


        // PUT: api/BlogPosts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBlogPost([FromRoute] int id, [FromBody] BlogPostRequest blogPost)
        {
            var entity = _context.BlogPosts.Where(a => a.ID == id).FirstOrDefault();

            if (entity == null)
            {
                return NotFound();
            }

            if (!String.IsNullOrEmpty(blogPost.BlogPost.Title))
            {
                entity.Title = blogPost.BlogPost.Title;
                entity.Slug = GetSlug(blogPost);
            }

            if (!String.IsNullOrEmpty(blogPost.BlogPost.Description))
            {
                entity.Description = blogPost.BlogPost.Description;
            }

            if (!String.IsNullOrEmpty(blogPost.BlogPost.Body))
            {
                entity.Body = blogPost.BlogPost.Body;
            }

            if (blogPost.BlogPost.TagList != null)
            {
                var oldTags = _context.BlogPostTags.Where(a => a.BlogPostId == id).ToList();

                foreach (var item in oldTags.ToList())
                {
                    _context.BlogPostTags.Remove(item);
                }

                foreach (var item in blogPost.BlogPost.TagList)
                {

                    var tag = _context.Tags.FirstOrDefault(t => t.Name == item);
                    if (tag == null)
                    {
                        tag = new Tag
                        {
                            Name = item
                        };
                        _context.Tags.Add(tag);
                    }

                    _context.BlogPostTags.Add(new BlogPostTag
                    {
                        BlogPostId = id,
                        TagId = tag.ID
                    });
                }
            }

            entity.UpdatedAt = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogPostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return await GetBlogPost(id);
        }

        // POST: api/BlogPosts
        [HttpPost]
        public async Task<IActionResult> PostBlogPost([FromBody] BlogPostRequest blogPost)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = new BlogPost
            {
                Title = blogPost.BlogPost.Title,
                Description = blogPost.BlogPost.Description,
                Body = blogPost.BlogPost.Body,
                Slug = GetSlug(blogPost),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                BlogPostTags = new List<BlogPostTag>()

            };


            foreach (var item in blogPost.BlogPost.TagList)
            {
                var tag = _context.Tags.FirstOrDefault(t => t.Name == item);
                if (tag == null)
                {
                    tag = new Tag
                    {
                        Name = item
                    };
                    _context.Tags.Add(tag);
                }
                entity.BlogPostTags.Add(new BlogPostTag
                {
                    Tag = tag
                });
            }

            _context.BlogPosts.Add(entity);
            await _context.SaveChangesAsync();

            return await GetBlogPost(entity.ID);
        }

        private static string GetSlug(BlogPostRequest blogPost)
        {
            return blogPost.BlogPost.Title.Replace(' ', '-').ToLower().Replace("\'", "").Replace("\"", "");
        }

        // DELETE: api/BlogPosts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogPost([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }

            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();

            return Ok(blogPost);
        }

        private bool BlogPostExists(int id)
        {
            return _context.BlogPosts.Any(e => e.ID == id);
        }
    }
}