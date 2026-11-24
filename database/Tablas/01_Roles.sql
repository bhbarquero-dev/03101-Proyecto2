/*
    Roles del sistema (catálogo).
    Identificadores fijos, cargados por Datos/01_Roles.sql:
        1 = Administrador, 2 = Usuario.
*/
IF OBJECT_ID(N'dbo.Roles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Roles
    (
        Id          INT            NOT NULL,
        Nombre      NVARCHAR(50)   NOT NULL,
        Descripcion NVARCHAR(200)  NULL,

        CONSTRAINT PK_Roles        PRIMARY KEY (Id),
        CONSTRAINT UQ_Roles_Nombre UNIQUE (Nombre)
    );
END
GO
