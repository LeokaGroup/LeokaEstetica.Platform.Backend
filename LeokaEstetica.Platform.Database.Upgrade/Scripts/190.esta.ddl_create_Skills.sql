CREATE TABLE "Profile"."Skills"
(
    "SkillId"      SERIAL
        CONSTRAINT "PK_Skills_SkillId"
            PRIMARY KEY,
    "SkillName"    VARCHAR(200) NOT NULL,
    "SkillSysName" VARCHAR(200) NOT NULL,
    "Position"     INTEGER      NOT NULL,
    "Tag"          VARCHAR(200) NOT NULL
);