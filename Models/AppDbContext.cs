using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using reservasApp.Models;


namespace reservasApp.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UserModel> Usuarios { get; set; } = null!;
    public DbSet<RestaurantModel> Restaurantes { get; set; } = null!;
    public DbSet<MesaModel> Mesas { get; set; } = null!;
    public DbSet<ReservasModel> Reservaciones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
    }
}
