ALTER TABLE IF EXISTS project_management."Header"
    RENAME TO header;

ALTER TABLE IF EXISTS project_management.header
    RENAME COLUMN "HeaderId" TO header_id;

ALTER TABLE IF EXISTS project_management.header
    RENAME COLUMN "ItemName" TO item_name;

ALTER TABLE IF EXISTS project_management.header
    RENAME COLUMN "ItemUrl" TO item_url;

ALTER TABLE IF EXISTS project_management.header
    RENAME COLUMN "Position" TO position;

ALTER TABLE IF EXISTS project_management.header
    RENAME COLUMN "HeaderType" TO header_type;

ALTER TABLE IF EXISTS project_management.header
    RENAME COLUMN "Items" TO items;

ALTER TABLE IF EXISTS project_management.header
    RENAME COLUMN "HasItems" TO has_items;

ALTER TABLE IF EXISTS project_management.header
    RENAME COLUMN "IsDisabled" TO is_disabled;

-- ALTER TABLE IF EXISTS project_management.header
--     RENAME COLUMN "ControlType" TO control_type;

-- ALTER TABLE IF EXISTS project_management.header
--     RENAME COLUMN "Destination" TO destination;