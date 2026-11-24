<#
.SYNOPSIS
    Crea la base de datos GestionProyectos y carga los datos semilla.

.DESCRIPTION
    Ejecuta, en orden, 00_BaseDatos.sql, los scripts de Tablas/ y los de Datos/
    con sqlcmd. Todos los scripts son idempotentes: se pueden volver a ejecutar.

.PARAMETER Servidor
    Instancia de SQL Server. Por defecto, (localdb)\MSSQLLocalDB.

.PARAMETER Recrear
    Elimina la base de datos antes de crearla (pierde todos los datos).

.EXAMPLE
    ./database/crear-bd.ps1
    ./database/crear-bd.ps1 -Recrear
#>
param(
    [string] $Servidor = '(localdb)\MSSQLLocalDB',
    [switch] $Recrear
)

$ErrorActionPreference = 'Stop'
$baseDatos = 'GestionProyectos'

if (-not (Get-Command sqlcmd -ErrorAction SilentlyContinue)) {
    throw 'No se encontró sqlcmd. Instale las herramientas de línea de comandos de SQL Server.'
}

function Invoke-Script([string] $archivo, [string] $bd) {
    Write-Host "  $([IO.Path]::GetRelativePath($PSScriptRoot, $archivo))"
    # -b: termina con error si el script falla; -f 65001: lee los archivos como UTF-8.
    sqlcmd -S $Servidor -d $bd -E -b -f 65001 -i $archivo
    if ($LASTEXITCODE -ne 0) { throw "Falló $archivo" }
}

if ($Recrear) {
    Write-Host "Eliminando $baseDatos..."
    sqlcmd -S $Servidor -d master -E -b -Q "IF DB_ID(N'$baseDatos') IS NOT NULL BEGIN ALTER DATABASE [$baseDatos] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$baseDatos]; END"
    if ($LASTEXITCODE -ne 0) { throw 'No se pudo eliminar la base de datos.' }
}

Write-Host 'Base de datos:'
Invoke-Script (Join-Path $PSScriptRoot '00_BaseDatos.sql') 'master'

foreach ($carpeta in 'Tablas', 'Datos') {
    Write-Host "${carpeta}:"
    Get-ChildItem (Join-Path $PSScriptRoot $carpeta) -Filter *.sql |
        Sort-Object Name |
        ForEach-Object { Invoke-Script $_.FullName $baseDatos }
}

Write-Host "Listo: $baseDatos en $Servidor."
