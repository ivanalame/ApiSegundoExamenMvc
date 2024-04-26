using ApiSegundoExamenMvc.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiSegundoExamenMvc.Data
{
    public class CubosContext : DbContext
    {
        public CubosContext(DbContextOptions<CubosContext>options):base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<CompraCubo>ComprasCubo { get; set; }
        public DbSet<Cubo> Cubos { get; set; }
    }
}
