CREATE UNIQUE INDEX idx_paramkey_paramtype
ON "Configs"."GlobalConfig" ("ParamKey", "ParamType");