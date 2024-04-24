ALTER TABLE IF EXISTS "Templates"."ProjectManagmentTaskStatusTemplates"
    ALTER COLUMN "StatusId" TYPE BIGINT;

CREATE SEQUENCE task_status_id_seq MINVALUE 1;
ALTER TABLE "Templates"."ProjectManagmentTaskStatusTemplates" ALTER "StatusId" SET DEFAULT nextval('task_status_id_seq');