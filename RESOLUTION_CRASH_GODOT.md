# Résolution du problème de crash de l'app Godot

## Problème identifié

L'application Godot s'installe correctement mais s'éteint immédiatement au lancement. Les logs Android montrent :
- `Package [com.company.mygodotgame] reported as REPLACED, but missing application info. Assuming REMOVED.`
- L'app démarre puis s'arrête immédiatement

## Solutions à essayer

### Solution 1 : Vérifier les logs de crash détaillés

Lancez l'app Godot depuis l'émulateur et capturez immédiatement les logs :

```powershell
adb logcat -c  # Efface les logs précédents
adb logcat *:E AndroidRuntime:E > godot-crash.log
```

Puis lancez l'app et attendez quelques secondes. Arrêtez avec Ctrl+C et examinez le fichier `godot-crash.log`.

### Solution 2 : Vérifier les permissions dans Godot

Dans Godot 4.5.1 :
1. Allez dans **Project → Project Settings → Application → Config**
2. Vérifiez que les permissions nécessaires sont configurées
3. Pour un jeu simple, vous pouvez ne pas avoir besoin de permissions spéciales

### Solution 3 : Vérifier l'export Android dans Godot

1. **Project → Export**
2. Sélectionnez le preset **Android**
3. Vérifiez les paramètres suivants :

#### Dans l'onglet **Options** :
- **Export Format** : APK
- **Package/App Name** : `com.company.mygodotgame` (doit être en minuscules)
- **Version Code** : `1`
- **Version Name** : `1.0.0`
- **Minimum SDK** : `21` (Android 5.0) ou supérieur
- **Target SDK** : `34` ou la dernière version disponible

#### Dans l'onglet **Permissions** :
- Vérifiez que vous n'avez pas de permissions inutiles qui pourraient causer des problèmes

#### Dans l'onglet **Icons** :
- Assurez-vous que les icônes sont configurées correctement

### Solution 4 : Réexporter l'APK avec des paramètres minimaux

1. Dans Godot, créez un nouveau preset d'export Android
2. Configurez uniquement les paramètres essentiels :
   - Package/App Name
   - Version Code
   - Version Name
   - Minimum SDK
3. Exportez l'APK
4. Réinstallez :
   ```powershell
   adb uninstall com.company.mygodotgame
   adb install -r "Godot\MyGodotGame.apk"
   ```

### Solution 5 : Vérifier les ressources du projet Godot

Assurez-vous que votre projet Godot a :
1. Une scène principale définie dans **Project → Project Settings → Application → Run → Main Scene**
2. Toutes les ressources nécessaires sont incluses dans l'export
3. Aucune ressource manquante qui pourrait causer un crash au démarrage

### Solution 6 : Tester avec un projet Godot minimal

Créez un projet Godot minimal pour tester :
1. Créez une nouvelle scène avec juste un Label "Hello World"
2. Définissez cette scène comme Main Scene
3. Exportez en APK
4. Testez si cet APK minimal fonctionne

Si l'APK minimal fonctionne, le problème vient de votre projet principal.

## Vérification dans le code MAUI

Le code dans `MainPage.xaml.cs` a été amélioré pour :
1. Essayer `GetLaunchIntentForPackage` d'abord
2. Si ça échoue, créer un Intent explicite avec l'activité principale `com.godot.game.GodotApp`
3. Vérifier que l'activité peut être résolue avant de la lancer

## Commandes utiles

```powershell
# Vérifier que l'app est installée
adb shell pm list packages | Select-String -Pattern "godot"

# Voir les informations détaillées du package
adb shell dumpsys package com.company.mygodotgame

# Lancer l'app manuellement depuis ADB
adb shell am start -n com.company.mygodotgame/com.godot.game.GodotApp

# Voir les logs en temps réel
adb logcat | Select-String -Pattern "godot|Godot|FATAL|AndroidRuntime"
```

## Prochaines étapes

1. Testez le code amélioré dans votre app MAUI
2. Si l'app Godot crash toujours, capturez les logs détaillés avec `adb logcat`
3. Partagez les logs pour identifier la cause exacte du crash

