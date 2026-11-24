using _03101_Proyecto2.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace _03101_Proyecto2.Api.Data.Configurations;

/// <summary>Mapeo de <see cref="Tarea"/> a dbo.Tareas (database/Tablas/06_Tareas.sql).</summary>
public sealed class TareaConfiguration : IEntityTypeConfiguration<Tarea>
{
    public void Configure(EntityTypeBuilder<Tarea> builder)
    {
        builder.ToTable("Tareas");
        builder.HasKey(t => t.Id).HasName("PK_Tareas");

        builder.Property(t => t.Id).UseIdentityColumn();
        builder.Property(t => t.Titulo).HasMaxLength(150).IsRequired();
        builder.Property(t => t.Descripcion).HasMaxLength(1000);
        builder.Property(t => t.FechaCreacion)
            .HasColumnType("datetime2(0)")
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(t => t.FechaVencimiento).HasColumnType("date");

        // Los índices compuestos sirven a la vez a la FK (columna inicial) y a los indicadores.
        builder.HasIndex(t => new { t.IdProyecto, t.IdEstado }).HasDatabaseName("IX_Tareas_IdProyecto_IdEstado");
        builder.HasIndex(t => new { t.IdAsignado, t.IdEstado }).HasDatabaseName("IX_Tareas_IdAsignado_IdEstado");
        builder.HasIndex(t => t.IdEstado).HasDatabaseName("IX_Tareas_IdEstado");
        builder.HasIndex(t => t.IdPrioridad).HasDatabaseName("IX_Tareas_IdPrioridad");

        builder.HasOne(t => t.Proyecto)
            .WithMany(p => p.Tareas)
            .HasForeignKey(t => t.IdProyecto)
            .HasConstraintName("FK_Tareas_Proyectos")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Asignado)
            .WithMany(u => u.TareasAsignadas)
            .HasForeignKey(t => t.IdAsignado)
            .HasConstraintName("FK_Tareas_Usuarios")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Estado)
            .WithMany(e => e.Tareas)
            .HasForeignKey(t => t.IdEstado)
            .HasConstraintName("FK_Tareas_Estados")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Prioridad)
            .WithMany(p => p.Tareas)
            .HasForeignKey(t => t.IdPrioridad)
            .HasConstraintName("FK_Tareas_Prioridades")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
