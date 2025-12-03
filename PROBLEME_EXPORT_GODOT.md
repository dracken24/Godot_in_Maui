# Problème : Godot ne copie pas les fichiers modifiés lors de l'export

## Problème identifié

Après modification de `android/src/com/godot/game/GodotApp.java` et recompilation de l'APK, les modifications n'apparaissent pas dans l'APK.

**Cause** : Le fichier dans `android/build/src/com/godot/game/GodotApp.java` n'est **PAS mis à jour** lors de l'export depuis l'éditeur Godot.

## Preuve

- Fichier `src/GodotApp.java` modifié à **17:47:26** (contient `Log.d()`)
- Fichier `build/src/GodotApp.java` date de **17:21:47** (contient encore `Log.v()`)
- Le fichier `build/src/` est **plus ancien** que `src/`

## Solution temporaire

Le fichier `build/src/GodotApp.java` a été **copié manuellement** depuis `src/` pour inclure les modifications (`Log.d()` au lieu de `Log.v()`).

## Solution permanente

### Option 1 : Supprimer build/ avant chaque export

Avant d'exporter l'APK depuis Godot :

1. **Supprimer le répertoire `android/build/`** :
   ```powershell
   Remove-Item -Recurse -Force "C:\Users\drack\Documents\Godot\untitled-platform-game\android\build"
   ```

2. **Exporter l'APK depuis Godot** :
   - Projet → Exporter... → Android → Exporter le projet
   - Godot régénérera le répertoire `build/` et copiera les fichiers depuis `src/`

### Option 2 : Copier manuellement après chaque modification

Après chaque modification de fichiers dans `android/src/` :

1. **Copier le fichier modifié vers `build/src/`** :
   ```powershell
   Copy-Item "C:\Users\drack\Documents\Godot\untitled-platform-game\android\src\com\godot\game\GodotApp.java" `
             "C:\Users\drack\Documents\Godot\untitled-platform-game\android\build\src\com\godot\game\GodotApp.java" `
             -Force
   ```

2. **Exporter l'APK depuis Godot**

### Option 3 : Script automatique

Créer un script PowerShell qui :
1. Copie tous les fichiers de `src/` vers `build/src/`
2. Lance l'export depuis Godot (si possible)

## Vérification

Après recompilation, vérifiez que le fichier `build/src/GodotApp.java` contient bien :
- `Log.d("GODOT", "GodotApp.onCreate() appelé")` (et non `Log.v()`)
- L'enregistrement des plugins dans `onGodotSetupCompleted()`

## Notes

- Le répertoire `android/build/` est **régénéré** lors de chaque export
- Les modifications dans `build/` sont **perdues** si elles ne sont pas dans `src/`
- Il semble que Godot ne copie pas toujours les fichiers depuis `src/` vers `build/src/` lors de l'export
- C'est peut-être un bug de Godot ou une configuration spécifique du projet

