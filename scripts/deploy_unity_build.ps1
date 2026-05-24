param(
    [Parameter(Mandatory=$true)]
    [string]$SourcePath,
    [Parameter(Mandatory=$false)]
    [string]$DestPath = "AvatarFlaskApp\app\AvatarGame"
)

# Deploy Unity WebGL output into the Flask app's AvatarGame folder.
# Usage:
# .\deploy_unity_build.ps1 -SourcePath C:\tmp\UnityOut

Write-Host "Source:" $SourcePath
Write-Host "Destination:" $DestPath

if (-not (Test-Path $SourcePath)) {
    Write-Error "Source path does not exist: $SourcePath"
    exit 1
}

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $root

$fullDest = Join-Path (Get-Location) $DestPath
Write-Host "Full destination:" $fullDest

# Ensure destination exists
New-Item -ItemType Directory -Force -Path $fullDest | Out-Null

# If source contains index.html at its root, copy entire folder contents
if (Test-Path (Join-Path $SourcePath 'index.html')) {
    Write-Host "Found index.html in source. Copying entire output to destination..."
    Remove-Item -Recurse -Force (Join-Path $fullDest 'Build') -ErrorAction SilentlyContinue
    Remove-Item -Force (Join-Path $fullDest 'index.html') -ErrorAction SilentlyContinue
    robocopy $SourcePath $fullDest /MIR | Out-Host
    Write-Host "Deploy complete."
    exit 0
}

# Otherwise, look for a Build folder inside source
$srcBuild = Join-Path $SourcePath 'Build'
if (-not (Test-Path $srcBuild)) {
    # maybe the provided path already points to the Build folder
    if (Test-Path (Join-Path $SourcePath 'AvatarGame.loader.js')) {
        $srcBuild = $SourcePath
    } else {
        Write-Error "Could not find index.html or Build/ in source. Provide the Unity output folder (the folder that contains index.html)."
        exit 1
    }
}

Write-Host "Copying Build folder from $srcBuild to $fullDest\Build"
Remove-Item -Recurse -Force (Join-Path $fullDest 'Build') -ErrorAction SilentlyContinue
robocopy $srcBuild (Join-Path $fullDest 'Build') /MIR | Out-Host

# If destination lacks index.html, generate a minimal index.html referencing the files found
if (-not (Test-Path (Join-Path $fullDest 'index.html'))) {
    Write-Host "Generating index.html in destination based on Build contents..."
    $buildFiles = Get-ChildItem (Join-Path $fullDest 'Build') -File | Select-Object -ExpandProperty Name
    $loader = ($buildFiles | Where-Object {$_ -match '\.loader\.js$'})[0]
    $data = ($buildFiles | Where-Object {$_ -match '\.data.*$'})[0]
    $framework = ($buildFiles | Where-Object {$_ -match 'framework.*js.*'})[0]
    $code = ($buildFiles | Where-Object {$_ -match '\.wasm.*$|\.js\.wasm$|\.wasm$'})[0]

    $index = @"
<!DOCTYPE html>
<html>
  <head><meta charset='utf-8'><title>Unity Build</title></head>
  <body>
    <canvas id="unity-canvas"></canvas>
    <script src="Build/$loader"></script>
    <script>
      createUnityInstance(document.querySelector('#unity-canvas'), {
        dataUrl: 'Build/$data',
        frameworkUrl: 'Build/$framework',
        codeUrl: 'Build/$code',
      }, (progress)=>{})
      .then(()=>{})
      .catch(alert);
    </script>
  </body>
</html>
"@
    $index | Out-File -FilePath (Join-Path $fullDest 'index.html') -Encoding utf8
    Write-Host "Generated index.html ->" (Join-Path $fullDest 'index.html')
}

Write-Host "Deploy finished. Restart Flask server to serve new build."