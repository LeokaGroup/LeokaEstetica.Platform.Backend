CREATE TABLE IF NOT EXISTS "Configs"."ProjectManagmentProjectSettings"
(
    "ConfigId"         BIGSERIAL,
    "ProjectId"        BIGINT       NOT NULL,
    "UserId"           BIGINT       NOT NULL,
    "ParamKey"         VARCHAR(200) NOT NULL,
    "ParamValue"       VARCHAR(200) NOT NULL,
    "ParamType"        VARCHAR(50)  NOT NULL,
    "ParamDescription" VARCHAR(200) NOT NULL,
    "ParamTag"         VARCHAR(50)  NULL,
    CONSTRAINT "PK_ProjectManagmentProjectSettings_ConfigId" PRIMARY KEY ("ConfigId")
);