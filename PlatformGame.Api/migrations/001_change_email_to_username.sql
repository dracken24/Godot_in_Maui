-- Migration: Changer la colonne email en username dans game_results
-- Date: 2025-12-02

-- Étape 1: Renommer la colonne email en username
ALTER TABLE game_results 
RENAME COLUMN email TO username;

-- Étape 2: (Optionnel) Si vous voulez changer le type ou les contraintes
-- Par exemple, rendre username NOT NULL si nécessaire:
-- ALTER TABLE game_results 
-- ALTER COLUMN username SET NOT NULL;

-- Note: Les données existantes seront conservées, seule la colonne est renommée

