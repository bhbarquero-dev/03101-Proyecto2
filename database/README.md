# Base de datos (SQL Server)

Scripts de la base de datos `GestionProyectos`.

| Archivo (pendiente) | Contenido |
|---|---|
| `01_schema.sql` | Tablas, PK, FK, restricciones e índices |
| `02_seed.sql` | Catálogos (Roles, Estados, Prioridades) y usuario administrador inicial |

Tablas mínimas según el enunciado (4.2):

- `Roles` (Id, Nombre, Descripcion)
- `Usuarios` (Id, Nombre, Email, ContrasenaHash, IdRol, FechaRegistro, Activo)
- `Estados` (Id, Nombre, EntidadAplicacion)
- `Prioridades` (Id, Nombre)
- `Proyectos` (Id, Nombre, Descripcion, FechaInicio, FechaFinEstimada, IdEstado, IdResponsable, FechaCreacion)
- `Tareas` (Id, IdProyecto, Titulo, Descripcion, IdAsignado, IdEstado, IdPrioridad, FechaCreacion, FechaVencimiento)
