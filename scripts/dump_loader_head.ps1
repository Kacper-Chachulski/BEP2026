$url = 'http://127.0.0.1:5000/AvatarGame/Build/AvatarGame.loader.js'
$r = Invoke-WebRequest -Uri $url -UseBasicParsing
$c = $r.Content
$len = [Math]::Min(2000, $c.Length)
Write-Output $c.Substring(0,$len)
