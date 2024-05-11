using CrudOperation.Models;
using Microsoft.EntityFrameworkCore;

namespace CrudOperation.Services
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions options) :base(options)
        {
            
        }

        public DbSet<Item> Items { get; set; }  


    }
}
