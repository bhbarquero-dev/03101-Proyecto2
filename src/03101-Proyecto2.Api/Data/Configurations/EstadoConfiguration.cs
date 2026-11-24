using _03101_Proyecto2.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace _03101_Proyecto2.Api.Data.Configurations;

/// <summary>Mapeo de <see cref="Estado"/> a dbo.Estados (database/Tablas/03_Estados.sql).</summary>
public sealed class EstadoConfiguration : IEntityTypeConfiguration<Estado>
{
    public void Configure(EntityTypeBuilder<Estado> builder)
    {
        builder.ToTable("Estados", t => t.HasCheckConstraint(
            "CK_Estados_EntidadAplicacion",
            "EntidadAplicacion IN (N'Proyecto', N'Tarea')"));
        builder.HasKey(e => e.Id).HasName("PK_Estados");

        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.Nombre).HasMaxLength(50).IsRequired();
        builder.Property(e => e.EntidadAplicacion).HasMaxLength(20).IsRequired();

        builder.HasIndex(e => new { e.Nombre, e.EntidadAplicacion })
            .IsUnique()
            .HasDatabaseName("UQ_Estados_Nombre_Entidad");
    }
}
