using System;
using System.Collections.Generic;
using INCLUTEC.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace INCLUTEC.Api.Infrastructure.Data;


public partial class InclutecbdContext : DbContext
{
    public InclutecbdContext()
    {
    }

    public InclutecbdContext(DbContextOptions<InclutecbdContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aula> Aulas { get; set; }

    public virtual DbSet<Estudiante> Estudiantes { get; set; }

    public virtual DbSet<RegistroAsistencium> RegistroAsistencia { get; set; }

    public virtual DbSet<ResponsableAula> ResponsableAulas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(LocalDB)\\MSSQLLocalDB; Database=INCLUTECBD; Trusted_Connection=True; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Aula>(entity =>
        {
            entity.ToTable("Aula");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Nombre).HasMaxLength(50);

            entity.HasOne(d => d.Responsable).WithMany(p => p.Aulas)
                .HasForeignKey(d => d.ResponsableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Aula_ResponsableAula");
        });

        modelBuilder.Entity<Estudiante>(entity =>
        {
            entity.ToTable("Estudiante");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Apellido).HasMaxLength(50);
            entity.Property(e => e.AvatarUrlPictogramaPath).HasMaxLength(500);
            entity.Property(e => e.Nombre).HasMaxLength(50);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Estudiante)
                .HasForeignKey<Estudiante>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Estudiante_Aula");
        });

        modelBuilder.Entity<RegistroAsistencium>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Fecha).HasColumnType("datetime");

            entity.HasOne(d => d.Estudiante).WithMany(p => p.RegistroAsistencia)
                .HasForeignKey(d => d.EstudianteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RegistroAsistencia_Estudiante");
        });

        modelBuilder.Entity<ResponsableAula>(entity =>
        {
            entity.ToTable("ResponsableAula");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Apellido).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Nombre).HasMaxLength(50);
            entity.Property(e => e.Rol).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
