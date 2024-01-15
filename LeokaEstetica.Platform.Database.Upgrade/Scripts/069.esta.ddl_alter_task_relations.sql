ALTER TABLE IF EXISTS project_management."TaskRelations"
    RENAME TO task_relations;

ALTER TABLE IF EXISTS project_management.task_relations
    RENAME COLUMN "RelationId" TO relation_id;

ALTER TABLE IF EXISTS project_management.task_relations
    RENAME COLUMN "RelationType" TO relation_type;

ALTER TABLE IF EXISTS project_management.task_relations
    RENAME COLUMN "TaskId" TO task_id;