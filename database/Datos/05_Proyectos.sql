/*
    Proyectos de demostración. Solo inserta los que no existan (por nombre).
    El responsable se busca por correo para no depender de los Id generados.
    Estados de proyecto: 1 Activo, 2 Cerrado.
*/
INSERT INTO dbo.Proyectos (Nombre, Descripcion, FechaInicio, FechaFinEstimada, IdEstado, IdResponsable)
SELECT origen.Nombre, origen.Descripcion, origen.FechaInicio, origen.FechaFinEstimada, origen.IdEstado, u.Id
FROM (VALUES
    (N'Portal de clientes',
     N'Sitio web para que los clientes consulten el avance de sus proyectos.',
     CAST('2026-09-01' AS DATE), CAST('2026-12-18' AS DATE), 1, N'admin@gestionproyectos.test'),
    (N'Migración a la nube',
     N'Traslado de los servicios internos a un proveedor de nube.',
     CAST('2026-10-05' AS DATE), CAST('2027-03-31' AS DATE), 1, N'ana.mora@gestionproyectos.test'),
    (N'Inventario de equipos 2026',
     N'Levantamiento del inventario de equipos de cómputo de la empresa.',
     CAST('2026-06-01' AS DATE), CAST('2026-08-28' AS DATE), 2, N'admin@gestionproyectos.test')
) AS origen (Nombre, Descripcion, FechaInicio, FechaFinEstimada, IdEstado, EmailResponsable)
JOIN dbo.Usuarios u ON u.Email = origen.EmailResponsable
WHERE NOT EXISTS (SELECT 1 FROM dbo.Proyectos p WHERE p.Nombre = origen.Nombre);
GO
