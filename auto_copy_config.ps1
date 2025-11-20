# Script pour copier automatiquement le fichier de configuration dans le user data dir de Godot
# Usage: .\auto_copy_config.ps1
# Peut etre appele depuis MainPage.xaml.cs apres CreateSharedConfigFile

$configFile = "/sdcard/Download/platformgame_config.json"
$godotUserDataDir = "/data/data/com.company.mygodotgame/files"
$targetFile = "$godotUserDataDir/platformgame_config.json"

# Verifier que le fichier source existe
$exists = adb shell "test -f `"$configFile`" && echo 'EXISTS' || echo 'NOT_EXISTS'"
if ($exists -match "NOT_EXISTS") {
    exit 1
}

# Lire le contenu
$content = adb shell "cat `"$configFile`""

# Encoder en base64 et copier
$bytes = [System.Text.Encoding]::UTF8.GetBytes($content)
$base64 = [Convert]::ToBase64String($bytes)

# Copier via run-as
$result = adb shell "run-as com.company.mygodotgame sh -c 'echo `"$base64`" | base64 -d > $targetFile' 2>&1"

if ($result -match "package not debuggable") {
    exit 2
}

exit 0

