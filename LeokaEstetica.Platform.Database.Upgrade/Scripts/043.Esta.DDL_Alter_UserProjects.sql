ALTER TABLE IF EXISTS "Projects"."UserProjects"
    ADD COLUMN IF NOT EXISTS "TemplateId" INT NULL;
COMMENT ON COLUMN "Projects"."UserProjects"."TemplateId" IS 'Id шаблона, который использует проект.';