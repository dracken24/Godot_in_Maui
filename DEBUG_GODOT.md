# Guide de débogage - Communication Godot vers API

## Problème
L'API backend ne reçoit pas d'appels du jeu Godot lorsque le joueur termine le niveau.

## Solutions

### 1. Voir les logs Godot

**IMPORTANT** : Les logs Godot ne sont PAS visibles dans Visual Studio quand le jeu est lancé depuis l'app MAUI. Vous devez utiliser `adb logcat` via PowerShell.

#### Méthode 1 : Script automatique (recommandé)

Utilisez le script de test qui affiche les logs en temps réel :

```powershell
.\test_godot_api.ps1
```

Ce script :
1. Vérifie qu'ADB est disponible et qu'un appareil est connecté
2. Nettoie les logs précédents
3. Affiche les logs Godot en temps réel avec filtres
4. Recherche les messages importants : `UserEmailManager`, `End`, `SaveScore`, etc.

**Procédure** :
1. Ouvrez un terminal PowerShell dans le dossier du projet
2. Lancez `.\test_godot_api.ps1`
3. Appuyez sur Entrée pour commencer
4. **Dans l'app MAUI**, lancez le jeu Godot
5. Terminez un niveau pour voir les logs d'enregistrement du score

#### Méthode 2 : Script simple

```powershell
.\view_godot_logs.ps1
```

#### Méthode 3 : Commande ADB directe

Si les scripts ne fonctionnent pas, utilisez directement :

```powershell
# Nettoyer les logs
adb logcat -c

# Afficher les logs avec filtres
adb logcat | Select-String -Pattern "UserEmailManager|End|SaveScore|Godot|com.company.mygodotgame" -CaseSensitive:$false
```

#### Ce que vous devriez voir dans les logs :

- `[UserEmailManager] Email de l'utilisateur: ...` - Email chargé
- `[UserEmailManager] URL de l'API: ...` - URL API chargée
- `[End] UserEmailManager trouvé. Email: ...` - UserEmailManager détecté
- `[End] Enregistrement du score: ...` - Score en cours d'enregistrement
- `[UserEmailManager] Requête HTTP envoyée...` - Requête envoyée à l'API
- `[UserEmailManager] ✓ Score enregistré avec succès!` - Succès
- `[UserEmailManager] ✗ ERREUR: ...` - Erreur (à noter)

### 2. Vérifier que le UserEmailManager est présent dans la scène

**IMPORTANT** : Le nœud `UserEmailManager` doit être ajouté dans votre scène principale Godot.

#### Étapes dans Godot :

1. Ouvrez votre scène principale (généralement `main.tscn` ou la scène de votre niveau)
2. Ajoutez un nœud `Node` (ou `Node2D` si vous préférez)
3. Renommez-le en `UserEmailManager`
4. Attachez le script `UserEmailManager.cs` à ce nœud
5. Le nœud peut être placé n'importe où dans l'arbre de scène (racine recommandée)

#### Vérification dans les logs :

Lancez le jeu et regardez les logs. Vous devriez voir :
```
[UserEmailManager] Email de l'utilisateur: dracken24@gmail.com
[UserEmailManager] URL de l'API: http://10.0.0.49:5000
[End] UserEmailManager trouvé. Email: dracken24@gmail.com
```

Si vous voyez :
```
[End] ERREUR: UserEmailManager non disponible
```
Cela signifie que le nœud n'est pas trouvé dans la scène.

### 3. Vérifier le chargement du fichier de configuration

Le fichier de configuration est créé par l'app MAUI à :
- Android : `/storage/emulated/0/Documents/platformgame_config.json`

Vérifiez dans les logs Godot :
```
[UserEmailManager] Fichier trouvé: /storage/emulated/0/Documents/platformgame_config.json
[UserEmailManager] ✓ Email chargé depuis le fichier: dracken24@gmail.com
```

Si le fichier n'est pas trouvé, vérifiez :
- Que l'app MAUI a bien créé le fichier (regardez les logs MAUI)
- Que l'APK Godot a les permissions de lecture des fichiers externes

### 4. Vérifier les permissions Android

L'APK Godot doit avoir la permission `INTERNET` dans son `AndroidManifest.xml`.

Dans Godot :
1. Projet → Exporter → Android
2. Vérifiez que "Internet" est coché dans les permissions
3. Ou éditez manuellement `android/app/src/main/AndroidManifest.xml` :

```xml
<uses-permission android:name="android.permission.INTERNET" />
```

### 5. Tester manuellement l'envoi du score

Quand le joueur atteint la fin du niveau, vous devriez voir dans les logs :

```
[End] Player entered End
[End] Enregistrement du score: 12345ms pour l'utilisateur: dracken24@gmail.com
[End] URL API: http://10.0.0.49:5000
[UserEmailManager] Requête HTTP envoyée pour enregistrer le score: 12345ms
[UserEmailManager] ✓ Score enregistré avec succès!
```

Si vous voyez une erreur, notez le message d'erreur exact.

### 6. Vérifier la connectivité réseau

Sur Android, l'émulateur/téléphone doit pouvoir accéder à `http://10.0.0.49:5000`.

Testez depuis l'app MAUI d'abord (cela fonctionne d'après vos logs).

Pour Godot, vérifiez que :
- L'API est bien accessible depuis l'appareil Android
- Le firewall Windows n'bloque pas les connexions
- L'IP `10.0.0.49` est correcte (utilisez `ipconfig` pour vérifier)

### 7. Commandes utiles

```powershell
# Voir tous les logs Android (peut être très verbeux)
adb logcat

# Voir seulement les logs de votre package Godot
adb logcat | Select-String "com.company.mygodotgame"

# Voir les logs avec filtres spécifiques (erreurs et warnings)
adb logcat -s "Godot:*" "*:E" "*:W"

# Vérifier que le fichier de configuration existe sur l'appareil
adb shell ls -la /storage/emulated/0/Documents/platformgame_config.json

# Afficher le contenu du fichier de configuration
adb shell cat /storage/emulated/0/Documents/platformgame_config.json

# Voir les logs en temps réel avec filtres personnalisés
adb logcat | Select-String -Pattern "UserEmailManager|End|SaveScore|Godot|platformgame_config" -CaseSensitive:$false

# Vérifier que l'APK Godot est installé
adb shell pm list packages | Select-String "com.company.mygodotgame"

# Voir les logs d'une application spécifique (PID)
adb logcat --pid=$(adb shell pidof -s com.company.mygodotgame)
```

## Checklist de débogage

- [ ] Le nœud `UserEmailManager` est présent dans la scène Godot
- [ ] Le script `UserEmailManager.cs` est attaché au nœud
- [ ] Les logs Godot montrent que l'email est chargé
- [ ] Les logs Godot montrent que `UserEmailManager` est trouvé par `End.cs`
- [ ] Le fichier de configuration existe sur l'appareil Android
- [ ] L'APK Godot a la permission INTERNET
- [ ] L'API est accessible depuis l'appareil Android
- [ ] Les logs montrent que la requête HTTP est envoyée
- [ ] Les logs montrent la réponse de l'API (succès ou erreur)

