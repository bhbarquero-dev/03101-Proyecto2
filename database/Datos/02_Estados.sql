/* Catálogo de estados con identificadores fijos. Idempotente. */
MERGE dbo.Estados AS destino
USING (VALUES
    (1, N'Activo',      N'Proyecto'),
    (2, N'Cerrado',     N'Proyecto'),
    (3, N'Pendiente',   N'Tarea'),
    (4, N'En progreso', N'Tarea'),
    (5, N'Completada',  N'Tarea'),
    (6, N'Cancelada',   N'Tarea')
) AS origen (Id, Nombre, EntidadAplicacion)
    ON destino.Id = origen.Id
WHEN MATCHED THEN
    UPDATE SET Nombre = origen.Nombre, EntidadAplicacion = origen.EntidadAplicacion
WHEN NOT MATCHED THEN
    INSERT (Id, Nombre, EntidadAplicacion) VALUES (origen.Id, origen.Nombre, origen.EntidadAplicacion);
GO
