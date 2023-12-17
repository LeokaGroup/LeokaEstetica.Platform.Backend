CREATE TABLE IF NOT EXISTS "Templates"."ProjectManagmentTaskStatusIntermediateTemplates"
(
    "StatusId"   SERIAL NOT NULL,
    "TemplateId" SERIAL NOT NULL,
    CONSTRAINT "PK_ProjectManagmentTaskStatusTemplates_StatusId_TemplateId" PRIMARY KEY ("StatusId", "TemplateId")
);