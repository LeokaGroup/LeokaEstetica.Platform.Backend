ALTER TABLE IF EXISTS project_management."ViewStrategies"
    RENAME TO view_strategies;

ALTER TABLE IF EXISTS project_management.view_strategies
    RENAME COLUMN "StrategyId" TO strategy_id;

ALTER TABLE IF EXISTS project_management.view_strategies
    RENAME COLUMN "ViewStrategyName" TO view_strategy_name;

ALTER TABLE IF EXISTS project_management.view_strategies
    RENAME COLUMN "ViewStrategySysName" TO view_strategy_sys_name;

ALTER TABLE IF EXISTS project_management.view_strategies
    RENAME COLUMN "Position" TO position;