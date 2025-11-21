using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace reservasApp.Namespace
{
    public class AppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UserModel> Usuarios { get; set; } = null!;
    public DbSet<RestaurantModel> Restaurantes { get; set; } = null!;
    public DbSet<MesaModel> Mesas { get; set; } = null!;
    public DbSet<Reservacion> Reservaciones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aquí puedes configurar relaciones si EF no las detecta automáticamente
        // Por ejemplo, eliminar en cascada o restricciones únicas
    }
    }

    public class DbContextOptions<T>
    {
    }

    public class DbContext
    {
    }
}
