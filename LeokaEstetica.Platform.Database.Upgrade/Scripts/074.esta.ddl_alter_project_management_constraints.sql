ALTER TABLE IF EXISTS project_management.header
    RENAME CONSTRAINT "PK_ProjectManagment_Header_HeaderId" TO pk_project_management_header_Header_id;


ALTER TABLE IF EXISTS project_management.history_actions
    RENAME CONSTRAINT "PK_HistoryActions_ActionId" TO pk_history_actions_action_id;


-- ALTER TABLE IF EXISTS project_management.project_tasks
--     RENAME CONSTRAINT "PK_UserTasks_TaskId" TO pk_user_tasks_task_id;

ALTER TABLE IF EXISTS project_management.project_tasks
    RENAME CONSTRAINT "FK_TaskResolutions_ResolutionId" TO fk_task_resolutions_resolution_id;

ALTER TABLE IF EXISTS project_management.project_tasks
    RENAME CONSTRAINT "FK_TaskTypes_TypeId" TO fk_task_types_type_id;

ALTER INDEX project_management."IDX_ProjectTasks_ProjectId_ProjectTaskId"
    RENAME TO idx_project_tasks_project_id_project_task_id;


ALTER TABLE IF EXISTS project_management.task_comments
    RENAME CONSTRAINT "PK_TaskComments_CommentId" TO pk_task_comments_comment_id;

ALTER TABLE IF EXISTS project_management.task_comments
    RENAME CONSTRAINT "FK_UserTasks_TaskId" TO fk_user_tasks_task_id;


ALTER TABLE IF EXISTS project_management.task_dependencies
    RENAME CONSTRAINT "PK_TaskDependencies_DependencyId" TO pk_task_dependencies_dependency_id;

ALTER TABLE IF EXISTS project_management.task_dependencies
    RENAME CONSTRAINT "FK_UserTasks_TaskId" TO fk_user_tasks_task_id;


ALTER TABLE IF EXISTS project_management.task_history
    RENAME CONSTRAINT "PK_TaskHistory_HistoryId" TO pk_task_history_history_id;

ALTER TABLE IF EXISTS project_management.task_history
    RENAME CONSTRAINT "FK_HistoryActions_ActionId" TO fk_history_actions_action_id;

ALTER TABLE IF EXISTS project_management.task_history
    RENAME CONSTRAINT "FK_UserTasks_TaskId" TO fk_user_tasks_task_id;


ALTER TABLE IF EXISTS project_management.task_priorities
    RENAME CONSTRAINT "PK_TaskPriorities_PriorityId" TO pk_task_priorities_priority_id;


ALTER TABLE IF EXISTS project_management.task_relations
    RENAME CONSTRAINT "PK_TaskRelations_RelationId" TO pk_task_relations_relation_id;

ALTER TABLE IF EXISTS project_management.task_relations
    RENAME CONSTRAINT "FK_UserTasks_TaskId" TO fk_user_tasks_task_id;


ALTER TABLE IF EXISTS project_management.task_resolutions
    RENAME CONSTRAINT "PK_TaskResolutions_ResolutionId" TO pk_task_resolutions_resolution_id;


ALTER TABLE IF EXISTS project_management.task_types
    RENAME CONSTRAINT "PK_TaskTypes_TypeId" TO pk_task_types_type_id;


ALTER TABLE IF EXISTS project_management.user_task_tags
    RENAME CONSTRAINT "PK_TaskTags_TagId" TO pk_task_tags_tag_id;


ALTER TABLE IF EXISTS project_management.view_strategies
    RENAME CONSTRAINT "ViewStrategies_StrategyId" TO pk_view_strategies_strategy_id;