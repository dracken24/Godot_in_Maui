-- Script de migration pour remplacer player_name par email dans la table game_results
-- Exécutez ce script dans l'éditeur SQL de votre projet Supabase

-- Étape 1: Ajouter la nouvelle colonne email si elle n'existe pas
ALTER TABLE game_results 
ADD COLUMN IF NOT EXISTS email TEXT;

-- Étape 2: Copier les données de player_name vers email (si vous voulez conserver les données existantes)
-- Note: Cette étape est optionnelle si vous voulez juste renommer la colonne
-- UPDATE game_results SET email = player_name WHERE email IS NULL;

-- Étape 3: Supprimer l'ancienne colonne player_name
ALTER TABLE game_results 
DROP COLUMN IF EXISTS player_name;

-- Vérification: Afficher la structure de la table pour confirmer
-- SELECT column_name, data_type 
-- FROM information_schema.columns 
-- WHERE table_name = 'game_results';

