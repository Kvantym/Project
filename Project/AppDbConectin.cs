namespace Project
{
    using Microsoft.EntityFrameworkCore;
    using Project.Models;

    public class AppDbConectin : DbContext
    {
        public AppDbConectin(DbContextOptions<AppDbConectin> options) : base(options)
        {
            
        }
       public DbSet<Movie> movies { get; set; }

    }
}
