# Guide : Recompilation de l'APK Godot avec les plugins

## Problème actuel

Les logs montrent :
```
ERROR: Failed to retrieve non-existent singleton 'SharedPreferencesPlugin'.
ERROR: Failed to retrieve non-existent singleton 'IntentExtrasPlugin'.
```

**Cause** : L'APK Godot actuellement installé ne contient pas les nouveaux plugins.

## Solution : Recompiler l'APK Godot

### Étape 1 : Vérifier les fichiers plugins

Les fichiers suivants doivent exister :
- `C:\Users\drack\Documents\Godot\untitled-platform-game\android\src\com\godot\game\IntentExtrasPlugin.java`
- `C:\Users\drack\Documents\Godot\untitled-platform-game\android\src\com\godot\game\SharedPreferencesPlugin.java`
- `C:\Users\drack\Documents\Godot\untitled-platform-game\android\src\com\godot\game\GodotApp.java`

### Étape 2 : Recompiler l'APK depuis Godot

1. **Ouvrir le projet Godot** :
   - Ouvrir `C:\Users\drack\Documents\Godot\untitled-platform-game\` dans l'éditeur Godot

2. **Exporter l'APK Android** :
   - Aller dans **Projet → Exporter...**
   - Sélectionner le preset **Android**
   - Cliquer sur **Exporter le projet**
   - Choisir un emplacement pour l'APK (ex: `untitled-platform-game.apk`)

3. **Installer le nouvel APK** :
   ```powershell
   adb install -r "chemin/vers/untitled-platform-game.apk"
   ```
   Ou utilisez votre méthode habituelle d'installation.

### Étape 3 : Vérifier que les plugins sont inclus

Après la recompilation, les plugins seront automatiquement détectés par Godot car :
- Ils héritent de `GodotPlugin`
- Ils sont dans le package `com.godot.game`
- Ils sont compilés dans l'APK

### Vérification dans les logs

Après recompilation et lancement, vous devriez voir dans les logs :
```
[UserEmailManager] [OK] Plugin SharedPreferencesPlugin trouvé!
[UserEmailManager] [OK] Email reçu depuis SharedPreferences: aaa@aaa.com
```

OU

```
[UserEmailManager] [OK] Plugin IntentExtrasPlugin trouvé!
[UserEmailManager] [OK] Email reçu depuis Intent extras: aaa@aaa.com
```

## Ordre de priorité (après recompilation)

1. **SharedPreferences** (localStorage) - PRIORITÉ 1
2. **Intent Extras** (argc/argv) - PRIORITÉ 2
3. **Fichier de configuration** - PRIORITÉ 3

## Note importante

**Les plugins ne seront pas détectés tant que l'APK n'est pas recompilé !**

Les fichiers Java existent, mais ils doivent être compilés dans l'APK pour être utilisables.

