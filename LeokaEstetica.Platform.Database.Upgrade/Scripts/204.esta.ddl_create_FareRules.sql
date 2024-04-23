CREATE TABLE "Rules"."FareRules"
(
    "RuleId"    SERIAL
        CONSTRAINT "PK_FareRules_RuleId"
            PRIMARY KEY,
    "Name"      VARCHAR(150)                                NOT NULL,
    "Label"     VARCHAR(200)                                NOT NULL,
    "Position"  INTEGER    DEFAULT 0                        NOT NULL,
    "Price"     NUMERIC(12, 2)                              NOT NULL,
    "Currency"  VARCHAR(5) DEFAULT 'RUB'::CHARACTER VARYING NOT NULL,
    "IsPopular" BOOLEAN    DEFAULT FALSE                    NOT NULL,
    "IsFree"    BOOLEAN    DEFAULT FALSE                    NOT NULL,
    "PublicId"  UUID       DEFAULT uuid_in(("overlay"(
            "overlay"(MD5((((RANDOM())::TEXT || ':'::TEXT) || (RANDOM())::TEXT)), '4'::TEXT, 13),
            TO_HEX((FLOOR(((RANDOM() * (((11 - 8) + 1))::DOUBLE PRECISION) + (8)::DOUBLE PRECISION)))::INTEGER),
            17))::CSTRING)                                  NOT NULL
);