CREATE TABLE "Communications"."Contacts"
(
    "ContactId"   SMALLSERIAL
        CONSTRAINT "PK_Contacts_ContactId"
            PRIMARY KEY,
    "Name"        VARCHAR(200) NOT NULL,
    "Description" TEXT         NOT NULL
);