CREATE OR REPLACE FUNCTION search.catalog_projects(last_id BIGINT,
                                                   pagination_count_rows SMALLINT)
    RETURNS SETOF RECORD AS
$$
DECLARE
sql TEXT;
BEGIN
    -- Если last_id = NULL, значит получают первую страницу каталога.
    -- Применяем пагинацию для 1 страницы.
    IF (last_id IS NULL) THEN
        sql = 'SELECT prj."ProjectId", ' ||
              'prj."ProjectName", ' ||
              'prj."DateCreated", ' ||
              'prj."ProjectIcon",' ||
              'prj."ProjectDetails",' ||
              'prj."UserId",' ||
              'ProjectStageSysName = (SELECT "StageSysName" ' ||
              'FROM "Projects"."ProjectStages" ' ||
              'WHERE "StageId" = up."") ' ||
              'FROM "Projects"."CatalogProjects" AS cp ' ||
              'INNER JOIN "Projects"."UserProjects" AS up ' ||
              'ON cp."ProjectId" = up."ProjectId" ' ||
              'LEFT JOIN "Projects"."ModerationProjects" AS mp ' ||
              'ON up."ProjectId" = mp."ProjectId" ' ||
              'INNER JOIN subscriptions.user_subscriptions AS us ' ||
              'ON up."UserId" = us.user_id ' ||
              'INNER JOIN subscriptions.all_subscriptions AS asub ' ||
              'ON us.subscription_id = asub.object_id ' ||
              'INNER JOIN "Projects"."UserProjectsStages" AS ups ' ||
              'ON up."ProjectId" = ups."ProjectId" ' ||
              'WHERE up."IsPublic" ' ||
              'AND mp."ModerationStatusId" NOT IN (2, 3) ';

        -- Применяем пагинацию для страницы отталкиваясь от Id последней записи страницы.
    ELSIF (last_id > 0) THEN
        sql = sql || 'AND cp."CatalogProjectId" > ' || last_id;
END IF;

sql = sql || ' LIMIT ' || pagination_count_rows;

RETURN QUERY EXECUTE sql;
END
$$ LANGUAGE plpgsql;

COMMENT ON FUNCTION search.catalog_projects(last_id BIGINT,
    pagination_count_rows SMALLINT) IS 'Функция для каталога проектов (применяет пагинацию)';