/*
    Estados de proyectos y tareas (catálogo compartido).
    EntidadAplicacion indica a qué entidad aplica cada estado:
        Proyecto: Activo, Cerrado
        Tarea:    Pendiente, En progreso, Completada, Cancelada
    La regla "una tarea solo usa estados de tarea" (y lo mismo para proyectos)
    se valida en la API antes de guardar.
*/
IF OBJECT_ID(N'dbo.Estados', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Estados
    (
        Id                INT           NOT NULL,
        Nombre            NVARCHAR(50)  NOT NULL,
        EntidadAplicacion NVARCHAR(20)  NOT NULL,

        CONSTRAINT PK_Estados                  PRIMARY KEY (Id),
        CONSTRAINT UQ_Estados_Nombre_Entidad   UNIQUE (Nombre, EntidadAplicacion),
        CONSTRAINT CK_Estados_EntidadAplicacion CHECK (EntidadAplicacion IN (N'Proyecto', N'Tarea'))
    );
END
GO
