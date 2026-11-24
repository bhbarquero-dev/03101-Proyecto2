/*
    Base de datos GestionProyectos.
    Se ejecuta contra master. Crea la base solo si no existe.
*/
IF DB_ID(N'GestionProyectos') IS NULL
BEGIN
    CREATE DATABASE GestionProyectos;
END
GO
