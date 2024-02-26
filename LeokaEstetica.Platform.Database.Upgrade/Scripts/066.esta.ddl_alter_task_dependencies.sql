ALTER TABLE IF EXISTS project_management."TaskDependencies"
    RENAME TO task_dependencies;

ALTER TABLE IF EXISTS project_management.task_dependencies
    RENAME COLUMN "DependencyId" TO dependency_id;

ALTER TABLE IF EXISTS project_management.task_dependencies
    RENAME COLUMN "TaskId" TO task_id;

ALTER TABLE IF EXISTS project_management.task_dependencies
    RENAME COLUMN "DependencySysType" TO dependency_sys_type;

ALTER TABLE IF EXISTS project_management.task_dependencies
    RENAME COLUMN "DependencyTypeName" TO dependency_type_name;

ALTER TABLE IF EXISTS project_management.task_dependencies
    RENAME COLUMN "Position" TO position;