CREATE TABLE IF NOT EXISTS "Templates"."ProjectManagementTransitionTemplates"
(
    "TransitionId"      BIGSERIAL    NOT NULL,
    "TransitionName"    VARCHAR(150) NOT NULL,
    "TransitionSysName" VARCHAR(150) NOT NULL,
    "Position"          INT          NOT NULL DEFAULT 1,
    "FromStatusId"      BIGINT       NOT NULL,
    "ToStatusId"        BIGINT       NOT NULL,
    CONSTRAINT "PK_ProjectManagementTransitionTemplates_TransitionId" PRIMARY KEY ("TransitionId")
);

COMMENT ON TABLE "Templates"."ProjectManagementTransitionTemplates" IS 'Таблица переходов статусов шаблонов.';
COMMENT ON COLUMN "Templates"."ProjectManagementTransitionTemplates"."TransitionId" IS 'PK.';
COMMENT ON COLUMN "Templates"."ProjectManagementTransitionTemplates"."TransitionName" IS 'Название перехода.';
COMMENT ON COLUMN "Templates"."ProjectManagementTransitionTemplates"."TransitionSysName" IS 'Системное название перехода.';
COMMENT ON COLUMN "Templates"."ProjectManagementTransitionTemplates"."Position" IS 'Позиция.';
COMMENT ON COLUMN "Templates"."ProjectManagementTransitionTemplates"."FromStatusId" IS 'Id статуса, из которого переход.';
COMMENT ON COLUMN "Templates"."ProjectManagementTransitionTemplates"."ToStatusId" IS 'Id статуса, в который переход.';