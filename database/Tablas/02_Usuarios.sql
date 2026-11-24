/*
    Usuarios del sistema.
    - Email único: es el identificador de inicio de sesión.
    - ContrasenaHash guarda solo el hash (PBKDF2, formato PasswordHasher de ASP.NET Core).
    - Activo permite la baja lógica: un usuario desactivado conserva su historial.
*/
IF OBJECT_ID(N'dbo.Usuarios', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Usuarios
    (
        Id             INT IDENTITY(1, 1) NOT NULL,
        Nombre         NVARCHAR(100)      NOT NULL,
        Email          NVARCHAR(256)      NOT NULL,
        ContrasenaHash VARCHAR(256)       NOT NULL,
        IdRol          INT                NOT NULL,
        FechaRegistro  DATETIME2(0)       NOT NULL CONSTRAINT DF_Usuarios_FechaRegistro DEFAULT (SYSUTCDATETIME()),
        Activo         BIT                NOT NULL CONSTRAINT DF_Usuarios_Activo        DEFAULT (1),

        CONSTRAINT PK_Usuarios       PRIMARY KEY (Id),
        CONSTRAINT UQ_Usuarios_Email UNIQUE (Email),
        CONSTRAINT FK_Usuarios_Roles FOREIGN KEY (IdRol)
            REFERENCES dbo.Roles (Id) ON DELETE NO ACTION ON UPDATE NO ACTION
    );

    CREATE INDEX IX_Usuarios_IdRol ON dbo.Usuarios (IdRol);
END
GO
