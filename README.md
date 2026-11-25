# 03101-Proyecto2

UNED - Proyecto 2 del curso 03101 Programación avanzada en web.

Sistema de Gestión de Proyectos y Tareas: aplicación ASP.NET MVC que consume una API REST
(ASP.NET Web API) con persistencia en SQL Server.

## Estructura

```
03101-Proyecto2.slnx                  Solución
global.json                           Versión fijada del SDK de .NET
src/
├── 03101-Proyecto2.Api/              API REST (capa de servicios / backend)
│   ├── Controllers/                  Endpoints REST (auth, usuarios, proyectos, tareas, indicadores)
│   ├── Data/                         Contexto / acceso a datos
│   ├── Entities/                     Entidades del modelo de datos
│   ├── Services/                     Lógica de negocio
│   └── Security/                     Autenticación y autorización
├── 03101-Proyecto2.Contracts/        DTOs compartidos entre API y Web
│   ├── Auth/  Usuarios/  Proyectos/  Tareas/  Indicadores/  Catalogos/
└── 03101-Proyecto2.Web/              ASP.NET MVC (capa de presentación / cliente)
    ├── Controllers/                  Controladores MVC (consumen la API, sin acceso a BD)
    ├── Services/                     Clientes HTTP hacia la API
    ├── Models/ViewModels/            Modelos de vista
    └── Views/
database/                             Scripts SQL Server (esquema y datos semilla)
docs/                                 Material del documento técnico
```

## Requisitos

- .NET SDK 10 (fijado en `global.json`)
- SQL Server Express LocalDB, incluido con Visual Studio (instancia `(localdb)\MSSQLLocalDB`)

## Base de datos

Antes de la primera ejecución, crear la base de datos y cargar los datos semilla
(detalle y cuentas de prueba en [database/README.md](database/README.md)):

```
./database/crear-bd.ps1
```

## Cuentas de prueba

Los datos semilla crean estas cuentas (solo para desarrollo y demostración):

| Correo | Contraseña | Rol |
|---|---|---|
| admin@gestionproyectos.test | Admin#2026 | Administrador |
| ana.mora@gestionproyectos.test | Usuario#2026 | Usuario |
| luis.solis@gestionproyectos.test | Usuario#2026 | Usuario |

## Ejecución

Las dos aplicaciones deben correr al mismo tiempo (la Web consume la API).

- **Visual Studio:** elegir el perfil de inicio **API + Web** (definido en `03101-Proyecto2.slnLaunch`) y presionar F5.
- **Terminal:** `./run.ps1` (PowerShell 7). Compila una vez, levanta ambas y Ctrl+C detiene las dos.

Por separado:

```
dotnet build 03101-Proyecto2.slnx
dotnet run --project src/03101-Proyecto2.Api --launch-profile https
dotnet run --project src/03101-Proyecto2.Web --launch-profile https
```

- API: https://localhost:7005
- Web: https://localhost:7286
