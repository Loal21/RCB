using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CRUD_Imagenes.Models;

public partial class DbpruebaContext : DbContext
{
    public DbpruebaContext()
    {
    }

    public DbpruebaContext(DbContextOptions<DbpruebaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__USUARIO__5B65BF97D1F49851");

            entity.ToTable("USUARIO");

            entity.Property(e => e.Contraseña)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Documento)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Apellido)
               .HasMaxLength(100)
               .IsUnicode(false);
            entity.Property(e => e.Telefono)
               .HasMaxLength(100)
               .IsUnicode(false);
            entity.Property(e => e.Rol)
              .HasMaxLength(100)
              .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
