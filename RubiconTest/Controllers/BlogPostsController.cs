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
        
        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBlogPost([FromRoute] string slug)
        {
            var blogPost = await _context.BlogPosts

                           .Include(bp => bp.BlogPostTags)
                                .ThenInclude(t => t.Tag)
                            .Where(bp => bp.Slug == slug)
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
        
        [HttpPut("{slug}")]
        public async Task<IActionResult> PutBlogPost([FromRoute] string slug, [FromBody] BlogPostRequest blogPost)
        {
            var entity = _context.BlogPosts.Where(a => a.Slug == slug).FirstOrDefault();

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
                var oldTags = _context.BlogPostTags.Where(a => a.BlogPost.Slug == entity.Slug).ToList();

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
                        BlogPostId = entity.ID,
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
                return BadRequest("Desila se greška");
            }

            return await GetBlogPost(entity.Slug);
        }
        
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

            return await GetBlogPost(entity.Slug);
        }
        
        [HttpDelete("{slug}")]
        public async Task<IActionResult> DeleteBlogPost([FromRoute] string slug)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var blogPost = _context.BlogPosts.Where(a => a.Slug == slug).FirstOrDefault();
            if (blogPost == null)
            {
                return NotFound();
            }

            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();

            return Ok(blogPost);
        }

        private static string GetSlug(BlogPostRequest blogPost)
        {
            return blogPost.BlogPost.Title
                                    .Replace(' ', '-')
                                    .Replace("?", "")
                                    .Replace("&", "")
                                    .Replace("\'", "")
                                    .Replace("\"", "")
                                    .ToLower();
        }
    }
}