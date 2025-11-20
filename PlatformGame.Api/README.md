# PlatformGame.Api

API Backend pour le jeu Platform Game, connectée à Supabase.

## Configuration

1. **Configurer Supabase** :
   - Créer un projet Supabase sur https://supabase.com
   - Exécuter le script SQL `create_table.sql` dans l'éditeur SQL de Supabase
   - Récupérer l'URL du projet et la clé Anon (anon/public key)
   - Mettre à jour `appsettings.json` et `appsettings.Development.json` :
     ```json
     {
       "Supabase": {
         "Url": "https://votre-projet.supabase.co",
         "AnonKey": "votre-clé-anon"
       }
     }
     ```

## Résolution des problèmes d'IDE

Si vous rencontrez des erreurs dans l'IDE (Visual Studio, Rider, etc.) mais que `dotnet build` réussit :

1. **Nettoyer et reconstruire** :
   ```bash
   dotnet clean
   dotnet restore
   dotnet build
   ```

2. **Fermer et rouvrir l'IDE**

3. **Supprimer les dossiers bin/ et obj/** :
   ```bash
   Remove-Item -Recurse -Force PlatformGame.Api/bin, PlatformGame.Api/obj
   dotnet restore
   dotnet build
   ```

4. **Vérifier que le SDK .NET 9.0 est installé** :
   ```bash
   dotnet --version
   ```

## Exécution

```bash
cd PlatformGame.Api
dotnet run
```

L'API sera accessible sur `https://localhost:5001` ou `http://localhost:5000` (selon la configuration).

## Endpoints

- `GET /api/gameresults/results` - Tous les résultats
- `GET /api/gameresults/results/best` - Meilleur temps
- `GET /api/gameresults/results/top/{count}` - Top N résultats
- `POST /api/gameresults/results` - Enregistrer un nouveau temps
- `GET /api/gameresults/stats` - Statistiques

