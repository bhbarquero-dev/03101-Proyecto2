# Base de datos (SQL Server)

Scripts de la base de datos `GestionProyectos`. Los scripts son la fuente de verdad del esquema:
EF Core solo mapea las tablas existentes, sin migraciones.

Hay un archivo `.sql` por objeto. El prefijo numérico fija el orden de ejecución, que respeta
las claves foráneas. Todos los scripts son idempotentes y se pueden volver a ejecutar.

```
00_BaseDatos.sql        Crea la base de datos (se ejecuta contra master)
Tablas/                 Una tabla por archivo, con su PK, FK, restricciones e índices
  01_Roles.sql
  02_Usuarios.sql
  03_Estados.sql
  04_Prioridades.sql
  05_Proyectos.sql
  06_Tareas.sql
Datos/                  Datos semilla, un archivo por tabla
  01_Roles.sql          Administrador, Usuario
  02_Estados.sql        Activo, Cerrado (proyecto) / Pendiente, En progreso, Completada, Cancelada (tarea)
  03_Prioridades.sql    Alta, Media, Baja
  04_Usuarios.sql       Cuentas de prueba
  05_Proyectos.sql      Proyectos de demostración
  06_Tareas.sql         Tareas de demostración
crear-bd.ps1            Ejecuta todo lo anterior en orden con sqlcmd
```

## Crear la base de datos

Requiere `sqlcmd` (se instala con Visual Studio o con las herramientas de SQL Server).

```powershell
./database/crear-bd.ps1              # crea o completa la base en (localdb)\MSSQLLocalDB
./database/crear-bd.ps1 -Recrear     # la elimina y la crea desde cero
./database/crear-bd.ps1 -Servidor .\SQLEXPRESS
```

## Catálogos

Los catálogos usan identificadores fijos (sin `IDENTITY`) para que la API pueda referirse a ellos.

| Tabla | Id | Nombre |
|---|---|---|
| Roles | 1 / 2 | Administrador / Usuario |
| Estados (Proyecto) | 1 / 2 | Activo / Cerrado |
| Estados (Tarea) | 3 / 4 / 5 / 6 | Pendiente / En progreso / Completada / Cancelada |
| Prioridades | 1 / 2 / 3 | Alta / Media / Baja |

## Cuentas de prueba

Solo para desarrollo y demostración.

| Correo | Contraseña | Rol |
|---|---|---|
| admin@gestionproyectos.test | Admin#2026 | Administrador |
| ana.mora@gestionproyectos.test | Usuario#2026 | Usuario |
| luis.solis@gestionproyectos.test | Usuario#2026 | Usuario |

Las contraseñas se guardan como hash en el formato V3 de `PasswordHasher` de ASP.NET Core
(PBKDF2 con HMAC-SHA512, 100 000 iteraciones).
