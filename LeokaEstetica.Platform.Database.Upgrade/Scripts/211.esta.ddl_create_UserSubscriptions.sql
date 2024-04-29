CREATE TABLE "Subscriptions"."UserSubscriptions"
(
    "UserSubscriptionId" BIGSERIAL
        CONSTRAINT "PK_UserSubscriptions_UserSubscriptionId"
            PRIMARY KEY,
    "UserId"             BIGINT,
    "IsActive"           BOOLEAN DEFAULT FALSE NOT NULL,
    "MonthCount"         SMALLINT,
    "SubscriptionId"     BIGINT                NOT NULL
);