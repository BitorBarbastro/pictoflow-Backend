using Microsoft.EntityFrameworkCore;

namespace pictoflow_Backend.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Watermark> Watermarks { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuraciones adicionales de las entidades

            // Configuración de la entidad User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Email).IsRequired();
                entity.Property(u => u.PasswordSalt).IsRequired();
                // Otras configuraciones de la entidad User
            });

            // Configuración de la entidad Photo
            modelBuilder.Entity<Photo>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.HasOne(p => p.User)
                    .WithMany(u => u.Photos)
                    .HasForeignKey(p => p.UserId);
                // Otras configuraciones de la entidad Photo
            });

            // Configuración de la entidad Watermark
            modelBuilder.Entity<Watermark>(entity =>
            {
                entity.HasKey(w => w.Id);
                entity.HasOne(w => w.Photographer)
                    .WithMany(u => u.Watermarks)
                    .HasForeignKey(w => w.PhotographerId);
                // Otras configuraciones de la entidad Watermark
            });

            // Configuración de la entidad Gallery
            modelBuilder.Entity<Gallery>(entity =>
            {
                entity.HasKey(g => g.Id);
                entity.HasOne(g => g.Photographer)
                    .WithMany(u => u.Galleries)
                    .HasForeignKey(g => g.PhotographerId);
            });

            // Configuración de la entidad Transaction
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.HasOne(t => t.User)
                    .WithMany(u => u.Transactions)
                    .HasForeignKey(t => t.UserId);
                // Otras configuraciones de la entidad Transaction
            });

            // Configuración de la entidad Comment
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.HasOne(c => c.User)
                    .WithMany(u => u.Comments)
                    .HasForeignKey(c => c.UserId);
                entity.HasOne(c => c.Photo)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(c => c.PhotoId);
                // Otras configuraciones de la entidad Comment
            });
        }
    }
}
