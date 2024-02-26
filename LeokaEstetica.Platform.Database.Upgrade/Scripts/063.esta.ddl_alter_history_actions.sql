ALTER TABLE IF EXISTS project_management."HistoryActions"
    RENAME TO history_actions;

ALTER TABLE IF EXISTS project_management.history_actions
    RENAME COLUMN "ActionId" TO action_id;

ALTER TABLE IF EXISTS project_management.history_actions
    RENAME COLUMN "ActionName" TO action_name;

ALTER TABLE IF EXISTS project_management.history_actions
    RENAME COLUMN "ActionSysName" TO action_sys_name;

ALTER TABLE IF EXISTS project_management.history_actions
    RENAME COLUMN "Position" TO position;