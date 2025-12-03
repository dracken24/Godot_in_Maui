// Code C# pour Godot - UserEmailManager.cs
// À ajouter/modifier dans votre projet Godot

using Godot;
using System;
using System.IO;
using System.Text.Json;

public partial class UserEmailManager : Node
{
    private string _userEmail = "";
    private string _apiBaseUrl = "http://192.168.2.169:5000";

    public string UserEmail => _userEmail;
    public string ApiBaseUrl => _apiBaseUrl;

    public override void _Ready()
    {
        GD.Print("[UserEmailManager] _Ready() appelé");
        
        // Ordre de priorité pour charger l'email :
        // 1. Intent extras (le plus fiable)
        // 2. Fichier dans user:// (répertoire de l'app Godot)
        // 3. Fichier dans d'autres emplacements accessibles
        
        if (LoadUserEmailFromIntent())
        {
            GD.Print($"[UserEmailManager] ✓ Email chargé depuis Intent extras: {_userEmail}");
            return;
        }
        
        if (LoadUserEmailFromFile())
        {
            GD.Print($"[UserEmailManager] ✓ Email chargé depuis fichier: {_userEmail}");
            return;
        }
        
        GD.PrintErr("[UserEmailManager] ✗ Aucun email trouvé !");
    }

    /// <summary>
    /// Essaie de charger l'email depuis les Intent extras (Android)
    /// </summary>
    private bool LoadUserEmailFromIntent()
    {
#if GODOT_ANDROID
        try
        {
            // Utiliser JNI pour obtenir l'activité Android
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                if (currentActivity != null)
                {
                    var intent = currentActivity.Call<AndroidJavaObject>("getIntent");
                    if (intent != null)
                    {
                        var email = intent.Call<string>("getStringExtra", "user_email");
                        var apiUrl = intent.Call<string>("getStringExtra", "api_base_url");
                        
                        if (!string.IsNullOrEmpty(email))
                        {
                            _userEmail = email;
                            if (!string.IsNullOrEmpty(apiUrl))
                            {
                                _apiBaseUrl = apiUrl;
                            }
                            return true;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[UserEmailManager] Erreur lors de la lecture des Intent extras: {ex.Message}");
        }
#endif
        return false;
    }

    /// <summary>
    /// Charge l'email depuis un fichier de configuration
    /// Cherche d'abord dans le répertoire externe de Godot (accessible par l'app MAUI)
    /// </summary>
    private bool LoadUserEmailFromFile()
    {
        // Liste des chemins à vérifier (par ordre de priorité)
        var paths = new string[]
        {
            // 1. PRIORITÉ ABSOLUE: Répertoire externe de l'app MAUI (où l'app MAUI crée le fichier)
            // Ce répertoire est accessible par Godot et est créé automatiquement par l'app MAUI
            "/storage/emulated/0/Android/data/com.companyname.platformgame/files/platformgame_config.json",
            
            // 2. Répertoire externe de Godot (si l'app MAUI peut y écrire)
            "/storage/emulated/0/Android/data/com.company.mygodotgame/files/platformgame_config.json",
            
            // 3. Répertoire user:// de Godot (accessible sans permissions, mais l'app MAUI ne peut pas y écrire)
            "user://platformgame_config.json",
            
            // 4. Autres emplacements (fallback)
            "/storage/emulated/0/Download/platformgame_config.json",
            "/sdcard/Download/platformgame_config.json",
            "/storage/emulated/0/Documents/platformgame_config.json",
            "/sdcard/Documents/platformgame_config.json",
        };

        foreach (var path in paths)
        {
            try
            {
                string filePath = path;
                
                // Si c'est un chemin user://, le convertir en chemin absolu
                if (path.StartsWith("user://"))
                {
                    filePath = OS.GetUserDataDir() + "/" + path.Substring(7);
                    GD.Print($"[UserEmailManager] Chemin user:// converti: {filePath}");
                }
                
                // Vérifier si le fichier existe
                if (File.Exists(filePath))
                {
                    GD.Print($"[UserEmailManager] Fichier trouvé: {filePath}");
                    
                    // Lire le fichier
                    var jsonContent = File.ReadAllText(filePath);
                    GD.Print($"[UserEmailManager] Contenu du fichier: {jsonContent}");
                    
                    // Parser le JSON
                    using (JsonDocument doc = JsonDocument.Parse(jsonContent))
                    {
                        var root = doc.RootElement;
                        
                        if (root.TryGetProperty("email", out var emailElement))
                        {
                            _userEmail = emailElement.GetString() ?? "";
                        }
                        
                        if (root.TryGetProperty("apiBaseUrl", out var apiUrlElement))
                        {
                            _apiBaseUrl = apiUrlElement.GetString() ?? _apiBaseUrl;
                        }
                    }
                    
                    if (!string.IsNullOrEmpty(_userEmail))
                    {
                        GD.Print($"[UserEmailManager] ✓ Email chargé: {_userEmail}");
                        GD.Print($"[UserEmailManager] ✓ URL API: {_apiBaseUrl}");
                        return true;
                    }
                }
                else
                {
                    GD.Print($"[UserEmailManager] Fichier non trouvé: {filePath}");
                }
            }
            catch (Exception ex)
            {
                GD.PrintErr($"[UserEmailManager] Erreur lors de la lecture de {path}: {ex.Message}");
            }
        }
        
        return false;
    }
}

// Note: Pour utiliser AndroidJavaClass, vous devez avoir :
// - Le package Godot.Android dans votre projet
// - Ou utiliser la méthode alternative avec JNI direct

#if GODOT_ANDROID
// Classe helper pour JNI Android (si AndroidJavaClass n'est pas disponible)
public static class AndroidJavaClass
{
    // Implémentation simplifiée - à adapter selon votre version de Godot
    // Cette classe peut nécessiter des ajustements selon votre configuration
}
#endif

