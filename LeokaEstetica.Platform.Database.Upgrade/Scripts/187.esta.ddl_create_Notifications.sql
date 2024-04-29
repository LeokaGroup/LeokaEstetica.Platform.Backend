CREATE TABLE "Notifications"."Notifications"
(
    "NotificationId"      BIGSERIAL
        CONSTRAINT "PK_Notifications_NotificationId"
            PRIMARY KEY,
    "NotificationName"    VARCHAR(200)            NOT NULL,
    "NotificationSysName" VARCHAR(200)            NOT NULL,
    "IsNeedAccepted"      BOOLEAN                 NOT NULL,
    "Approved"            BOOLEAN,
    "Rejected"            BOOLEAN,
    "ProjectId"           BIGINT,
    "VacancyId"           BIGINT,
    "UserId"              BIGINT                  NOT NULL,
    "NotificationText"    TEXT                    NOT NULL,
    "Created"             TIMESTAMP DEFAULT NOW() NOT NULL,
    "NotificationType"    VARCHAR(100)            NOT NULL,
    "IsShow"              BOOLEAN                 NOT NULL,
    "IsOwner"             BOOLEAN   DEFAULT FALSE NOT NULL
);