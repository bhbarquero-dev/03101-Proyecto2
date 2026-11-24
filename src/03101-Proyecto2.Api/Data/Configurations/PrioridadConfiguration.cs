using _03101_Proyecto2.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace _03101_Proyecto2.Api.Data.Configurations;

/// <summary>Mapeo de <see cref="Prioridad"/> a dbo.Prioridades (database/Tablas/04_Prioridades.sql).</summary>
public sealed class PrioridadConfiguration : IEntityTypeConfiguration<Prioridad>
{
    public void Configure(EntityTypeBuilder<Prioridad> builder)
    {
        builder.ToTable("Prioridades");
        builder.HasKey(p => p.Id).HasName("PK_Prioridades");

        builder.Property(p => p.Id).ValueGeneratedNever();
        builder.Property(p => p.Nombre).HasMaxLength(20).IsRequired();

        builder.HasIndex(p => p.Nombre).IsUnique().HasDatabaseName("UQ_Prioridades_Nombre");
    }
}
