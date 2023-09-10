using Microsoft.EntityFrameworkCore;
using Project_Management.Models.DatabaseModel;

namespace Project_Management.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
            try
            {
                Database.EnsureCreated();
            }
            catch (Exception ex) {
                Console.WriteLine("An error occurred while testing the database connection: " + ex.Message);
            }
        }

        public bool TestConnection()
        {
            try
            {
                return Database.CanConnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while testing the database connection: " + ex.Message);
                return false;
            }
        }
        public DbSet<Project> Project { get; set; }
        public DbSet<UserProject> UserProject { get; set; }

        public DbSet<Comment> Comment { get; set; }

        public DbSet<Models.DatabaseModel.Task> Task { get; set; }

        public DbSet<User> User { get; set; }
    }
}
