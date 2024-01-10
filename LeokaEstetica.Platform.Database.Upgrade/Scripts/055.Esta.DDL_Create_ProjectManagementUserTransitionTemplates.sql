CREATE TABLE IF NOT EXISTS "Templates"."ProjectManagementUserTransitionTemplates"
(
    "TransitionId"      BIGSERIAL    NOT NULL,
    "TransitionName"    VARCHAR(150) NOT NULL,
    "TransitionSysName" VARCHAR(150) NOT NULL,
    "Position"          INT          NOT NULL DEFAULT 1,
    "FromStatusId"      BIGINT       NOT NULL,
    "ToStatusId"        BIGINT       NOT NULL,
    "UserId"            BIGINT       NOT NULL,
    CONSTRAINT "PK_ProjectManagementUserTransitionTemplates_TransitionId" PRIMARY KEY ("TransitionId")
);

COMMENT ON TABLE "Templates"."ProjectManagementUserTransitionTemplates" IS 'Таблица переходов статусов шаблонов пользователя.';
COMMENT ON COLUMN "Templates"."ProjectManagementUserTransitionTemplates"."TransitionId" IS 'PK.';
COMMENT ON COLUMN "Templates"."ProjectManagementUserTransitionTemplates"."TransitionName" IS 'Название перехода.';
COMMENT ON COLUMN "Templates"."ProjectManagementUserTransitionTemplates"."TransitionSysName" IS 'Системное название перехода.';
COMMENT ON COLUMN "Templates"."ProjectManagementUserTransitionTemplates"."Position" IS 'Позиция.';
COMMENT ON COLUMN "Templates"."ProjectManagementUserTransitionTemplates"."FromStatusId" IS 'Id статуса, из которого переход.';
COMMENT ON COLUMN "Templates"."ProjectManagementUserTransitionTemplates"."ToStatusId" IS 'Id статуса, в который переход.';
COMMENT ON COLUMN "Templates"."ProjectManagementUserTransitionTemplates"."UserId" IS 'Id пользователя.';