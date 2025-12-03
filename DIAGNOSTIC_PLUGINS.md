# Diagnostic : Plugins Android non détectés

## Situation actuelle

### ✅ Ce qui est en place
1. **Fichiers créés** :
   - `android/src/com/godot/game/IntentExtrasPlugin.java` ✓
   - `android/src/com/godot/game/SharedPreferencesPlugin.java` ✓
   - `android/src/com/godot/game/GodotApp.java` (modifié) ✓
   - `android/build/src/com/godot/game/IntentExtrasPlugin.java` ✓
   - `android/build/src/com/godot/game/SharedPreferencesPlugin.java` ✓
   - `android/build/src/com/godot/game/GodotApp.java` (modifié) ✓

2. **Code correct** :
   - `GodotApp.java` contient bien les logs `Log.v("GODOT", ...)`
   - `GodotApp.java` enregistre bien les plugins dans `onGodotSetupCompleted()`
   - Les plugins héritent bien de `GodotPlugin`

### ❌ Ce qui ne fonctionne pas
1. **Aucun log de `GodotApp.onCreate()`** dans les logs Android
2. **Les plugins ne sont pas détectés** :
   ```
   ERROR: Failed to retrieve non-existent singleton 'SharedPreferencesPlugin'.
   ERROR: Failed to retrieve non-existent singleton 'IntentExtrasPlugin'.
   ```

## Cause du problème

**L'APK actuellement installé sur l'appareil n'a PAS été recompilé avec les nouveaux fichiers.**

Les logs montrent :
- `V Godot   : OnGodotSetupCompleted` (log de Godot lui-même)
- `V Godot   : OnGodotMainLoopStarted` (log de Godot lui-même)
- **AUCUN** log `GODOT: GodotApp.onCreate() appelé` (notre code)

Cela signifie que le code modifié dans `GodotApp.java` **n'est pas exécuté** car l'APK installé est une ancienne version.

## Solution

### Étape 1 : Recompiler l'APK depuis l'éditeur Godot

1. **Ouvrir le projet Godot** :
   ```
   C:\Users\drack\Documents\Godot\untitled-platform-game\
   ```

2. **Exporter l'APK Android** :
   - Aller dans **Projet → Exporter...**
   - Sélectionner le preset **Android**
   - Cliquer sur **Exporter le projet**
   - Choisir un emplacement pour l'APK

3. **Vérifier qu'il n'y a pas d'erreurs de compilation** :
   - Regardez la console de l'éditeur Godot
   - Vérifiez qu'il n'y a pas d'erreurs Java

### Étape 2 : Installer le nouvel APK

```powershell
adb install -r "chemin/vers/votre-apk.apk"
```

### Étape 3 : Vérifier dans les logs

Après recompilation et installation, lancez le jeu et vérifiez les logs :

```powershell
adb logcat | Select-String "GODOT"
```

Vous devriez voir :
```
GODOT: GodotApp.onCreate() appelé
GODOT: Intent reçu, vérification des extras...
GODOT: GodotApp.onGodotSetupCompleted() appelé
GODOT: IntentExtrasPlugin enregistré avec succès
GODOT: SharedPreferencesPlugin enregistré avec succès
```

**Si ces logs n'apparaissent toujours pas**, cela signifie que :
- L'APK n'a pas été recompilé correctement
- Il y a une erreur de compilation silencieuse
- Le fichier dans `build/src/` n'est pas celui qui est compilé

## Vérifications supplémentaires

### Vérifier que les fichiers sont bien copiés

Après l'export, vérifiez que les fichiers dans `android/build/src/com/godot/game/` contiennent bien :
- Les logs `Log.v("GODOT", ...)`
- L'enregistrement des plugins dans `onGodotSetupCompleted()`

### Vérifier les erreurs de compilation

Si vous utilisez Android Studio ou Gradle pour compiler, vérifiez les logs de compilation pour voir s'il y a des erreurs.

## Conclusion

**Le problème n'est PAS dans le code** - les fichiers sont corrects et contiennent le bon code.

**Le problème est que l'APK n'a pas été recompilé** avec les nouveaux fichiers.

**Action requise** : Recompiler l'APK depuis l'éditeur Godot et vérifier qu'il n'y a pas d'erreurs de compilation.

