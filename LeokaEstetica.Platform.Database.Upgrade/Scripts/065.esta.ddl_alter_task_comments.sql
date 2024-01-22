ALTER TABLE IF EXISTS project_management."TaskComments"
    RENAME TO task_comments;

ALTER TABLE IF EXISTS project_management.task_comments
    RENAME COLUMN "CommentId" TO comment_id;

ALTER TABLE IF EXISTS project_management.task_comments
    RENAME COLUMN "Comment" TO comment;

ALTER TABLE IF EXISTS project_management.task_comments
    RENAME COLUMN "Created" TO created;

ALTER TABLE IF EXISTS project_management.task_comments
    RENAME COLUMN "Updated" TO updated;

ALTER TABLE IF EXISTS project_management.task_comments
    RENAME COLUMN "TaskId" TO task_id;

ALTER TABLE IF EXISTS project_management.task_comments
    RENAME COLUMN "AuthorId" TO author_id;