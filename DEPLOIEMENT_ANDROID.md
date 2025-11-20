# Guide de déploiement Android - APK Godot

Ce guide explique comment déployer automatiquement l'APK Godot sur votre téléphone Android.

## Prérequis

1. **Android Debug Bridge (ADB)** installé
   - Fait partie du SDK Android Platform Tools
   - Téléchargement: https://developer.android.com/studio/releases/platform-tools
   - Ou installez Android Studio (ADB est inclus)

2. **Téléphone Android connecté**
   - Connectez le téléphone en USB
   - Activez le **Débogage USB** dans les options développeur
   - Autorisez l'ordinateur quand Android le demande

3. **APK exporté depuis Godot**
   - Exportez depuis Godot: `Project > Export > AndroidGame > Export Project`
   - L'APK sera créé dans `PlatformGame/Godot/MyGodotGame.apk`

## Utilisation du script

### Méthode 1: Script automatique (recommandé)

1. Ouvrez PowerShell dans le dossier `PlatformGame`
2. Exécutez le script:
   ```powershell
   .\deploy_godot_android.ps1
   ```

Le script va:
- ✅ Vérifier que ADB est disponible
- ✅ Détecter le premier téléphone connecté
- ✅ Trouver l'APK automatiquement
- ✅ Désinstaller l'ancienne version
- ✅ Installer la nouvelle version

### Méthode 2: Spécifier le chemin de l'APK

Si l'APK est dans un autre emplacement:
```powershell
.\deploy_godot_android.ps1 -ApkPath "C:\chemin\vers\MyGodotGame.apk"
```

### Méthode 3: Installation manuelle avec ADB

Si vous préférez faire manuellement:
```powershell
# 1. Vérifier les appareils connectés
adb devices

# 2. Désinstaller l'ancienne version (optionnel)
adb uninstall com.company.mygodotgame

# 3. Installer la nouvelle version
adb install -r "Godot\MyGodotGame.apk"
```

## Dépannage

### "ADB non trouvé"
- Installez Android SDK Platform Tools
- Ou ajoutez ADB au PATH système
- Ou installez Android Studio

### "Aucun appareil connecté"
- Vérifiez que le câble USB fonctionne
- Activez le débogage USB dans les options développeur
- Autorisez l'ordinateur sur le téléphone
- Essayez `adb devices` pour voir si l'appareil apparaît

### "Installation échouée"
- Vérifiez qu'il y a assez d'espace sur le téléphone
- Vérifiez que l'APK n'est pas corrompu (réexportez depuis Godot)
- Désinstallez manuellement l'ancienne version depuis les paramètres Android

### "Permission denied"
- Sur certains téléphones, vous devez autoriser l'installation depuis des sources inconnues
- Allez dans: `Paramètres > Sécurité > Sources inconnues` (ou similaire)

## Workflow de développement recommandé

1. **Modifier le jeu dans Godot**
2. **Exporter l'APK** depuis Godot
3. **Exécuter le script de déploiement**:
   ```powershell
   .\deploy_godot_android.ps1
   ```
4. **Tester depuis l'app MAUI**

## Notes

- Le script désinstalle automatiquement l'ancienne version avant d'installer la nouvelle
- Le package name utilisé est `com.company.mygodotgame` (configuré dans Godot et dans MainPage.xaml.cs)
- L'APK doit être signé pour être installé (déjà configuré dans `export_presets.cfg`)

