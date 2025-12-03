# Solution : Transmission de l'email à Godot

## Problème actuel

Godot ne peut pas lire le fichier de configuration créé par l'app MAUI à cause des **permissions Android (Scoped Storage)**. Les fichiers dans `/storage/emulated/0/` nécessitent des permissions spéciales que Godot n'a pas.

## Solutions possibles

### ✅ Solution 1 : Utiliser les Intent Extras (RECOMMANDÉ)

**Avantage** : Déjà implémenté dans `GodotLauncherService.cs` !

L'email est déjà passé via `Intent.PutExtra("user_email", email)` et `Intent.PutExtra("api_base_url", _apiBaseUrl)`.

**Ce qu'il faut faire dans Godot** :

1. **Créer un plugin Android pour Godot** qui lit les Intent extras
2. **OU** modifier le code C# de Godot pour lire directement les Intent extras via JNI

**Exemple de code C# pour Godot** (à ajouter dans `UserEmailManager.cs`) :

```csharp
private void LoadUserEmailFromIntent()
{
    try
    {
        // Obtenir l'activité Android actuelle
        var activity = Godot.Engine.GetSingleton("GodotActivity") as AndroidJavaObject;
        if (activity == null)
        {
            // Essayer d'obtenir l'activité via JNI
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }
        }
        
        if (activity != null)
        {
            var intent = activity.Call<AndroidJavaObject>("getIntent");
            if (intent != null)
            {
                var email = intent.Call<string>("getStringExtra", "user_email");
                var apiUrl = intent.Call<string>("getStringExtra", "api_base_url");
                
                if (!string.IsNullOrEmpty(email))
                {
                    _userEmail = email;
                    _apiBaseUrl = apiUrl ?? "http://192.168.2.169:5000";
                    GD.Print($"[UserEmailManager] Email chargé depuis Intent: {email}");
                    return;
                }
            }
        }
    }
    catch (Exception ex)
    {
        GD.PrintErr($"[UserEmailManager] Erreur lors de la lecture des Intent extras: {ex.Message}");
    }
}
```

### Solution 2 : Créer un ContentProvider Android

Créer un ContentProvider dans l'app MAUI qui expose le fichier de configuration, et le lire depuis Godot.

**Complexité** : Moyenne à élevée

### Solution 3 : Utiliser un fichier dans le répertoire externe partagé

Créer le fichier dans un emplacement vraiment accessible (nécessite des permissions spéciales).

**Complexité** : Moyenne

### Solution 4 : Utiliser SharedPreferences (via plugin)

Créer un plugin Android qui utilise SharedPreferences pour partager les données.

**Complexité** : Moyenne

## Solution temporaire (pour tester)

Le script `copier_config_vers_godot.ps1` peut être utilisé pour tester, mais **n'est pas une solution pour l'utilisateur final**.

**Pour l'utilisateur final** : La solution doit être automatique via les Intent extras.

## Recommandation

**Utiliser les Intent extras** car :
- ✅ Déjà implémenté dans l'app MAUI
- ✅ Pas besoin de fichiers partagés
- ✅ Fonctionne immédiatement au lancement
- ✅ Pas de problèmes de permissions

Il faut juste modifier le code Godot pour lire les Intent extras au lieu de chercher un fichier.

