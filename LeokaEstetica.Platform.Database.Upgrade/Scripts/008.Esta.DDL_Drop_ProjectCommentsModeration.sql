ALTER TABLE IF EXISTS "Moderation"."ProjectCommentsModeration"
DROP CONSTRAINT IF EXISTS "FK_ProjectComments_StatusId";

ALTER TABLE IF EXISTS "Moderation"."ProjectCommentsModeration"
    DROP COLUMN IF EXISTS "StatusId";