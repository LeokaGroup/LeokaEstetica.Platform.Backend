CREATE TABLE IF NOT EXISTS "Templates"."ProjectManagementUserStatuseTemplates"
(
    "StatusId"      BIGSERIAL,
    "StatusName"    VARCHAR(100) NOT NULL,
    "StatusSysName" VARCHAR(100) NOT NULL,
    "Position"      INT          NOT NULL DEFAULT 1,
    "UserId"        BIGINT       NOT NULL,
    "StatusDescription" VARCHAR(150) NULL,
    CONSTRAINT "PK_ProjectManagementUserStatuseTemplates_StatusId" PRIMARY KEY ("StatusId")
);

COMMENT ON TABLE "Templates"."ProjectManagementUserStatuseTemplates" IS 'Таблица кастомных статусов шаблонов пользователей.';
COMMENT ON COLUMN "Templates"."ProjectManagementUserStatuseTemplates"."StatusId" IS 'PK.';
COMMENT ON COLUMN "Templates"."ProjectManagementUserStatuseTemplates"."StatusName" IS 'Название статуса.';
COMMENT ON COLUMN "Templates"."ProjectManagementUserStatuseTemplates"."StatusSysName" IS 'Системное название статуса.';
COMMENT ON COLUMN "Templates"."ProjectManagementUserStatuseTemplates"."Position" IS 'Позиция.';
COMMENT ON COLUMN "Templates"."ProjectManagementUserStatuseTemplates"."UserId" IS 'Id пользователя.';
COMMENT ON COLUMN "Templates"."ProjectManagementUserStatuseTemplates"."StatusDescription" IS 'Описание статуса.';