using _03101_Proyecto2.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace _03101_Proyecto2.Api.Data;

/// <summary>
/// Contexto de EF Core para la base GestionProyectos.
/// El esquema lo definen los scripts de la carpeta database (fuente de verdad); este contexto
/// solo lo mapea y no se usa para crearlo ni para generar migraciones. Las configuraciones
/// de Data/Configurations replican nombres, tipos, restricciones e índices de esos scripts.
/// </summary>
public class GestionProyectosContext(DbContextOptions<GestionProyectosContext> options) : DbContext(options)
{
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Estado> Estados => Set<Estado>();
    public DbSet<Prioridad> Prioridades => Set<Prioridad>();
    public DbSet<Proyecto> Proyectos => Set<Proyecto>();
    public DbSet<Tarea> Tareas => Set<Tarea>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("dbo");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GestionProyectosContext).Assembly);
    }
}
