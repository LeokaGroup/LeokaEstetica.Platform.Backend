CREATE TABLE IF NOT EXISTS "ProjectManagment"."TaskPriorities"
(
    "PriorityId"      SERIAL,
    "PriorityName"    VARCHAR(50) NOT NULL,
    "PrioritySysName" VARCHAR(50) NOT NULL,
    "Position"        INT         NOT NULL DEFAULT 0,
    CONSTRAINT "PK_TaskPriorities_PriorityId" PRIMARY KEY ("PriorityId")
);