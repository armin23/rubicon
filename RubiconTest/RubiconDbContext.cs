using Microsoft.EntityFrameworkCore;
using RubiconTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RubiconTest
{
    public class RubiconDbContext : DbContext
    {
        public RubiconDbContext(DbContextOptions<RubiconDbContext> options)
            : base(options)
        {
        }

        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlogPostTag>()
                .HasKey(bc => new { bc.BlogPostId, bc.TagId });

            modelBuilder.Entity<BlogPostTag>()
                .HasOne(bc => bc.BlogPost)
                .WithMany(b => b.BlogPostTags)
                .HasForeignKey(bc => bc.BlogPostId);

            modelBuilder.Entity<BlogPostTag>()
                .HasOne(bc => bc.Tag)
                .WithMany(c => c.BlogPostTags)
                .HasForeignKey(bc => bc.TagId);
        }

    }

}
