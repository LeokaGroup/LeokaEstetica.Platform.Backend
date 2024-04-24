ALTER TABLE IF EXISTS "Templates"."ProjectManagmentTaskStatusIntermediateTemplates"
    ADD COLUMN IF NOT EXISTS "IsCustomStatus" BOOLEAN NOT NULL DEFAULT FALSE;