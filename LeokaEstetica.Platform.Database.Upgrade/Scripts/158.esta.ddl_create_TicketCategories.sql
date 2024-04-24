CREATE TABLE "Communications"."TicketCategories"
(
    "CategoryId"      SMALLSERIAL
        CONSTRAINT "PK_TicketCategories_CategoryId"
            PRIMARY KEY,
    "CategoryName"    VARCHAR(150)       NOT NULL,
    "CategorySysName" VARCHAR(150)       NOT NULL,
    "Position"        SMALLINT DEFAULT 0 NOT NULL
);