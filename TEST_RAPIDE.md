# Guide de test rapide - Communication Godot -> API

## Problème
Vous ne voyez pas les logs Godot dans Visual Studio quand le jeu est lancé depuis l'app MAUI.

## Solution : Utiliser ADB Logcat

### Étape 1 : Préparer l'environnement

1. **Assurez-vous que l'API backend est en cours d'exécution** :
   ```powershell
   cd PlatformGame.Api
   dotnet run --launch-profile http
   ```
   L'API doit être accessible sur `http://10.0.0.49:5000`

2. **Assurez-vous qu'un appareil Android est connecté** :
   ```powershell
   adb devices
   ```
   Vous devriez voir votre appareil listé.

### Étape 2 : Lancer le script de test

Dans un **nouveau terminal PowerShell** (gardez l'API en cours d'exécution dans l'autre) :

**Option 1 : Script avec filtres (recommandé)**
```powershell
cd C:\Users\drack\Documents\prog\Developpement-Application--Maui-..-\PlatformGame
.\test_godot_api.ps1
```

**Option 2 : Tous les logs Godot (si Option 1 ne montre rien)**
```powershell
.\view_all_godot_logs.ps1
```

Le script va :
- Vérifier qu'ADB est disponible
- Vérifier qu'un appareil est connecté
- Chercher le processus Godot (PID)
- Nettoyer les logs précédents
- Afficher les logs Godot en temps réel

**IMPORTANT** : Si le script dit "Processus Godot non trouvé", cela signifie que le jeu n'est pas encore lancé. Lancez-le depuis l'app MAUI d'abord.

### Étape 3 : Tester le jeu

1. **Dans l'app MAUI** (sur votre téléphone/émulateur) :
   - Connectez-vous si nécessaire
   - Cliquez sur "Lancer le jeu Godot"
   - Jouez jusqu'à la fin du niveau

2. **Dans le terminal PowerShell** (où le script tourne) :
   - Vous devriez voir les logs apparaître en temps réel
   - Recherchez les messages suivants :

#### Messages attendus (dans l'ordre) :

```
[UserEmailManager] Recherche du fichier de configuration dans X emplacements...
[UserEmailManager] Fichier trouvé: /storage/emulated/0/Documents/platformgame_config.json
[UserEmailManager] Contenu du fichier: {"email":"...","apiBaseUrl":"..."}
[UserEmailManager] ✓ Email chargé depuis le fichier: dracken24@gmail.com
[UserEmailManager] ✓ URL API chargée depuis le fichier: http://10.0.0.49:5000
[End] UserEmailManager trouvé. Email: dracken24@gmail.com
[End] Player entered End
[End] Enregistrement du score: 12345ms pour l'utilisateur: dracken24@gmail.com
[End] URL API: http://10.0.0.49:5000
[UserEmailManager] Requête HTTP envoyée pour enregistrer le score: 12345ms
[UserEmailManager] ✓ Score enregistré avec succès!
[UserEmailManager] Réponse: {"id":...,"email":"...","completionTimeMs":...}
```

### Étape 4 : Vérifier dans l'API

Dans la console de l'API (où `dotnet run` est en cours), vous devriez voir :

```
info: Microsoft.AspNetCore.Hosting.Diagnostics[1]
      Request starting HTTP POST /api/gameresults/results ...
info: Microsoft.AspNetCore.Hosting.Diagnostics[2]
      Request finished HTTP POST /api/gameresults/results - 201
```

## Problèmes courants

### Aucun log n'apparaît

1. **Vérifiez que le jeu Godot est bien lancé** :
   ```powershell
   # Vérifier que l'APK est installé
   adb shell pm list packages | Select-String "com.company.mygodotgame"
   
   # Vérifier que le processus est en cours d'exécution
   adb shell "pidof com.company.mygodotgame"
   ```
   Si le PID n'apparaît pas, le jeu n'est pas lancé. Lancez-le depuis l'app MAUI.

2. **Vérifiez que le fichier de configuration existe** :
   ```powershell
   adb shell cat /storage/emulated/0/Documents/platformgame_config.json
   ```

3. **Essayez de voir tous les logs du processus Godot** :
   ```powershell
   # Obtenir le PID
   $pid = adb shell "pidof com.company.mygodotgame"
   
   # Afficher tous les logs de ce processus
   adb logcat --pid=$pid
   ```

4. **Vérifiez que les logs Godot sont activés** :
   - Dans Godot, les `GD.Print()` devraient apparaître dans logcat
   - Si vous ne voyez rien, essayez `view_all_godot_logs.ps1` qui affiche TOUS les logs du processus

5. **Vérifiez les permissions de logcat** :
   ```powershell
   # Vérifier que vous pouvez voir les logs
   adb logcat -d | Select-String "com.company.mygodotgame" | Select-Object -First 10
   ```

### Le UserEmailManager n'est pas trouvé

**Solution** : Ajoutez le nœud `UserEmailManager` dans votre scène Godot :
1. Ouvrez votre scène principale dans Godot
2. Ajoutez un nœud `Node` nommé `UserEmailManager`
3. Attachez le script `UserEmailManager.cs` à ce nœud
4. Réexportez l'APK et réinstallez-le

### L'email n'est pas chargé

**Vérifiez le fichier de configuration** :
```powershell
adb shell cat /storage/emulated/0/Documents/platformgame_config.json
```

Le fichier doit contenir :
```json
{"email":"votre@email.com","apiBaseUrl":"http://10.0.0.49:5000","timestamp":"..."}
```

Si le fichier n'existe pas ou est vide, relancez le jeu depuis l'app MAUI (le fichier est créé à chaque lancement).

### La requête HTTP échoue

**Vérifiez la connectivité réseau** :
1. L'API doit être accessible depuis l'appareil Android
2. L'IP `10.0.0.49` doit être correcte (utilisez `ipconfig` pour vérifier)
3. Le firewall Windows ne doit pas bloquer les connexions

**Vérifiez les permissions Android** :
L'APK Godot doit avoir la permission `INTERNET` dans son `AndroidManifest.xml`.

## Commandes de débogage rapides

```powershell
# Voir tous les logs Godot (sans filtre)
adb logcat | Select-String "com.company.mygodotgame"

# Voir seulement les erreurs
adb logcat *:E | Select-String "com.company.mygodotgame"

# Vérifier le fichier de configuration
adb shell cat /storage/emulated/0/Documents/platformgame_config.json

# Vérifier que l'APK est installé
adb shell pm list packages | Select-String "com.company.mygodotgame"

# Voir les processus en cours
adb shell ps | Select-String "godot"
```

