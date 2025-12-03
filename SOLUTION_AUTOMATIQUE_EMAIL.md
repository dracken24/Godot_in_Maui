# Solution automatique : Transmission de l'email à Godot

## Solution implémentée

L'app MAUI crée automatiquement le fichier de configuration dans le **répertoire externe de Godot** qui est accessible par les deux applications.

### Comment ça fonctionne

1. **L'app MAUI** crée le fichier dans `/storage/emulated/0/Android/data/com.company.mygodotgame/files/platformgame_config.json`
2. **Godot** lit le fichier depuis ce même emplacement
3. **Automatique** : Aucune intervention manuelle nécessaire

### Avantages

- ✅ **Automatique** : Le fichier est créé à chaque lancement
- ✅ **Accessible** : Le répertoire externe de Godot est accessible par l'app MAUI
- ✅ **Fiable** : Fonctionne à chaque fois
- ✅ **Pas de scripts** : Tout est géré par le code

## Code modifié

### App MAUI (`GodotLauncherService.cs`)

Le fichier est créé automatiquement dans :
- `/storage/emulated/0/Android/data/com.company.mygodotgame/files/platformgame_config.json`

### Code Godot (`UserEmailManager.cs`)

Modifiez votre `UserEmailManager.cs` dans Godot pour chercher le fichier dans cet ordre :

1. **Répertoire externe de Godot** (priorité) : `/storage/emulated/0/Android/data/com.company.mygodotgame/files/platformgame_config.json`
2. Répertoire externe de l'app MAUI
3. Répertoire `user://` de Godot
4. Autres emplacements (fallback)

Voir `CODE_GODOT_UserEmailManager.cs` pour le code complet.

## Vérification

Après avoir modifié le code Godot :

1. Recompilez l'app MAUI
2. Recompilez l'app Godot
3. Lancez l'app MAUI et cliquez sur "Lancer le jeu"
4. Vérifiez les logs :
   ```powershell
   .\voir_logs_godot.ps1 | Select-String "Fichier trouvé"
   ```

Vous devriez voir :
```
[UserEmailManager] Fichier trouvé: /storage/emulated/0/Android/data/com.company.mygodotgame/files/platformgame_config.json
[UserEmailManager] ✓ Email chargé: [email]
```

## Notes importantes

- Le répertoire externe de Godot est créé automatiquement par Godot au premier lancement
- L'app MAUI peut écrire dans ce répertoire car il est dans le stockage externe partagé
- Si le répertoire n'existe pas encore, l'app MAUI le crée automatiquement
- L'email est aussi transmis via Intent extras comme solution de secours

