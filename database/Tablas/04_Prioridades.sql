/*
    Prioridades de las tareas (catálogo).
    Identificadores fijos: 1 = Alta, 2 = Media, 3 = Baja.
*/
IF OBJECT_ID(N'dbo.Prioridades', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Prioridades
    (
        Id     INT           NOT NULL,
        Nombre NVARCHAR(20)  NOT NULL,

        CONSTRAINT PK_Prioridades        PRIMARY KEY (Id),
        CONSTRAINT UQ_Prioridades_Nombre UNIQUE (Nombre)
    );
END
GO
