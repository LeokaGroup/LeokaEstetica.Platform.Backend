ALTER TABLE IF EXISTS "ProjectManagment"."TaskHistory"
    DROP CONSTRAINT IF EXISTS "FK_HistoryActions_ActionId";

ALTER TABLE IF EXISTS "ProjectManagment"."TaskHistory"
    ADD CONSTRAINT "FK_HistoryActions_ActionId" FOREIGN KEY ("ActionId") REFERENCES "ProjectManagment"."HistoryActions" ("ActionId");