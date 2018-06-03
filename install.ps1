# Force Unity version.
& choco install unity --version 2018.1.2 -y --no-progress
& choco install autoit.commandline -y --no-progress

# We need a desktop to do automation
.\enable-desktop.ps1

$installUnity = Get-Content .\install-unity.au3
$newLines = @()
foreach ($line in $installUnity) {
    if ($line -eq "; **Send User Name**") {
        $newLines += "Send(`"$($env:UNITY_EMAIL)`")"
    }
    elseif ($line -eq "") {
        $newLines += "Send(`"$($env:UNITY_PASSWORD)`")"
    }
    else {
        $newLines += $lines
    }
}
Set-Content -Path .\install.ps1 -Value $newLines

& autoit3 .\install-unity.au3