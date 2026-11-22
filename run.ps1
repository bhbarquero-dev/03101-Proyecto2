#Requires -Version 7
<#
.SYNOPSIS
    Compila la solución y levanta la API y la aplicación Web al mismo tiempo.
.DESCRIPTION
    Compila una sola vez (para que los dos proyectos no compilen Contracts en paralelo)
    y luego ejecuta ambos con el perfil https. Ctrl+C detiene los dos.
    API: https://localhost:7005  ·  Web: https://localhost:7286
#>
$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot

dotnet build (Join-Path $root '03101-Proyecto2.slnx')
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$api = Start-Process dotnet -NoNewWindow -PassThru -ArgumentList @(
    'run', '--no-build', '--launch-profile', 'https',
    '--project', (Join-Path $root 'src/03101-Proyecto2.Api'))

try {
    dotnet run --no-build --launch-profile https --project (Join-Path $root 'src/03101-Proyecto2.Web')
}
finally {
    # dotnet run lanza la aplicación como proceso hijo: se detiene el árbol completo.
    if (-not $api.HasExited) { $api.Kill($true) }
}
