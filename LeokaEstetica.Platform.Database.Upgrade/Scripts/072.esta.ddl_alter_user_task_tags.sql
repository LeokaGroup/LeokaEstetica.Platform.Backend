ALTER TABLE IF EXISTS project_management."UserTaskTags"
    RENAME TO user_task_tags;

ALTER TABLE IF EXISTS project_management.user_task_tags
    RENAME COLUMN "TagId" TO tag_id;

ALTER TABLE IF EXISTS project_management.user_task_tags
    RENAME COLUMN "TagName" TO tag_name;

ALTER TABLE IF EXISTS project_management.user_task_tags
    RENAME COLUMN "TagSysName" TO tag_sys_name;

ALTER TABLE IF EXISTS project_management.user_task_tags
    RENAME COLUMN "Position" TO position;

ALTER TABLE IF EXISTS project_management.user_task_tags
    RENAME COLUMN "UserId" TO user_id;

ALTER TABLE IF EXISTS project_management.user_task_tags
    RENAME COLUMN "TagDescription" TO tag_description;