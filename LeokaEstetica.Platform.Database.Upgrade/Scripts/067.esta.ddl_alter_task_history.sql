ALTER TABLE IF EXISTS project_management."TaskHistory"
    RENAME TO task_history;

ALTER TABLE IF EXISTS project_management.task_history
    RENAME COLUMN "HistoryId" TO history_id;

ALTER TABLE IF EXISTS project_management.task_history
    RENAME COLUMN "ActionId" TO action_id;

ALTER TABLE IF EXISTS project_management.task_history
    RENAME COLUMN "Created" TO created;

ALTER TABLE IF EXISTS project_management.task_history
    RENAME COLUMN "Updated" TO updated;

ALTER TABLE IF EXISTS project_management.task_history
    RENAME COLUMN "TaskId" TO task_id;