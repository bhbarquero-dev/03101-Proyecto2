using _03101_Proyecto2.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace _03101_Proyecto2.Api.Data.Configurations;

/// <summary>Mapeo de <see cref="Rol"/> a dbo.Roles (database/Tablas/01_Roles.sql).</summary>
public sealed class RolConfiguration : IEntityTypeConfiguration<Rol>
{
    public void Configure(EntityTypeBuilder<Rol> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(r => r.Id).HasName("PK_Roles");

        // Catálogo con identificadores fijos: la columna no es IDENTITY.
        builder.Property(r => r.Id).ValueGeneratedNever();
        builder.Property(r => r.Nombre).HasMaxLength(50).IsRequired();
        builder.Property(r => r.Descripcion).HasMaxLength(200);

        builder.HasIndex(r => r.Nombre).IsUnique().HasDatabaseName("UQ_Roles_Nombre");
    }
}
