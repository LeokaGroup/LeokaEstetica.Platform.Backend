ALTER TABLE IF EXISTS "ProjectManagment"."ProjectTasks"
    DROP CONSTRAINT IF EXISTS "FK_TaskStatuses_StatusId",
    DROP CONSTRAINT IF EXISTS "FK_TaskResolutions_ResolutionId",
    DROP CONSTRAINT IF EXISTS "FK_TaskTypes_TypeId";

ALTER TABLE IF EXISTS "ProjectManagment"."ProjectTasks"
    ADD CONSTRAINT "FK_TaskStatuses_StatusId" FOREIGN KEY ("StatusId") REFERENCES "ProjectManagment"."TaskStatuses" ("StatusId"),
    ADD CONSTRAINT "FK_TaskResolutions_ResolutionId" FOREIGN KEY ("ResolutionId") REFERENCES "ProjectManagment"."TaskResolutions" ("ResolutionId"),
    ADD CONSTRAINT "FK_TaskTypes_TypeId" FOREIGN KEY ("TaskTypeId") REFERENCES "ProjectManagment"."TaskTypes" ("TypeId");