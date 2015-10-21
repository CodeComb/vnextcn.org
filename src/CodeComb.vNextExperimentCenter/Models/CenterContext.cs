using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CodeComb.vNextExperimentCenter.Models
{
    public class CenterContext : IdentityDbContext<User, IdentityRole<long>, long>
    {
        public DbSet<Problem> Problems { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Blob> Blobs { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Problem>(e =>
            {
                e.HasIndex(x => x.Title);
                e.HasIndex(x => x.CheckPassed);
                e.HasIndex(x => x.Difficulty);
            });

            builder.Entity<Status>(e =>
            {
                e.HasIndex(x => x.Time);
                e.HasIndex(x => x.Result);
            });

            builder.Entity<Blob>(e =>
            {
                e.HasIndex(x => x.Time);
                e.HasIndex(x => x.ContentLength);
                e.HasIndex(x => x.ContentType);
            });
            
            builder.Entity<Tag>(e =>
            {
               e.HasIndex(x => x.PRI);
               e.HasIndex(x => x.Title); 
            });
        }
    }
}
