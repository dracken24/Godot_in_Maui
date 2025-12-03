-- Fonction SQL optimisée pour calculer les statistiques directement dans la base de données
-- Cette fonction évite de charger tous les résultats en mémoire
-- Exécutez ce script dans l'éditeur SQL de votre projet Supabase

CREATE OR REPLACE FUNCTION get_game_stats()
RETURNS JSON AS $$
DECLARE
    total_count BIGINT;
    avg_time BIGINT;
    best_record RECORD;
    result JSON;
BEGIN
    -- Compter le total
    SELECT COUNT(*) INTO total_count FROM game_results;
    
    -- Si aucun résultat, retourner des valeurs par défaut
    IF total_count = 0 THEN
        RETURN json_build_object(
            'total_attempts', 0,
            'average_time_ms', 0,
            'best_time_ms', 0,
            'best_result', NULL
        );
    END IF;
    
    -- Calculer la moyenne
    SELECT AVG(completion_time_ms)::BIGINT INTO avg_time FROM game_results;
    
    -- Récupérer le meilleur résultat
    SELECT id, email, completion_time_ms, completion_time_formatted, created_at, platform
    INTO best_record
    FROM game_results
    ORDER BY completion_time_ms ASC
    LIMIT 1;
    
    -- Construire le résultat JSON
    result := json_build_object(
        'total_attempts', total_count,
        'average_time_ms', COALESCE(avg_time, 0),
        'best_time_ms', best_record.completion_time_ms,
        'best_result', json_build_object(
            'id', best_record.id,
            'email', best_record.email,
            'completion_time_ms', best_record.completion_time_ms,
            'completion_time_formatted', best_record.completion_time_formatted,
            'created_at', best_record.created_at,
            'platform', best_record.platform
        )
    );
    
    RETURN result;
END;
$$ LANGUAGE plpgsql;

-- Donner les permissions nécessaires
GRANT EXECUTE ON FUNCTION get_game_stats() TO anon, authenticated;

-- Pour utiliser cette fonction depuis l'API, vous devrez utiliser RPC:
-- var response = await client.Rpc("get_game_stats");

