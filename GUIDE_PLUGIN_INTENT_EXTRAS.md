# Guide : Plugin Intent Extras pour Godot

## Solution : Intent Extras (comme argc/argv)

**Réponse à votre question** : Oui, il existe une solution alternative comme passer des args à Godot (comme argc/argv) ! C'est exactement ce que fait cette solution avec les **Intent extras Android**.

## Comment ça fonctionne

1. **L'app MAUI** passe l'email via `Intent.PutExtra("user_email", email)` (comme des arguments de ligne de commande)
2. **Le plugin Android** (`IntentExtrasPlugin.java`) lit ces Intent extras au démarrage
3. **Godot** accède aux valeurs via le singleton `IntentExtrasPlugin`

## Fichiers créés/modifiés

### 1. Plugin Android (`IntentExtrasPlugin.java`)
- **Emplacement** : `C:\Users\drack\Documents\Godot\untitled-platform-game\android\src\com\godot\game\IntentExtrasPlugin.java`
- **Fonction** : Lit les Intent extras et les expose à Godot
- **Méthodes disponibles** :
  - `hasUserEmail()` : Vérifie si un email est disponible
  - `getUserEmail()` : Récupère l'email
  - `getApiBaseUrl()` : Récupère l'URL de l'API
  - `refreshFromIntent()` : Rafraîchit les valeurs depuis l'Intent

### 2. Code C# Godot (`UserEmailManager.cs`)
- **Modifié** : Amélioration de la gestion du plugin avec rafraîchissement automatique

## Activation du plugin dans Godot

### Méthode 1 : Automatique (Godot 4.x)

Dans Godot 4, les plugins Android sont **automatiquement détectés** s'ils :
- Héritent de `GodotPlugin`
- Sont dans le package `com.godot.game`
- Sont compilés dans l'APK

**Aucune configuration supplémentaire n'est nécessaire !**

### Méthode 2 : Vérification manuelle

1. Ouvrez le projet Godot
2. Allez dans **Projet → Exporter → Android**
3. Vérifiez que le plugin est listé dans la section **Plugins**
4. Si nécessaire, activez-le

## Compilation et test

### 1. Recompiler l'APK Godot

Le plugin sera automatiquement inclus lors de la compilation de l'APK Android.

### 2. Tester

1. Lancez le jeu depuis l'app MAUI
2. Vérifiez les logs Godot :
   ```powershell
   adb logcat | Select-String "IntentExtrasPlugin|UserEmailManager"
   ```

### 3. Logs attendus

**Si le plugin fonctionne** :
```
[IntentExtrasPlugin] Email lu depuis Intent: aaa@aaa.com
[UserEmailManager] [OK] Email reçu depuis Intent extras: aaa@aaa.com
```

**Si le plugin ne fonctionne pas** :
```
[UserEmailManager] Plugin IntentExtrasPlugin non trouvé
[UserEmailManager] Tentative de rafraîchissement depuis l'Intent...
```

## Avantages de cette solution

✅ **Automatique** : Comme argc/argv, les valeurs sont passées automatiquement  
✅ **Pas de permissions** : Pas besoin de permissions spéciales Android  
✅ **Pas de fichiers** : Pas besoin de fichiers partagés ou de permissions de stockage  
✅ **Fiable** : Fonctionne à chaque lancement  
✅ **Sécurisé** : Les données sont passées directement entre les apps  

## Dépannage

### Le plugin n'est pas trouvé

1. Vérifiez que le fichier Java est bien compilé :
   ```powershell
   adb shell "ls /data/app/com.company.mygodotgame*/base.apk"
   ```

2. Vérifiez les logs Android :
   ```powershell
   adb logcat | Select-String "IntentExtrasPlugin"
   ```

3. Vérifiez que le package est correct : `com.godot.game`

### L'email n'est pas lu

1. Vérifiez que l'app MAUI passe bien l'email :
   - Regardez les logs MAUI : `GodotLauncherService: Email ajouté comme extra dans l'Intent`

2. Vérifiez que l'Intent est bien reçu :
   - Regardez les logs Android : `[IntentExtrasPlugin] Email lu depuis Intent: ...`

3. Essayez de rafraîchir manuellement :
   - Le code C# essaie automatiquement de rafraîchir si le plugin existe mais l'email est vide

## Comparaison avec les autres solutions

| Solution | Avantages | Inconvénients |
|----------|-----------|--------------|
| **Intent Extras** ✅ | Automatique, pas de permissions, fiable | Nécessite un plugin Android |
| Fichier partagé | Simple | Permissions Android complexes |
| ContentProvider | Flexible | Complexe à implémenter |
| SharedPreferences | Standard Android | Nécessite un plugin |

## Conclusion

**Cette solution est la meilleure** car elle :
- Fonctionne comme argc/argv (arguments passés au démarrage)
- Est automatique et transparente
- Ne nécessite pas de permissions spéciales
- Est fiable et sécurisée

Il suffit de recompiler l'APK Godot et le plugin sera automatiquement inclus !

