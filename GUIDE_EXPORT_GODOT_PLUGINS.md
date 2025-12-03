# Guide : Export Godot avec Plugins Android

## Problème

Les plugins Android (`IntentExtrasPlugin` et `SharedPreferencesPlugin`) ne sont pas détectés après recompilation de l'APK.

## Solution

### Étape 1 : Vérifier les fichiers source

Les fichiers suivants doivent exister dans `android/src/com/godot/game/` :
- ✅ `GodotApp.java` (mis à jour avec l'enregistrement des plugins)
- ✅ `IntentExtrasPlugin.java`
- ✅ `SharedPreferencesPlugin.java`

### Étape 2 : Exporter depuis l'éditeur Godot

**IMPORTANT** : Ne modifiez PAS directement les fichiers dans `android/build/` car ce répertoire est régénéré lors de l'export !

1. **Ouvrir le projet Godot** :
   - Ouvrir `C:\Users\drack\Documents\Godot\untitled-platform-game\` dans l'éditeur Godot

2. **Exporter l'APK Android** :
   - Aller dans **Projet → Exporter...**
   - Sélectionner le preset **Android**
   - Cliquer sur **Exporter le projet**
   - Choisir un emplacement pour l'APK (ex: `untitled-platform-game.apk`)

3. **Vérifier que les fichiers sont copiés** :
   Après l'export, vérifiez que les fichiers dans `android/build/src/com/godot/game/` contiennent bien :
   - `GodotApp.java` avec les méthodes `onGodotSetupCompleted()` et `onGodotMainLoopStarted()`
   - `IntentExtrasPlugin.java`
   - `SharedPreferencesPlugin.java`

### Étape 3 : Installer le nouvel APK

```powershell
adb install -r "chemin/vers/untitled-platform-game.apk"
```

Ou utilisez votre méthode habituelle d'installation.

### Étape 4 : Vérifier dans les logs

Après recompilation et lancement, vous devriez voir dans les logs Android (`adb logcat | Select-String "GODOT"`) :

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
GODOT: GodotApp.onGodotSetupCompleted() terminé
```

Si vous ne voyez PAS ces logs, cela signifie que :
- L'APK n'a pas été recompilé avec les nouveaux fichiers
- Le fichier dans `src/` n'a pas été copié vers `build/src/` lors de l'export
- Il y a une erreur de compilation (vérifiez les logs de compilation)

## Dépannage

### Les logs "GODOT: GodotApp.onCreate()" n'apparaissent pas

**Cause** : Le fichier `GodotApp.java` dans `build/src/` n'est pas celui qui est compilé, ou l'APK n'a pas été recompilé.

**Solution** :
1. Vérifiez que le fichier dans `android/src/com/godot/game/GodotApp.java` contient bien tous les logs
2. Supprimez le répertoire `android/build/` (optionnel, il sera régénéré)
3. Recompilez l'APK depuis l'éditeur Godot
4. Vérifiez que les fichiers dans `build/src/` sont bien mis à jour après l'export

### Les plugins ne sont toujours pas détectés

**Cause** : Les plugins ne sont pas enregistrés correctement ou il y a une erreur lors de l'enregistrement.

**Solution** :
1. Vérifiez les logs pour voir s'il y a des erreurs lors de l'enregistrement des plugins
2. Vérifiez que les fichiers `IntentExtrasPlugin.java` et `SharedPreferencesPlugin.java` existent dans `build/src/com/godot/game/`
3. Vérifiez que les plugins héritent bien de `GodotPlugin` et ont la méthode `getPluginName()`

## Notes importantes

- Le répertoire `android/build/` est **régénéré** lors de chaque export
- Modifiez uniquement les fichiers dans `android/src/`
- Les fichiers dans `src/` sont copiés vers `build/src/` lors de l'export
- Si vous modifiez directement `build/src/`, vos modifications seront perdues au prochain export

