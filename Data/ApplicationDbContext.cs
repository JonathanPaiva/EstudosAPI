using EstudosAPI.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace EstudosAPI.Data
{
    public class ApplicationDbContext: DbContext
    {
        DbSet<Product> Products { get; set; }
     
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { 
        }

        //Com o modelCreating podemos definir as propriedades dos campos das tabelas
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Product>().Property(prod => prod.Name).IsRequired().HasMaxLength(150);

            builder.Entity<Product>().Property(prod => prod.Code).IsRequired();
        }        
    }
}
