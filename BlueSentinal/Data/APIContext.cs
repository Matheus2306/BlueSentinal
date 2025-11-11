using BlueSentinal.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlueSentinal.Data
{
    public class APIContext : IdentityDbContext<Usuario>
    {
        public APIContext(DbContextOptions<APIContext> options) : base(options)
        { }

        public DbSet<DroneFabri> DroneFabris { get; set; }
        public DbSet<Drone> Drones { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Tabelas
            builder.Entity<DroneFabri>().ToTable("Drone Fabrica");
            builder.Entity<Drone>().ToTable("Drones Usuários");
        }

    }
}
