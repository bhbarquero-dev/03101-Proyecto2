/*
    Cuentas de prueba (solo para desarrollo y demostración).

    Correo                             Contraseña     Rol
    admin@gestionproyectos.test        Admin#2026     Administrador
    ana.mora@gestionproyectos.test     Usuario#2026   Usuario
    luis.solis@gestionproyectos.test   Usuario#2026   Usuario

    Los hashes siguen el formato V3 de PasswordHasher de ASP.NET Core
    (PBKDF2 con HMAC-SHA512, 100 000 iteraciones, sal de 128 bits).
    Solo inserta las cuentas que no existan; no sobrescribe contraseñas cambiadas.
*/
INSERT INTO dbo.Usuarios (Nombre, Email, ContrasenaHash, IdRol)
SELECT origen.Nombre, origen.Email, origen.ContrasenaHash, origen.IdRol
FROM (VALUES
    (N'Administrador del sistema', N'admin@gestionproyectos.test',
     'AQAAAAIAAYagAAAAEE2ISMmccEsXVfPgFXjxdJoi2a0xi5pnmW6sBb5bOJVbRvUz8gdXl+gQcA4qxSltow==', 1),
    (N'Ana Mora',   N'ana.mora@gestionproyectos.test',
     'AQAAAAIAAYagAAAAEP+5U1b/t6F41JH6j5/kQWarZj49vctaU/Ux5dI+rqiF7wZIzhRiJ7fNBQyPHdeU5A==', 2),
    (N'Luis Solís', N'luis.solis@gestionproyectos.test',
     'AQAAAAIAAYagAAAAEP+5U1b/t6F41JH6j5/kQWarZj49vctaU/Ux5dI+rqiF7wZIzhRiJ7fNBQyPHdeU5A==', 2)
) AS origen (Nombre, Email, ContrasenaHash, IdRol)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Usuarios u WHERE u.Email = origen.Email);
GO
