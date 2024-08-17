CREATE OR REPLACE FUNCTION search.catalog_main_universal(
    catalog_type search.CATALOG_TYPE_ENUM,
    action_type search.ACTION_TYPE_ENUM,
    action_value RECORD,
    is_multi_value BOOLEAN,
    last_id BIGINT,
    pagination_count_rows SMALLINT)
    RETURNS SETOF RECORD AS
$$
DECLARE
result RECORD;
BEGIN
    -- Если получение каталога и если нет составного признака.
    IF (action_type = 'Init' AND NOT is_multi_value) THEN
        -- Если каталог проектов
        IF (catalog_type = 'Project') THEN
SELECT *
INTO result
FROM search.catalog_projects(last_id, pagination_count_rows);

-- Если каталог вакансий
ELSIF (catalog_type = 'Vacancy') THEN

            -- Если каталог базы резюме
        ELSIF (catalog_type = 'Resume') THEN
END IF;

        -- Если фильтры и если нет составного признака.
    ELSIF (action_type = 'Filter' AND NOT is_multi_value) THEN

        -- Если поиск (составной признак не применяется вместе с поиском).
    ELSIF (action_type = 'Search' AND NOT is_multi_value) THEN
END IF;

RETURN QUERY SELECT * FROM result;
END
$$ LANGUAGE plpgsql;

COMMENT ON FUNCTION search.catalog_main_universal(catalog_type search.CATALOG_TYPE_ENUM,
    action_type search.ACTION_TYPE_ENUM,
    action_value RECORD,
    is_multi_value BOOLEAN,
    last_id BIGINT,
    pagination_count_rows SMALLINT) IS 'Универсальная функция для каталогов основного модуля, поиска в них и их фильтров';