ALTER TABLE IF EXISTS project_management."TaskTypes"
    RENAME TO task_types;

ALTER TABLE IF EXISTS project_management.task_types
    RENAME COLUMN "TypeId" TO type_id;

ALTER TABLE IF EXISTS project_management.task_types
    RENAME COLUMN "TypeName" TO type_name;

ALTER TABLE IF EXISTS project_management.task_types
    RENAME COLUMN "TypeSysName" TO type_sys_name;

ALTER TABLE IF EXISTS project_management.task_types
    RENAME COLUMN "Position" TO position;