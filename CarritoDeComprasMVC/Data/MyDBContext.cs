using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CarritoDeComprasMVC.Models.Entity;

namespace CarritoDeComprasMVC.Data
{
    public class MyDBContext : IdentityDbContext<IdentityUser>
    {
        public MyDBContext(DbContextOptions<MyDBContext> options) : base(options)
        {

        }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<CarritoItem> CarritoItems { get; set; }
        public DbSet<Orden> Ordenes { get; set; }
        public DbSet<OrdenItem> OrdenItems { get; set; }
        public DbSet<Carrito> Carritos { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Descripcion).HasMaxLength(500);
                entity.Property(e => e.Precio).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.Stock).IsRequired();
                entity.Property(e => e.ImagenUrl).HasMaxLength(255);
            });

            modelBuilder.Entity<Carrito>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Usuario)
                    .WithMany() 
                    .HasForeignKey(e => e.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.CarritoItems)
                    .WithOne(ci => ci.Carrito)
                    .HasForeignKey(ci => ci.CarritoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CarritoItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Cantidad).IsRequired();
                entity.Property(e => e.PrecioTotal).HasColumnType("decimal(18,2)").IsRequired();

                entity.HasOne(e => e.Producto)
                    .WithMany()
                    .HasForeignKey(e => e.ProductoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Orden>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FechaCreacion).IsRequired();
                entity.Property(e => e.Total).HasColumnType("decimal(18,2)").IsRequired();

                entity.HasOne(e => e.Usuario)
                    .WithMany()
                    .HasForeignKey(e => e.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.Items)
                    .WithOne(e => e.Orden) 
                    .HasForeignKey(e => e.OrdenId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OrdenItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Cantidad).IsRequired();
                entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.PrecioTotal).HasColumnType("decimal(18,2)").IsRequired();

                entity.HasOne(e => e.Producto)
                    .WithMany()
                    .HasForeignKey(e => e.ProductoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Orden) 
                    .WithMany(o => o.Items) 
                    .HasForeignKey(e => e.OrdenId) 
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
