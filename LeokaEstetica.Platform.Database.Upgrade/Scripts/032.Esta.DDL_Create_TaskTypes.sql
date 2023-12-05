CREATE TABLE IF NOT EXISTS "ProjectManagment"."TaskTypes"
(
    "TypeId"      SERIAL          NOT NULL,
    "TypeName"    VARCHAR(150) NOT NULL,
    "TypeSysName" VARCHAR(150) NOT NULL,
    "Position"    INT          NOT NULL DEFAULT 0,
    CONSTRAINT "PK_TaskTypes_TypeId" PRIMARY KEY ("TypeId")
);

COMMENT ON TABLE "ProjectManagment"."TaskTypes" IS 'Таблица типов задач.';
COMMENT ON COLUMN "ProjectManagment"."TaskTypes"."TypeId" IS 'PK.';
COMMENT ON COLUMN "ProjectManagment"."TaskTypes"."TypeName" IS 'Название типа.';
COMMENT ON COLUMN "ProjectManagment"."TaskTypes"."TypeSysName" IS 'Системное название типа.';
COMMENT ON COLUMN "ProjectManagment"."TaskTypes"."Position" IS 'Порядковый номер.';