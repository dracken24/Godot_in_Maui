# Solution pour transférer l'email à Godot

## ✅ Solution implémentée : Plugin Android Intent Extras

Un plugin Android a été créé pour lire automatiquement les Intent extras passés par l'app MAUI. **Cette solution est automatique et ne nécessite aucune intervention manuelle.**

### Comment ça fonctionne

1. **L'app MAUI** passe l'email et l'URL de l'API via Intent extras lors du lancement de Godot
2. **GodotApp.java** lit les Intent extras au démarrage et les stocke dans des variables statiques
3. **IntentExtrasPlugin.java** expose ces valeurs à Godot via un plugin
4. **UserEmailManager.cs** utilise le plugin en priorité pour récupérer l'email

### Avantages

- ✅ **Automatique** : Aucune intervention manuelle nécessaire
- ✅ **Fiable** : Fonctionne à chaque lancement du jeu
- ✅ **Sécurisé** : Pas besoin de fichiers partagés ou de permissions spéciales
- ✅ **Transparent** : L'utilisateur n'a rien à faire

### Utilisation

Aucune action requise ! Le système fonctionne automatiquement :

1. L'utilisateur se connecte dans l'app MAUI
2. L'utilisateur clique sur "Lancer le jeu"
3. L'email est automatiquement transmis à Godot via les Intent extras
4. Godot lit l'email depuis le plugin et l'utilise pour enregistrer les scores

### Vérification

Pour vérifier que le plugin fonctionne, consultez les logs Godot :
```powershell
.\copy_godot_logs.ps1
```

Vous devriez voir :
```
[UserEmailManager] [OK] Email reçu depuis Intent extras: [email]
```

### Dépannage

Si le plugin ne fonctionne pas :
1. Vérifiez que les fichiers Java sont bien compilés dans l'APK
2. Vérifiez les logs Android : `adb logcat | grep -i "IntentExtrasPlugin"`
3. Vérifiez que le plugin est activé dans Godot (Projet -> Exporter -> Android -> Plugins)

## Solutions alternatives (non recommandées)

### Solution alternative 1: Copier le fichier via ADB

Cette solution nécessite une intervention manuelle et n'est pas viable pour les utilisateurs finaux.

### Solution alternative 2: Fichier partagé

Cette solution ne fonctionne pas sur Android moderne à cause des restrictions de permissions (Scoped Storage).

