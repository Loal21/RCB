using ApiProductos.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiProductos.Context
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Producto> Producto { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
            { 
            }
    }
}
