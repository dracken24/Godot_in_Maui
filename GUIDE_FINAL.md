# Guide Final - Configuration Godot

## Problèmes résolus

1. ✅ **Logs Godot visibles** : Utilisez `.\copy_godot_logs.ps1` pour voir les logs depuis `user://godot_logs.txt`
2. ✅ **Email chargé** : Le fichier de configuration est copié dans le user data dir de l'app Godot
3. ⚠️ **Permission internet** : DOIT être activée dans `export_presets.cfg` pour que Godot puisse se connecter à l'API

## Configuration requise

### 1. Permission Internet (OBLIGATOIRE)

Dans `export_presets.cfg`, la ligne suivante DOIT être :
```
permissions/internet=true
```

**Sans cette permission, Godot ne peut pas se connecter à l'API et vous verrez l'erreur `CantConnect`.**

### 2. Fichier de configuration

Le fichier de configuration doit être copié dans le user data dir de l'app Godot après chaque réinstallation de l'APK :

```powershell
.\copy_config_to_godot.ps1
```

**Pourquoi ?** Le user data dir est supprimé lors de la désinstallation/réinstallation de l'APK.

## Workflow complet

### Après chaque modification du code Godot :

1. **Réexporter l'APK** :
   - Ouvrir le projet Godot
   - Vérifier que `permissions/internet=true` dans `export_presets.cfg`
   - Projet → Exporter → Android → Exporter le projet

2. **Réinstaller l'APK** :
   ```powershell
   .\deploy_godot_android.ps1
   ```

3. **Copier le fichier de configuration** :
   ```powershell
   .\copy_config_to_godot.ps1
   ```
   ⚠️ **IMPORTANT** : À faire après chaque réinstallation !

4. **Lancer le jeu depuis l'app MAUI**

5. **Vérifier les logs** :
   ```powershell
   .\copy_godot_logs.ps1
   ```

## Vérifications

### Si l'email n'est pas chargé :
- Vérifier que le fichier de configuration a été copié : `.\copy_config_to_godot.ps1`
- Vérifier les logs : `.\copy_godot_logs.ps1`
- Chercher : `[UserEmailManager] Fichier trouve: /data/data/com.company.mygodotgame/files/platformgame_config.json`

### Si la connexion HTTP échoue (`CantConnect`) :
- Vérifier que `permissions/internet=true` dans `export_presets.cfg`
- Réexporter l'APK
- Réinstaller l'APK
- Vérifier que l'API est accessible : `adb shell "curl http://10.0.0.49:5000/api/gameresults/stats"`

## Logs attendus (succès)

```
[UserEmailManager] Fichier trouve: /data/data/com.company.mygodotgame/files/platformgame_config.json
[UserEmailManager] Email charge depuis le fichier: dracken24@gmail.com
[End] Enregistrement du score: 3119ms
[UserEmailManager] Requete HTTP envoyee pour enregistrer le score: 3119ms
[UserEmailManager] OK Score enregistre avec succes!
```

## Scripts disponibles

- `.\copy_godot_logs.ps1` : Voir les logs Godot
- `.\copy_config_to_godot.ps1` : Copier le fichier de configuration
- `.\deploy_godot_android.ps1` : Déployer l'APK Godot
- `.\diagnostic_logs_godot.ps1` : Diagnostic complet

