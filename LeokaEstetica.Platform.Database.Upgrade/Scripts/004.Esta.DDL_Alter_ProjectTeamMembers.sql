ALTER TABLE IF EXISTS "Teams"."ProjectsTeamsMembers"
    ALTER COLUMN "VacancyId"
        DROP NOT NULL;