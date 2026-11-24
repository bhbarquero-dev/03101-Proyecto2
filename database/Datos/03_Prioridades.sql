/* Catálogo de prioridades con identificadores fijos. Idempotente. */
MERGE dbo.Prioridades AS destino
USING (VALUES
    (1, N'Alta'),
    (2, N'Media'),
    (3, N'Baja')
) AS origen (Id, Nombre)
    ON destino.Id = origen.Id
WHEN MATCHED THEN
    UPDATE SET Nombre = origen.Nombre
WHEN NOT MATCHED THEN
    INSERT (Id, Nombre) VALUES (origen.Id, origen.Nombre);
GO
