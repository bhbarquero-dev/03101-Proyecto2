/*
    Tareas: tabla central del modelo, con cuatro claves foráneas.
    Ninguna relación propaga borrados en cascada (ON DELETE NO ACTION).

    Índices:
    - IX_Tareas_IdProyecto_IdEstado: sirve a la FK de IdProyecto (columna inicial)
      y cubre el indicador "porcentaje de tareas completadas por proyecto".
    - IX_Tareas_IdAsignado_IdEstado: sirve a la FK de IdAsignado y cubre
      "carga de trabajo por usuario" y "tareas asignadas al usuario conectado".
    - IX_Tareas_IdEstado: indicador "total de tareas por estado".
    - IX_Tareas_IdPrioridad: FK hacia Prioridades.
*/
IF OBJECT_ID(N'dbo.Tareas', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Tareas
    (
        Id               INT IDENTITY(1, 1) NOT NULL,
        IdProyecto       INT                NOT NULL,
        Titulo           NVARCHAR(150)      NOT NULL,
        Descripcion      NVARCHAR(1000)     NULL,
        IdAsignado       INT                NOT NULL,
        IdEstado         INT                NOT NULL,
        IdPrioridad      INT                NOT NULL,
        FechaCreacion    DATETIME2(0)       NOT NULL CONSTRAINT DF_Tareas_FechaCreacion DEFAULT (SYSUTCDATETIME()),
        FechaVencimiento DATE               NULL,

        CONSTRAINT PK_Tareas             PRIMARY KEY (Id),
        CONSTRAINT FK_Tareas_Proyectos   FOREIGN KEY (IdProyecto)
            REFERENCES dbo.Proyectos (Id)   ON DELETE NO ACTION ON UPDATE NO ACTION,
        CONSTRAINT FK_Tareas_Usuarios    FOREIGN KEY (IdAsignado)
            REFERENCES dbo.Usuarios (Id)    ON DELETE NO ACTION ON UPDATE NO ACTION,
        CONSTRAINT FK_Tareas_Estados     FOREIGN KEY (IdEstado)
            REFERENCES dbo.Estados (Id)     ON DELETE NO ACTION ON UPDATE NO ACTION,
        CONSTRAINT FK_Tareas_Prioridades FOREIGN KEY (IdPrioridad)
            REFERENCES dbo.Prioridades (Id) ON DELETE NO ACTION ON UPDATE NO ACTION
    );

    CREATE INDEX IX_Tareas_IdProyecto_IdEstado ON dbo.Tareas (IdProyecto, IdEstado);
    CREATE INDEX IX_Tareas_IdAsignado_IdEstado ON dbo.Tareas (IdAsignado, IdEstado);
    CREATE INDEX IX_Tareas_IdEstado            ON dbo.Tareas (IdEstado);
    CREATE INDEX IX_Tareas_IdPrioridad         ON dbo.Tareas (IdPrioridad);
END
GO
