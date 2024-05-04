ALTER TABLE project_management.sprints
    ADD COLUMN executor_id BIGINT   NULL,
    ADD COLUMN watcher_ids BIGINT[] NULL,
    ADD COLUMN created_by  BIGINT   NULL;

ALTER TABLE project_management.sprints
    ADD CONSTRAINT fk_created_by_users_user_id FOREIGN KEY (created_by) REFERENCES dbo."Users" ("UserId"),
    ADD CONSTRAINT fk_users_user_id_created_by FOREIGN KEY (created_by) REFERENCES dbo."Users" ("UserId"),
    ADD CONSTRAINT fk_users_user_id_executor_id FOREIGN KEY (executor_id) REFERENCES dbo."Users" ("UserId")