using FileManager.Model;
using Microsoft.EntityFrameworkCore;

namespace FileManager.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

        public DbSet<FileInformation> Files { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileInformation>().HasIndex(u => u.UUID).IsUnique().HasDatabaseName("UUID_INDEX");
            modelBuilder.Entity<FileInformation>().HasIndex(u => u.FileName).HasDatabaseName("FILENAME_INDEX");
        }
    }
}
