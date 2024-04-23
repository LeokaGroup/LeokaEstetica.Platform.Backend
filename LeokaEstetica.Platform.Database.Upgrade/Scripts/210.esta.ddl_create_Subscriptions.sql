CREATE TABLE "Subscriptions"."Subscriptions"
(
    "SubscriptionId"   BIGSERIAL
        CONSTRAINT "PK_Subscriptions_SubscriptionId"
            PRIMARY KEY,
    "ObjectId"         BIGINT                NOT NULL,
    "IsLatter"         BOOLEAN DEFAULT FALSE NOT NULL,
    "SubscriptionType" VARCHAR(100)          NOT NULL
);
