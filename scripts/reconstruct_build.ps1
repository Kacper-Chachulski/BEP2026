$root = 'C:\Users\kacpe\Downloads\AvatarGameBEP-main\AvatarGameBEP-main'
$src1 = Join-Path $root 'CU.BE\Library\Bee\artifacts\WebGL\build\debug_WebGL_wasm'
$src2 = Join-Path $root 'CU.BE\Library\Bee\artifacts\WebGL'
$dest = Join-Path $root 'AvatarFlaskApp\app\AvatarGame\Build'
Write-Host "Root: $root"
Write-Host "src1: $src1"
Write-Host "src2: $src2"
Write-Host "dest: $dest"

New-Item -ItemType Directory -Force -Path $dest | Out-Null

$filesToCopy = @(
    @{src=Join-Path $src1 'build.framework.js'; dst=Join-Path $dest 'AvatarGame.framework.js.unityweb'},
    @{src=Join-Path $src1 'build.wasm'; dst=Join-Path $dest 'AvatarGame.wasm.unityweb'},
    @{src=Join-Path $src1 'build.js'; dst=Join-Path $dest 'AvatarGame.loader.js'},
    @{src=Join-Path $src2 'webgl.data'; dst=Join-Path $dest 'AvatarGame.data.unityweb'}
)

foreach ($f in $filesToCopy) {
    if (Test-Path $f.src) {
        Copy-Item -Path $f.src -Destination $f.dst -Force
        Write-Host "Copied: $($f.src) -> $($f.dst)"
    } else {
        Write-Host "MISSING: $($f.src)"
    }
}

Write-Host "Build folder contents:"
Get-ChildItem -Path $dest -File | Select-Object Name, LastWriteTime | Format-Table -AutoSize
