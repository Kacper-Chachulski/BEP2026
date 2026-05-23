$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$repoRoot = Resolve-Path (Join-Path $scriptDir '..')
$build = Join-Path $repoRoot 'AvatarFlaskApp\app\AvatarGame\static\Build'
if (-not (Test-Path $build)) { Write-Error "Build path missing: $build"; exit 1 }
$map = @{
    'AvatarGame.wasm.unityweb' = 'build.wasm'
    'AvatarGame.framework.js.unityweb' = 'build.framework.js'
    'AvatarGame.data.unityweb' = 'build.data'
    'AvatarGame.loader.js' = 'build.js'
}
foreach ($k in $map.Keys) {
    $src = Join-Path $build $k
    $dst = Join-Path $build $map[$k]
    if (Test-Path $src) {
        Copy-Item -Path $src -Destination $dst -Force
        Write-Host "Copied $k -> $($map[$k])"
    } else {
        Write-Host "Missing source: $src"
    }
}
Get-ChildItem -Path $build -File | Select Name, Length | Format-Table -AutoSize
