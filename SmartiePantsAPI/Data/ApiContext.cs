using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using SmartiePantsAPI.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartiePantsAPI.Data
{
    public class ApiContext : DbContext
    {
        public DbSet<Instance> Instances { get; set; }
        
        public DbSet<Waterfall> Waterfalls { get; set; }

        public ApiContext(DbContextOptions<ApiContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Waterfall>()
                .HasMany(x => x.Instances)
                .WithOne()
                .HasForeignKey(x => x.WaterfallId);
        }

    }
}

