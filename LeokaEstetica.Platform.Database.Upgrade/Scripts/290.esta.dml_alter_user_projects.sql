ALTER TABLE "Projects"."UserProjects"
    ADD COLUMN "IsPublic" BOOL DEFAULT TRUE;
COMMENT ON COLUMN "Projects"."UserProjects"."IsPublic" IS 'Поле видимости проекта.';