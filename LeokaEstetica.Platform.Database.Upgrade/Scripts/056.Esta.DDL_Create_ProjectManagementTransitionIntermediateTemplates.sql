CREATE TABLE IF NOT EXISTS "Templates"."ProjectManagementTransitionIntermediateTemplates"
(
    "TransitionId" BIGINT NOT NULL,
    "FromStatusId" BIGINT NOT NULL,
    "ToStatusId"   BIGINT NOT NULL,
    CONSTRAINT "PK_ProjectManagementTransitionIntermediateTemplates_TransitionId_FromStatusId_ToStatusId"
        PRIMARY KEY ("TransitionId", "FromStatusId", "ToStatusId")
);

COMMENT ON TABLE "Templates"."ProjectManagementTransitionIntermediateTemplates" IS 'Таблица связей многие-многие переходов статусов шаблонов пользователя.';
COMMENT ON COLUMN "Templates"."ProjectManagementTransitionIntermediateTemplates"."TransitionId" IS 'PK.';
COMMENT ON COLUMN "Templates"."ProjectManagementTransitionIntermediateTemplates"."FromStatusId" IS 'PK.';
COMMENT ON COLUMN "Templates"."ProjectManagementTransitionIntermediateTemplates"."ToStatusId" IS 'PK.';