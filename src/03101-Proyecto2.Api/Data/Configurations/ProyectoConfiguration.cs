using _03101_Proyecto2.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace _03101_Proyecto2.Api.Data.Configurations;

/// <summary>Mapeo de <see cref="Proyecto"/> a dbo.Proyectos (database/Tablas/05_Proyectos.sql).</summary>
public sealed class ProyectoConfiguration : IEntityTypeConfiguration<Proyecto>
{
    public void Configure(EntityTypeBuilder<Proyecto> builder)
    {
        builder.ToTable("Proyectos", t => t.HasCheckConstraint(
            "CK_Proyectos_Fechas",
            "FechaFinEstimada >= FechaInicio"));
        builder.HasKey(p => p.Id).HasName("PK_Proyectos");

        builder.Property(p => p.Id).UseIdentityColumn();
        builder.Property(p => p.Nombre).HasMaxLength(150).IsRequired();
        builder.Property(p => p.Descripcion).HasMaxLength(1000);
        builder.Property(p => p.FechaInicio).HasColumnType("date");
        builder.Property(p => p.FechaFinEstimada).HasColumnType("date");
        builder.Property(p => p.FechaCreacion)
            .HasColumnType("datetime2(0)")
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasIndex(p => p.IdEstado).HasDatabaseName("IX_Proyectos_IdEstado");
        builder.HasIndex(p => p.IdResponsable).HasDatabaseName("IX_Proyectos_IdResponsable");

        builder.HasOne(p => p.Estado)
            .WithMany(e => e.Proyectos)
            .HasForeignKey(p => p.IdEstado)
            .HasConstraintName("FK_Proyectos_Estados")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Responsable)
            .WithMany(u => u.ProyectosResponsable)
            .HasForeignKey(p => p.IdResponsable)
            .HasConstraintName("FK_Proyectos_Usuarios")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
