# Solution : Logs VERBOSE non visibles

## Problème identifié

Les logs `GODOT: GodotApp.onCreate()` n'apparaissaient pas dans les logs Android, même après recompilation de l'APK.

## Cause

Les logs utilisaient `Log.v()` (VERBOSE), qui est le niveau de log le plus bas. Par défaut, `adb logcat` **filtre les logs VERBOSE** et ne les affiche pas.

## Solution appliquée

Tous les `Log.v("GODOT", ...)` ont été changés en `Log.d("GODOT", ...)` (DEBUG) dans le fichier :
- `C:\Users\drack\Documents\Godot\untitled-platform-game\android\src\com\godot\game\GodotApp.java`

Les logs DEBUG sont visibles par défaut dans `adb logcat`.

## Niveaux de log Android

- `Log.v()` = VERBOSE (niveau le plus bas, souvent filtré)
- `Log.d()` = DEBUG (visible par défaut) ✅
- `Log.i()` = INFO (visible par défaut)
- `Log.w()` = WARNING (visible par défaut)
- `Log.e()` = ERROR (toujours visible)

## Prochaines étapes

1. **Recompiler l'APK depuis l'éditeur Godot** :
   - Ouvrir le projet Godot
   - Projet → Exporter... → Android → Exporter le projet

2. **Installer le nouvel APK** :
   ```powershell
   adb install -r "chemin/vers/votre-apk.apk"
   ```

3. **Vérifier les logs** :
   ```powershell
   adb logcat | Select-String "GODOT"
   ```

Vous devriez maintenant voir :
```
GODOT: GodotApp.onCreate() appelé
GODOT: Intent reçu, vérification des extras...
GODOT: Intent extra user_email: aaa@aaa.com
GODOT: GodotApp.onCreate() terminé
GODOT: GodotApp.onGodotSetupCompleted() appelé
GODOT: Création du plugin IntentExtrasPlugin...
GODOT: IntentExtrasPlugin enregistré avec succès - Nom: IntentExtrasPlugin
GODOT: Création du plugin SharedPreferencesPlugin...
GODOT: SharedPreferencesPlugin enregistré avec succès - Nom: SharedPreferencesPlugin
```

## Alternative : Voir les logs VERBOSE

Si vous voulez garder `Log.v()`, vous pouvez utiliser :
```powershell
adb logcat *:V | Select-String "GODOT"
```

Le `*:V` force logcat à afficher tous les logs VERBOSE.

