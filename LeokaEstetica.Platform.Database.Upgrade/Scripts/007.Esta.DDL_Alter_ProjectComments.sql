ALTER TABLE IF EXISTS "Communications"."ProjectComments"
DROP CONSTRAINT IF EXISTS "FK_ModerationStatuses_StatusId";

ALTER TABLE IF EXISTS "Communications"."ProjectComments"
    ADD COLUMN IF NOT EXISTS "ModerationStatusId" INT NOT NULL,
    ADD CONSTRAINT "FK_ModerationStatuses_StatusId"
        FOREIGN KEY ("ModerationStatusId")
            REFERENCES "Moderation"."ModerationStatuses" ("StatusId");