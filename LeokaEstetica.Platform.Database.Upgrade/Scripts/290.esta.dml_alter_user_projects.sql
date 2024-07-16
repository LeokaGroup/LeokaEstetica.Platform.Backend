ALTER TABLE "Projects"."UserProjects"
    ADD COLUMN IF NOT EXISTS "IsPublic" BOOL DEFAULT TRUE;
COMMENT ON COLUMN "Projects"."UserProjects"."IsPublic" IS 'Поле видимости проекта.';