# Guide pour voir les logs Godot sur Android

## Problème résolu

Les logs `GD.Print()` dans Godot C# ne sont pas toujours visibles via `adb logcat`. J'ai ajouté plusieurs méthodes pour capturer les logs :

1. **Fichier de log** : Les logs sont maintenant ecrits dans `/sdcard/Download/godot_logs.txt` (accessible sans permissions speciales)
2. **Debug.WriteLine()** : En plus de `GD.Print()` pour maximiser les chances de voir les logs
3. **Scripts PowerShell** : Pour lire le fichier de log facilement

## Méthodes pour voir les logs

### Méthode 1 : Fichier de log (RECOMMANDE)

Les logs sont maintenant ecrits dans un fichier accessible :

```powershell
# Lire le fichier de log
.\read_godot_log_file.ps1

# OU surveiller en temps reel
.\watch_godot_logs.ps1
```

**Avantages** :
- Fonctionne toujours, meme si `adb logcat` ne capture pas les logs
- Fichier accessible sans permissions speciales
- Peut etre lu a tout moment

### Méthode 2 : Script automatique (PID)

```powershell
.\view_godot_all_logs.ps1
```

Ce script :
1. Trouve automatiquement le PID du processus Godot
2. Affiche tous les logs pour ce processus

### Méthode 3 : Logs avec tag monodroid

Les logs `Debug.WriteLine()` apparaissent avec le tag `monodroid` :

```powershell
adb logcat -s monodroid:V
```

### Méthode 4 : Tous les logs sans filtre

Pour voir tous les logs (peut être très verbeux) :

```powershell
adb logcat | Select-String -Pattern "UserEmailManager|End|godot" -CaseSensitive:$false
```

### Méthode 5 : Logs par PID

1. Trouver le PID :
```powershell
adb shell "ps -A | grep mygodotgame"
```

2. Afficher les logs pour ce PID :
```powershell
adb logcat --pid=<PID>
```

## Logs attendus

Quand le jeu démarre, vous devriez voir :
```
[UserEmailManager] Email de l'utilisateur: ...
[UserEmailManager] URL de l'API: ...
[End] UserEmailManager trouve. Email: ...
```

Quand le joueur atteint la fin :
```
[End] Player entered End
[End] Enregistrement du score: ...ms
[UserEmailManager] Requête HTTP envoyée pour enregistrer le score: ...ms
[UserEmailManager] OK Score enregistre avec succes!
```

## Dépannage

Si vous ne voyez toujours pas les logs :

1. **Vérifiez que l'APK est bien réexporté** avec les nouvelles modifications
2. **Vérifiez que le jeu est lancé** depuis l'app MAUI
3. **Essayez de vider les logs** avant de lancer :
   ```powershell
   adb logcat -c
   ```
4. **Utilisez le script automatique** : `.\view_godot_all_logs.ps1`

## Notes

- Les logs sont maintenant ecrits dans `/sdcard/Download/godot_logs.txt` sur Android
- Les logs `GD.Print()` peuvent ne pas apparaître dans `adb logcat` sur Android
- Les logs `Debug.WriteLine()` apparaissent avec le tag `monodroid` (si capturés)
- Toutes les methodes sont utilisees pour maximiser les chances de voir les logs
- **Recommandation** : Utilisez `.\watch_godot_logs.ps1` pour voir les logs en temps reel

