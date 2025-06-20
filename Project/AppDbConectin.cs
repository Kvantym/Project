using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project
{


    public class AppDbConectin : IdentityDbContext<IdentityUser>
    {
        public AppDbConectin(DbContextOptions<AppDbConectin> options) : base(options)
        {
            
        }
       public DbSet<Movie> movies { get; set; }

    }
}
