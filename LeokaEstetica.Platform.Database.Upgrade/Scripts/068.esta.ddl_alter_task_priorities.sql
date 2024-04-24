ALTER TABLE IF EXISTS project_management."TaskPriorities"
    RENAME TO task_priorities;

ALTER TABLE IF EXISTS project_management.task_priorities
    RENAME COLUMN "PriorityId" TO priority_id;

ALTER TABLE IF EXISTS project_management.task_priorities
    RENAME COLUMN "PriorityName" TO priority_name;

ALTER TABLE IF EXISTS project_management.task_priorities
    RENAME COLUMN "PrioritySysName" TO priority_sys_name;

ALTER TABLE IF EXISTS project_management.task_priorities
    RENAME COLUMN "Position" TO position;