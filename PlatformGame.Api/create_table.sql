-- Script SQL pour créer la table game_results dans Supabase
-- Exécutez ce script dans l'éditeur SQL de votre projet Supabase

CREATE TABLE IF NOT EXISTS game_results (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    player_name TEXT,
    completion_time_ms BIGINT NOT NULL,
    completion_time_formatted TEXT NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    platform TEXT
);

-- Créer un index sur completion_time_ms pour améliorer les performances des requêtes de tri
CREATE INDEX IF NOT EXISTS idx_game_results_completion_time ON game_results(completion_time_ms);

-- Créer un index sur created_at pour les requêtes par date
CREATE INDEX IF NOT EXISTS idx_game_results_created_at ON game_results(created_at);

-- Activer Row Level Security (RLS) - optionnel mais recommandé
ALTER TABLE game_results ENABLE ROW LEVEL SECURITY;

-- Créer une politique pour permettre la lecture publique (pour l'API)
CREATE POLICY "Allow public read access" ON game_results
    FOR SELECT
    USING (true);

-- Créer une politique pour permettre l'insertion publique (pour le jeu Godot)
CREATE POLICY "Allow public insert" ON game_results
    FOR INSERT
    WITH CHECK (true);

