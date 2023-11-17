CREATE TABLE IF NOT EXISTS "ProjectManagment"."ViewStrategies"
(
    "StrategyId"          SERIAL,
    "ViewStrategyName"    VARCHAR(255) NOT NULL,
    "ViewStrategySysName" VARCHAR(100) NOT NULL,
    "Position"            INT          NOT NULL,
    CONSTRAINT "ViewStrategies_StrategyId" PRIMARY KEY ("StrategyId")
);