$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$repoRoot = Resolve-Path (Join-Path $scriptDir '..')
$root = $repoRoot
$src = Join-Path $root 'AvatarFlaskApp\app\AvatarGame\Build'
$dst = Join-Path $root 'AvatarFlaskApp\app\AvatarGame\static\Build'
Write-Host "src: $src"
Write-Host "dst: $dst"
New-Item -ItemType Directory -Force -Path $dst | Out-Null
Get-ChildItem -Path $src -File | ForEach-Object {
    $destPath = Join-Path $dst $_.Name
    Copy-Item -Path $_.FullName -Destination $destPath -Force
    Write-Host "Copied $_.Name -> $destPath"
}
Get-ChildItem -Path $dst -File | Select-Object Name, Length | Format-Table -AutoSize
