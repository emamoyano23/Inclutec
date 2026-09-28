using INCLUTEC.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace INCLUTEC.Api.Infrastructure.Data
{
    public partial class InclutecbdContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Aula>(entity =>
            {
                entity.Ignore(a => a.Estudiante);
            });

            modelBuilder.Entity<Estudiante>(entity =>
            {
                entity.Ignore(e => e.IdNavigation);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Apellido).HasMaxLength(100).IsRequired();
                entity.Property(e => e.AvatarUrlPictogramaPath).HasMaxLength(1000).IsRequired(false);
                entity.Property(e => e.EstadoActivo).HasDefaultValue(true);

                entity.HasOne<Aula>()
                      .WithMany()
                      .HasForeignKey(e => e.AulaId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_Estudiante_Aula");

                entity.HasIndex(e => e.AulaId, "IX_Estudiante_AulaId");
            });
        }
    }
}
