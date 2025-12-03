# Guide : Exécuter la Migration Username dans Supabase

## Problème

L'erreur suivante apparaît dans les logs de l'API :
```
Could not find the 'username' column of 'game_results' in the schema cache
```

Cela signifie que la colonne `email` n'a pas été renommée en `username` dans la base de données Supabase.

## Solution : Exécuter la Migration SQL

### Étape 1 : Accéder à l'éditeur SQL de Supabase

1. Connectez-vous à votre projet Supabase : https://supabase.com
2. Sélectionnez votre projet
3. Dans le menu de gauche, cliquez sur **"SQL Editor"** (Éditeur SQL)

### Étape 2 : Exécuter la Migration

Copiez et collez le script SQL suivant dans l'éditeur SQL de Supabase :

```sql
-- Migration: Changer la colonne email en username dans game_results
-- Date: 2025-12-02

-- Étape 1: Renommer la colonne email en username
ALTER TABLE game_results 
RENAME COLUMN email TO username;
```

### Étape 3 : Vérifier la Migration

Après avoir exécuté le script, vérifiez que la colonne a bien été renommée :

```sql
-- Vérifier la structure de la table
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_name = 'game_results'
ORDER BY ordinal_position;
```

Vous devriez voir `username` au lieu de `email` dans la liste des colonnes.

### Étape 4 : Rafraîchir le Cache du Schéma (si nécessaire)

Si l'erreur persiste après la migration, Supabase peut avoir mis en cache l'ancien schéma. Pour forcer le rafraîchissement :

1. Dans Supabase, allez dans **Settings** → **API**
2. Cliquez sur **"Reload Schema"** ou **"Refresh Schema"** (si disponible)
3. Redémarrez votre API

**Alternative** : Attendez quelques minutes - Supabase rafraîchit automatiquement le cache du schéma périodiquement.

## Vérification

Après avoir exécuté la migration :

1. Redémarrez votre API (`dotnet run` dans `PlatformGame.Api`)
2. Testez l'envoi d'un score depuis Godot
3. Vérifiez dans Supabase que les données sont bien enregistrées avec la colonne `username`

## Notes

- Les données existantes dans la colonne `email` seront conservées dans la colonne `username`
- Si vous avez des données existantes, elles seront automatiquement migrées
- La migration est réversible (vous pouvez renommer `username` en `email` si nécessaire)

