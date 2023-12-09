ALTER TABLE IF EXISTS "ProjectManagment"."ProjectTasks"
    ADD COLUMN IF NOT EXISTS "PriorityId" INT NULL;