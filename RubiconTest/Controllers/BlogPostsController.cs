using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RubiconTest;
using RubiconTest.Models;
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
        public BlogPostListResponse GetBlogPosts()
        {
            var blogs = _context.BlogPosts
                           .Include(bp => bp.BlogPostTags)
                                .ThenInclude(t => t.Tag)
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
        public async Task<IActionResult> PutBlogPost([FromRoute] int id, [FromBody] BlogPost blogPost)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != blogPost.ID)
            {
                return BadRequest();
            }

            _context.Entry(blogPost).State = EntityState.Modified;

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

            return NoContent();
        }

        // POST: api/BlogPosts
        [HttpPost]
        public async Task<IActionResult> PostBlogPost([FromBody] BlogPost blogPost)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBlogPost", new { id = blogPost.ID }, blogPost);
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