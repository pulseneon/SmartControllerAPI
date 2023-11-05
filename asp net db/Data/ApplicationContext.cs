using asp_net_db.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;

namespace asp_net_db.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            // Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Lesson>().HasQueryFilter(x => !x.isDeleted);
            modelBuilder.Entity<Server>().HasQueryFilter(x => !x.IsDeleted);
            //modelBuilder.Entity<Server>().HasData(
            //new Server[]
            //{
            //    new Server { Id=1, DisplayedName="Сервер вани", Host="26.193.128.247", Username="postgres", Port="5432", DbName="tg_bot_base", Password="admin" }
            //});
        }
        
        public DbSet<Server> Servers { get; set; }
        public DbSet<ServerStats> ServerStats { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<Backup> Backups { get; set; }
        //public DbSet<Test> Test { get; set; }
        //public DbSet<Course> Courses { get; set; }
        //public DbSet<Lesson> Lessons { get; set; }
        //public DbSet<Tracker> Trackers { get; set; }
        //public DbSet<Homework> Homeworks { get; set; }
        //public DbSet<SolvedHomework> SolvedHomeworks { get; set; }
        //public DbSet<Content> Contents { get; set; }
    }
}
