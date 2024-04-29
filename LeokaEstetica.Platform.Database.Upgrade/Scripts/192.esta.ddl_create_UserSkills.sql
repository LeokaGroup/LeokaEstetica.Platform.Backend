CREATE TABLE "Profile"."UserSkills"
(
    "UserSkillId" BIGSERIAL
        CONSTRAINT "PK_UserSkills_UserSkillId"
            PRIMARY KEY,
    "SkillId"     INTEGER NOT NULL,
    "UserId"      BIGINT  NOT NULL,
    "Position"    INTEGER NOT NULL
);