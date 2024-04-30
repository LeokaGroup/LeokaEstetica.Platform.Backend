DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'project_strategy_enum') THEN
CREATE TYPE settings.project_strategy_enum AS ENUM ('sm', 'kn');
END IF;
END$$;