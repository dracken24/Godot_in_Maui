# Diagnostic - Fichier de configuration non trouvé

## Problème identifié

Dans les logs Godot, on voit que le `UserEmailManager` cherche le fichier mais ne le trouve pas :
```
[UserEmailManager] Verification: /data/data/com.company.mygodotgame/files/../platformgame_config.json
[UserEmailManager] Verification: /data/data/com.company.mygodotgame/files/platformgame_config.json
```

Mais on ne voit **PAS** les vérifications pour :
- `/storage/emulated/0/Documents/platformgame_config.json` (premier chemin dans la liste)
- `/sdcard/Documents/platformgame_config.json` (deuxième chemin)

## Vérifications effectuées

✅ Le fichier existe : `/storage/emulated/0/Documents/platformgame_config.json`
✅ Le fichier est accessible depuis ADB
✅ Le contenu est correct : `{"email":"dracken24@gmail.com","apiBaseUrl":"http://10.0.0.49:5000",...}`

## Solutions possibles

### Solution 1 : Permissions Android (Scoped Storage)

Sur Android 10+, l'accès aux fichiers externes nécessite des permissions. Vérifiez dans Godot :

1. **Projet → Exporter → Android**
2. Vérifiez que les permissions suivantes sont cochées :
   - `READ_EXTERNAL_STORAGE`
   - `WRITE_EXTERNAL_STORAGE`
   - Ou utilisez `MANAGE_EXTERNAL_STORAGE` (Android 11+)

### Solution 2 : Utiliser le dossier de l'app Godot

Au lieu de `/storage/emulated/0/Documents/`, créons le fichier dans le dossier de données de l'app Godot :

**Modifier `MainPage.xaml.cs`** pour créer le fichier dans :
```
/data/data/com.company.mygodotgame/files/platformgame_config.json
```

Mais cela nécessite que l'app MAUI ait accès au dossier de l'app Godot, ce qui n'est pas possible sans root.

### Solution 3 : Utiliser un emplacement partagé accessible

Sur Android, les deux apps peuvent accéder à :
- `/sdcard/Android/data/com.company.mygodotgame/files/` (si l'app Godot le crée)
- `/sdcard/Download/` (accessible par toutes les apps)

### Solution 4 : Passer l'email via Intent extras (recommandé)

Au lieu d'un fichier, passer l'email directement via les Intent extras. Cela nécessite un plugin Android pour Godot.

## Action immédiate

1. **Réexportez l'APK Godot** avec les nouveaux logs ajoutés
2. **Réinstallez l'APK** : `.\deploy_godot_android.ps1`
3. **Relancez le jeu** et regardez les logs avec `.\view_all_godot_logs.ps1`
4. **Vérifiez** si tous les 4 chemins sont maintenant testés et si `FileExists()` retourne `true` pour `/storage/emulated/0/Documents/platformgame_config.json`

## Si le problème persiste

Si `FileExists()` retourne toujours `false` même si le fichier existe, c'est un problème de permissions Android. Dans ce cas, il faudra :

1. Ajouter les permissions dans le `AndroidManifest.xml` de Godot
2. Ou utiliser un autre emplacement de fichier (comme `/sdcard/Download/`)

