CREATE TABLE "Projects"."ProjectStatuses"
(
    "StatusId"             SERIAL
        CONSTRAINT "PK_ProjectStatuses_StatusId"
            PRIMARY KEY,
    "ProjectId"            BIGINT       NOT NULL
        CONSTRAINT "FK_UserProjects_ProjectId"
            REFERENCES "Projects"."UserProjects",
    "ProjectStatusSysName" VARCHAR(100) NOT NULL,
    "ProjectStatusName"    VARCHAR(100) NOT NULL
);