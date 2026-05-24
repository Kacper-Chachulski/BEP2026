$root = 'C:\Users\kacpe\Downloads\AvatarGameBEP-main\AvatarGameBEP-main'
$src = Join-Path $root 'AvatarFlaskApp\app\AvatarGame\BOFS_static'
$dst = Join-Path $root 'AvatarFlaskApp\app\static\BOFS_static'
Write-Host "src: $src"
Write-Host "dst: $dst"
if (-Not (Test-Path $src)) { Write-Host "SRC missing: $src"; exit 1 }

Remove-Item -LiteralPath $dst -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Force -Path $dst | Out-Null
Copy-Item -Path $src\* -Destination $dst -Recurse -Force
Write-Host "Copied BOFS_static to app static"
Get-ChildItem -Path $dst -Recurse | Select-Object FullName | Format-Table -AutoSize
