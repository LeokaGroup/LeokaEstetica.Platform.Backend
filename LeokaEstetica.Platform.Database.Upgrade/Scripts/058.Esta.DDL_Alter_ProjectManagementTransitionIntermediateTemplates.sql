ALTER TABLE IF EXISTS "Templates"."ProjectManagementTransitionIntermediateTemplates"
    ADD COLUMN IF NOT EXISTS "IsCustomTransition" BOOLEAN NOT NULL DEFAULT FALSE;