ALTER TABLE IF EXISTS "Templates"."ProjectManagmentTaskStatusTemplates"
    ADD COLUMN IF NOT EXISTS "TaskStatusId" INT NOT NULL DEFAULT 0;