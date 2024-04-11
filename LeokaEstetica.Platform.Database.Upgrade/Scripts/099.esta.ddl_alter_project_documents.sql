ALTER TABLE documents.project_documents
    ALTER COLUMN updated
        DROP NOT NULL;

ALTER TABLE documents.project_documents
    ALTER COLUMN task_id
        DROP NOT NULL;