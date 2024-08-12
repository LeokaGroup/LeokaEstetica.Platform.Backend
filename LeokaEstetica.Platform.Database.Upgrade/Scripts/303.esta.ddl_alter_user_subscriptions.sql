ALTER TABLE subscriptions.user_subscriptions
    ADD CONSTRAINT fk_users_user_id FOREIGN KEY (user_id) REFERENCES dbo."Users" ("UserId");