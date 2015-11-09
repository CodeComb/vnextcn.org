using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using CodeComb.vNextChina.Hub.Models;


namespace CodeComb.vNextChina.Models
{
    public class vNextChinaContext : IdentityDbContext<User, IdentityRole<long>, long>, INodeDbContext
    {
        public DbSet<Experiment> Experiments { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Blob> Blobs { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Node> Nodes { get; set; }
        public DbSet<StatusDetail> StatusDetails { get; set; }
        public DbSet<Contest> Contests { get; set; }
        public DbSet<ContestExperiment> ContestExperiments { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<CISet> CISets { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<Thread> Threads { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Thread>(e => 
            {
                e.HasIndex(x => x.IsTop);
                e.HasIndex(x => x.CreationTime);
                e.HasIndex(x => x.LastReplyTime);
                e.HasIndex(x => x.IsAnnouncement);
            });

            builder.Entity<Post>(e =>
            {
                e.HasIndex(x => x.Time);
            });

            builder.Entity<Forum>(e =>
            {
                e.Property(x => x.ParentId).IsRequired(false);
                e.HasIndex(x => x.PRI);
            });

            builder.Entity<Contest>(e => 
            {
                e.HasIndex(x => x.Begin);
                e.HasIndex(x => x.End);
            });

            builder.Entity<ContestExperiment>(e =>
            {
                e.HasIndex(x => x.Point);
                e.HasKey(x => new { x.ContestId, x.ExperimentId });
            });

            builder.Entity<Project>(e => 
            {
                e.HasIndex(x => x.PRI);
            });

            builder.Entity<User>(e => 
            {
                e.HasIndex(x => x.RegisteryTime);
            });

            builder.Entity<Experiment>(e =>
            {
                e.HasIndex(x => x.Title);
                e.HasIndex(x => x.CheckPassed);
                e.HasIndex(x => x.Difficulty);
            });

            builder.Entity<Status>(e =>
            {
                e.HasIndex(x => x.Time);
                e.HasIndex(x => x.Result);
                e.HasIndex(x => x.Type);
                e.HasIndex(x => x.RunWithLinux);
                e.HasIndex(x => x.RunWithOsx);
                e.HasIndex(x => x.RunWithLinux);
                e.HasIndex(x => x.LinuxResult);
                e.HasIndex(x => x.OsxResult);
                e.HasIndex(x => x.WindowsResult);
            });

            builder.Entity<StatusDetail>(e =>
            {
                e.HasIndex(x => x.OS);
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
