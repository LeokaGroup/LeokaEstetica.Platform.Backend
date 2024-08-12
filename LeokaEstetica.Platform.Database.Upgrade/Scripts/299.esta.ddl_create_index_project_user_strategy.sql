DO
$do$
BEGIN
        IF EXISTS(SELECT constraint_name
                  FROM information_schema.constraint_column_usage
                  WHERE table_schema = 'settings'
                    AND table_name = 'project_user_strategy'
                    AND constraint_schema = 'settings'
                    AND constraint_name = 'project_user_strategy_project_id_user_id_idx') THEN
DROP INDEX settings.project_user_strategy_project_id_user_id_idx;
END IF;
END
$do$