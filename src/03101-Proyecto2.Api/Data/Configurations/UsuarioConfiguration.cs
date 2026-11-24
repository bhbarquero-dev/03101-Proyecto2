using _03101_Proyecto2.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace _03101_Proyecto2.Api.Data.Configurations;

/// <summary>Mapeo de <see cref="Usuario"/> a dbo.Usuarios (database/Tablas/02_Usuarios.sql).</summary>
public sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");
        builder.HasKey(u => u.Id).HasName("PK_Usuarios");

        builder.Property(u => u.Id).UseIdentityColumn();
        builder.Property(u => u.Nombre).HasMaxLength(100).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(256).IsRequired();
        builder.Property(u => u.ContrasenaHash).HasMaxLength(256).IsUnicode(false).IsRequired();
        builder.Property(u => u.FechaRegistro)
            .HasColumnType("datetime2(0)")
            .HasDefaultValueSql("SYSUTCDATETIME()");
        // Activo tiene DEFAULT (1) en la base, pero la entidad ya inicia en true
        // y siempre envía el valor; no se declara aquí para no ocultar un false explícito.
        builder.Property(u => u.Activo).IsRequired();

        builder.HasIndex(u => u.Email).IsUnique().HasDatabaseName("UQ_Usuarios_Email");
        builder.HasIndex(u => u.IdRol).HasDatabaseName("IX_Usuarios_IdRol");

        builder.HasOne(u => u.Rol)
            .WithMany(r => r.Usuarios)
            .HasForeignKey(u => u.IdRol)
            .HasConstraintName("FK_Usuarios_Roles")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
