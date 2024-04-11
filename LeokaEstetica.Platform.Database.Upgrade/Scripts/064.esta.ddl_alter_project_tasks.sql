ALTER TABLE IF EXISTS project_management."ProjectTasks"
    RENAME TO project_tasks;

ALTER TABLE IF EXISTS project_management.project_tasks
    RENAME COLUMN "TaskId" TO task_id;

ALTER TABLE IF EXISTS project_management.project_tasks
    RENAME COLUMN "TaskStatusId" TO task_status_id;

ALTER TABLE IF EXISTS project_management.project_tasks
    RENAME COLUMN "AuthorId" TO author_id;

ALTER TABLE IF EXISTS project_management.project_tasks
    RENAME COLUMN "WatcherIds" TO watcher_ids;

ALTER TABLE IF EXISTS project_management.project_tasks
    RENAME COLUMN "Name" TO name;

ALTER TABLE IF EXISTS project_management.project_tasks
    RENAME COLUMN "Details" TO details;

ALTER TABLE IF EXISTS project_management.project_tasks
    RENAME COLUMN "Created" TO created;

ALTER TABLE IF EXISTS project_management.project_tasks
    RENAME COLUMN "Updated" TO updated;

ALTER TABLE IF EXISTS project_management.project_tasks
    RENAME COLUMN "ProjectId" TO project_id;

ALTER TABLE IF EXISTS project_management.project_tasks
    RENAME COLUMN "ProjectTaskId" TO project_task_id;

ALTER TABLE IF EXISTS project_management.project_tasks
    RENAME COLUMN "ResolutionId" TO resolution_id;

ALTER TABLE IF EXISTS project_management.project_tasks
    RENAME COLUMN "TagIds" TO tag_ids;

ALTER TABLE IF EXISTS project_management.project_tasks
    RENAME COLUMN "TaskTypeId" TO task_type_id;

ALTER TABLE IF EXISTS project_management.project_tasks
    RENAME COLUMN "ExecutorId" TO executor_id;

ALTER TABLE IF EXISTS project_management.project_tasks
    RENAME COLUMN "PriorityId" TO priority_id;