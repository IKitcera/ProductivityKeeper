using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProductivityKeeperWeb.Domain.Models;
using ProductivityKeeperWeb.Domain.Models.TaskRelated;
using System;
using System.IO;

namespace ProductivityKeeperWeb.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Subcategory> Subcategories { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<SubcategoryTask> SubcategoriesTasks { get; set; }
        public DbSet<UserStatistic> Statistics { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.SetCommandTimeout((int)TimeSpan.FromSeconds(30).TotalSeconds);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Tag>();

            modelBuilder.Entity<Category>();

            modelBuilder.Entity<Subcategory>()
              .HasMany(s => s.Tasks)
              .WithMany(t => t.Subcategories)
              .UsingEntity<SubcategoryTask>(j => j.ToTable("SubcategoryTask"));

            base.OnModelCreating(modelBuilder);
        }
    }
}
