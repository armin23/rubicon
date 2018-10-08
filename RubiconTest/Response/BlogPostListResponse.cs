using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RubiconTest.Response
{
    public class BlogPostListResponse
    {
        public List<BlogPostResponse> BlogPosts { get; set; }
        public int PostsCount { get; set; }
    }
}
