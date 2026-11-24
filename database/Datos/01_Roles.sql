/* Catálogo de roles con identificadores fijos. Idempotente. */
MERGE dbo.Roles AS destino
USING (VALUES
    (1, N'Administrador', N'Líder o gestor de proyectos. Administra usuarios, proyectos y tareas.'),
    (2, N'Usuario',       N'Miembro de equipo. Gestiona sus tareas y consulta los proyectos en los que participa.')
) AS origen (Id, Nombre, Descripcion)
    ON destino.Id = origen.Id
WHEN MATCHED THEN
    UPDATE SET Nombre = origen.Nombre, Descripcion = origen.Descripcion
WHEN NOT MATCHED THEN
    INSERT (Id, Nombre, Descripcion) VALUES (origen.Id, origen.Nombre, origen.Descripcion);
GO
