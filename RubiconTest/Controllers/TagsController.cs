using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RubiconTest.Response;

namespace RubiconTest.Controllers
{
    [Route("api/tags")]
    [ApiController]
    public class TagsController : ControllerBase
    {

        private readonly RubiconDbContext _context;

        public TagsController(RubiconDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ListTagsResponse GetTags()
        {

            var tags = _context.Tags
                          .Select(bp => bp.Name
                          ).ToList();

            return new ListTagsResponse
            {
                Tags = tags
            };
        }
    }
}