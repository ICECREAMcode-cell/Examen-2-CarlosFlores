using Examen2_CarlosFlores.Models;
using Microsoft.EntityFrameworkCore;

namespace Examen2_CarlosFlores.Data
{
    public class BaseDatos : DbContext
    {
        public BaseDatos(DbContextOptions<BaseDatos> options) : base(options) { }
        public DbSet<Tesis> tesis { get; set; }
        public DbSet<Tutor> tutores { get; set; }
        public DbSet<Usuario> usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.NombreUsuario)
                .IsUnique();

            modelBuilder.Entity<Tesis>()
                .HasOne(t => t.Tutor)
                .WithMany()
                .HasForeignKey(t => t.Id_Tutor)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
