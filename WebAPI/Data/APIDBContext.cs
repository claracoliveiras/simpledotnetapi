using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Data
{
    public class APIDBContext : IdentityDbContext

    {
        public DbSet<Produto> Produtos { get; set; }

        public APIDBContext(DbContextOptions<APIDBContext> options) : base(options)
        {

        }
    }
}
