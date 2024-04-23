CREATE TABLE "Rules"."FareRulesItems"
(
    "RuleItemId" SERIAL
        CONSTRAINT "PK_FareRulesItems_RuleItemId"
            PRIMARY KEY,
    "RuleId"     INTEGER
        CONSTRAINT "FK_FareRules_RuleId"
            REFERENCES "Rules"."FareRules",
    "Name"       VARCHAR(150)          NOT NULL,
    "Label"      VARCHAR(200),
    "Position"   INTEGER DEFAULT 0     NOT NULL,
    "IsLater"    BOOLEAN DEFAULT FALSE NOT NULL
);