# Guide : Modifier le code Godot pour lire le fichier de configuration

## Problème

Godot ne peut pas lire le fichier de configuration créé par l'app MAUI à cause des permissions Android (Scoped Storage).

## Solution

Modifier le code Godot pour qu'il cherche le fichier dans son propre répertoire (`user://`) qui est accessible sans permissions spéciales.

## Étapes

### 1. Modifier `UserEmailManager.cs` dans Godot

Le fichier `CODE_GODOT_UserEmailManager.cs` contient le code complet à utiliser.

**Points importants** :

1. **Ordre de priorité** :
   - D'abord : Intent extras (le plus fiable)
   - Ensuite : Fichier dans `user://platformgame_config.json` (répertoire de l'app Godot)
   - Enfin : Autres emplacements

2. **Chemin `user://`** :
   - `user://` pointe vers `/data/data/com.company.mygodotgame/files/`
   - C'est le répertoire de données de l'app Godot
   - Accessible sans permissions spéciales

### 2. Modifications dans l'app MAUI

L'app MAUI a été modifiée pour créer le fichier dans :
- Le répertoire externe de Godot : `/storage/emulated/0/Android/data/com.company.mygodotgame/files/`
- Ce répertoire est accessible par Godot via `user://` après le premier lancement

### 3. Comment ça fonctionne

1. **L'app MAUI** crée le fichier dans plusieurs emplacements, y compris le répertoire externe de Godot
2. **Godot** cherche d'abord dans `user://platformgame_config.json` (son propre répertoire)
3. Si le fichier n'existe pas là, il cherche dans d'autres emplacements
4. Si aucun fichier n'est trouvé, il essaie de lire les Intent extras

### 4. Vérification

Après avoir modifié le code Godot :

1. Recompilez l'app Godot
2. Lancez l'app MAUI et cliquez sur "Lancer le jeu"
3. Vérifiez les logs Godot :
   ```powershell
   .\voir_logs_godot.ps1
   ```

Vous devriez voir :
```
[UserEmailManager] Fichier trouvé: /data/data/com.company.mygodotgame/files/platformgame_config.json
[UserEmailManager] ✓ Email chargé: [email]
```

### 5. Si le fichier n'est toujours pas trouvé

1. **Vérifiez que le répertoire externe de Godot existe** :
   ```powershell
   adb shell "ls -la /storage/emulated/0/Android/data/com.company.mygodotgame/files/"
   ```

2. **Créez le fichier manuellement pour tester** :
   ```powershell
   .\copier_config_vers_godot.ps1
   ```

3. **Vérifiez les permissions** :
   - L'app Godot doit avoir les permissions de lecture des fichiers
   - Vérifiez dans `AndroidManifest.xml` de Godot

## Notes importantes

- Le répertoire externe de Godot (`/storage/emulated/0/Android/data/com.company.mygodotgame/files/`) est créé automatiquement par Godot au premier lancement
- Le fichier dans `user://` est accessible immédiatement sans permissions spéciales
- Les Intent extras restent la méthode la plus fiable si le plugin est configuré

