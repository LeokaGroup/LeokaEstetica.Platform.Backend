ALTER TABLE documents.project_documents
    ADD COLUMN IF NOT EXISTS user_id BIGINT NULL;