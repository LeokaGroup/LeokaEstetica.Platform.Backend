CREATE TABLE ai.scrum_master_ai_message_versions
(
    version_id     BIGSERIAL,
    created_at     TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    created_by     BIGINT                   NOT NULL,
    version_number VARCHAR(150)             NOT NULL,
    CONSTRAINT pk_scrum_master_ai_message_versions_version_id PRIMARY KEY (version_id),
    CONSTRAINT fk_users_user_id FOREIGN KEY (created_by) REFERENCES dbo."Users" ("UserId")
);