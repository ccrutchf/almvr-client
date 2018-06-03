# Force Unity version.
& choco install unity --version 2018.1.2 -y --no-progress
& choco install autoit.commandline -y --no-progress

# We need a desktop to do automation
.\enable-desktop.ps1

& autoit3 .\install-unity.au3