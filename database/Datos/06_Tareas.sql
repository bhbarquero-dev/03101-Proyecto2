/*
    Tareas de demostración, repartidas entre estados y prioridades para que
    el dashboard muestre indicadores. Solo inserta las que no existan
    (por proyecto y título).
    Estados de tarea: 3 Pendiente, 4 En progreso, 5 Completada, 6 Cancelada.
    Prioridades: 1 Alta, 2 Media, 3 Baja.
*/
INSERT INTO dbo.Tareas (IdProyecto, Titulo, Descripcion, IdAsignado, IdEstado, IdPrioridad, FechaVencimiento)
SELECT p.Id, origen.Titulo, origen.Descripcion, u.Id, origen.IdEstado, origen.IdPrioridad, origen.FechaVencimiento
FROM (VALUES
    (N'Portal de clientes', N'Diseñar maqueta de la página de inicio', N'Propuesta visual para validar con el cliente.',
     N'ana.mora@gestionproyectos.test',   5, 2, CAST('2026-09-20' AS DATE)),
    (N'Portal de clientes', N'Implementar inicio de sesión',            N'Autenticación de clientes contra la API.',
     N'luis.solis@gestionproyectos.test', 4, 1, CAST('2026-11-30' AS DATE)),
    (N'Portal de clientes', N'Consulta de avance por proyecto',        N'Vista con el porcentaje de tareas completadas.',
     N'ana.mora@gestionproyectos.test',   3, 1, CAST('2026-12-10' AS DATE)),
    (N'Portal de clientes', N'Redactar manual de usuario',             NULL,
     N'luis.solis@gestionproyectos.test', 3, 3, CAST('2026-12-18' AS DATE)),
    (N'Migración a la nube', N'Inventariar servicios a migrar',        N'Lista de servicios con sus dependencias.',
     N'ana.mora@gestionproyectos.test',   5, 1, CAST('2026-10-20' AS DATE)),
    (N'Migración a la nube', N'Estimar costos mensuales',              NULL,
     N'ana.mora@gestionproyectos.test',   4, 2, CAST('2026-12-05' AS DATE)),
    (N'Migración a la nube', N'Evaluar proveedor alterno',             N'Descartada: se mantiene el proveedor actual.',
     N'luis.solis@gestionproyectos.test', 6, 3, NULL),
    (N'Inventario de equipos 2026', N'Registrar equipos de oficina central', NULL,
     N'luis.solis@gestionproyectos.test', 5, 2, CAST('2026-07-31' AS DATE))
) AS origen (NombreProyecto, Titulo, Descripcion, EmailAsignado, IdEstado, IdPrioridad, FechaVencimiento)
JOIN dbo.Proyectos p ON p.Nombre = origen.NombreProyecto
JOIN dbo.Usuarios  u ON u.Email  = origen.EmailAsignado
WHERE NOT EXISTS (SELECT 1 FROM dbo.Tareas t WHERE t.IdProyecto = p.Id AND t.Titulo = origen.Titulo);
GO
