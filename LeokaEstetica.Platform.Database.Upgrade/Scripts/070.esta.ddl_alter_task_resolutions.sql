ALTER TABLE IF EXISTS project_management."TaskResolutions"
    RENAME TO task_resolutions;

ALTER TABLE IF EXISTS project_management.task_resolutions
    RENAME COLUMN "ResolutionId" TO resolution_id;

ALTER TABLE IF EXISTS project_management.task_resolutions
    RENAME COLUMN "ResolutionName" TO resolution_name;

ALTER TABLE IF EXISTS project_management.task_resolutions
    RENAME COLUMN "ResolutionSysName" TO resolution_sys_name;

ALTER TABLE IF EXISTS project_management.task_resolutions
    RENAME COLUMN "Position" TO position;