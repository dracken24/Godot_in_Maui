# Guide de débogage : Scores non enregistrés depuis Godot

## Problème
Les scores ne s'ajoutent plus à la base de données depuis Godot.

## Solutions de débogage

### 1. Voir les logs de Godot (Android)

```powershell
.\voir_logs_godot.ps1
```

Choisissez l'option **2** (Logs HTTP/Network) pour voir les requêtes API.

### 2. Voir les logs de l'API

L'API affiche maintenant des logs détaillés pour chaque requête POST :
- `=== REQUÊTE REÇUE ===` : Une requête a été reçue
- `Email: ...` : Détails de la requête
- `Tentative d'insertion dans Supabase...` : Début de l'insertion
- `Résultat inséré avec succès` : Succès
- `Erreur lors de la création` : Erreur

**Dans la console où vous avez lancé `dotnet run`**, vous verrez ces logs.

### 3. Vérifier que l'API reçoit les requêtes

Testez manuellement l'endpoint :

```powershell
.\voir_logs_api.ps1
```

Choisissez l'option **2** pour tester l'endpoint POST.

### 4. Vérifier la configuration de Godot

Godot doit avoir accès à :
1. **L'URL de l'API** : `http://192.168.2.169:5000` (ou celle dans votre `.env`)
2. **L'email de l'utilisateur** : passé via le fichier de configuration partagé

Le fichier de configuration est créé par `GodotLauncherService` dans :
- **Android** : `/data/data/com.companyname.platformgame/files/godot_config.json`

### 5. Vérifier les permissions réseau Android

Assurez-vous que l'app Godot a la permission INTERNET dans son `AndroidManifest.xml` :

```xml
<uses-permission android:name="android.permission.INTERNET" />
```

### 6. Vérifier la connectivité depuis Godot

Dans Godot, testez la connexion avec :

```gdscript
var http_request = HTTPRequest.new()
add_child(http_request)
http_request.request_completed.connect(_on_request_completed)

var url = "http://192.168.2.169:5000/api/gameresults/test"
var error = http_request.request(url)
if error != OK:
    print("Erreur de connexion: ", error)
```

### 7. Points de vérification

- [ ] L'API est en cours d'exécution (`dotnet run` dans `PlatformGame.Api`)
- [ ] L'IP de l'API est correcte (vérifiez avec `ipconfig`)
- [ ] Avast/firewall autorise les connexions sur le port 5000
- [ ] L'appareil Android et la machine sont sur le même réseau WiFi
- [ ] L'email est bien passé à Godot (vérifiez le fichier de config)
- [ ] Les logs de l'API montrent des requêtes reçues

### 8. Erreurs courantes

#### "Socket closed" ou "Connection timeout"
- **Cause** : Firewall bloque les connexions
- **Solution** : Autorisez le port 5000 dans Avast

#### "Email manquant"
- **Cause** : L'email n'est pas passé à Godot
- **Solution** : Vérifiez que `GodotLauncherService` crée bien le fichier de config

#### "Aucun résultat inséré"
- **Cause** : Erreur Supabase (permissions, connexion)
- **Solution** : Vérifiez les logs de l'API pour l'erreur exacte

## Commandes utiles

```powershell
# Voir les logs Godot
.\voir_logs_godot.ps1

# Tester l'API
.\voir_logs_api.ps1

# Vérifier l'IP
ipconfig

# Tester depuis Android
adb shell "curl http://192.168.2.169:5000/api/gameresults/test"
```

