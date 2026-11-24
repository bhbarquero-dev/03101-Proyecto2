/*
    Proyectos.
    - IdEstado: estado de proyecto (Activo o Cerrado). Un proyecto se cierra, no se borra.
    - IdResponsable: usuario líder del proyecto.
    - CK_Proyectos_Fechas impide una fecha estimada de fin anterior a la de inicio.
*/
IF OBJECT_ID(N'dbo.Proyectos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Proyectos
    (
        Id               INT IDENTITY(1, 1) NOT NULL,
        Nombre           NVARCHAR(150)      NOT NULL,
        Descripcion      NVARCHAR(1000)     NULL,
        FechaInicio      DATE               NOT NULL,
        FechaFinEstimada DATE               NOT NULL,
        IdEstado         INT                NOT NULL,
        IdResponsable    INT                NOT NULL,
        FechaCreacion    DATETIME2(0)       NOT NULL CONSTRAINT DF_Proyectos_FechaCreacion DEFAULT (SYSUTCDATETIME()),

        CONSTRAINT PK_Proyectos          PRIMARY KEY (Id),
        CONSTRAINT CK_Proyectos_Fechas   CHECK (FechaFinEstimada >= FechaInicio),
        CONSTRAINT FK_Proyectos_Estados  FOREIGN KEY (IdEstado)
            REFERENCES dbo.Estados (Id)  ON DELETE NO ACTION ON UPDATE NO ACTION,
        CONSTRAINT FK_Proyectos_Usuarios FOREIGN KEY (IdResponsable)
            REFERENCES dbo.Usuarios (Id) ON DELETE NO ACTION ON UPDATE NO ACTION
    );

    CREATE INDEX IX_Proyectos_IdEstado      ON dbo.Proyectos (IdEstado);
    CREATE INDEX IX_Proyectos_IdResponsable ON dbo.Proyectos (IdResponsable);
END
GO
