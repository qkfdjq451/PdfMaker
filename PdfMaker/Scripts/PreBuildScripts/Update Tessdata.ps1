param(
    [Parameter(Mandatory=$true)]
    [string]$ProjectDir
)

$tessDataFiles = @('eng.traineddata', 'kor_vert.traineddata')
$repoUrl = 'https://github.com/tesseract-ocr/tessdata/raw/main/'
$tessDataDir = "$ProjectDir\tessdata"

if (-not(Test-Path $tessDataDir)) {
    New-Item -ItemType Directory -Path $tessDataDir
}

foreach ($file in $tessDataFiles) {
    $url = $repoUrl + $file
    $target = Join-Path $tessDataDir $file

    Write-Host "Checking $file for updates..."

    $webRequest = Invoke-WebRequest $url -Method Head
    $remoteSize = $webRequest.Headers.'Content-Length'

    if (-not(Test-Path $target) -or ((Get-Item $target).Length -ne $remoteSize)) {
        Write-Host "Downloading latest $file..."
        Invoke-WebRequest -Uri $url -OutFile $target
    } else {
        Write-Host "$file is already up to date."
    }
}
