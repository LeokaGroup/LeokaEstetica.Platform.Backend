CREATE TABLE dbo."Header"
(
    "HeaderId"      SERIAL
        CONSTRAINT "PK_Header_HeaderId"
            PRIMARY KEY,
    "MenuItemTitle" VARCHAR(200)      NOT NULL,
    "MenuItemUrl"   VARCHAR(200),
    "Position"      INTEGER DEFAULT 0 NOT NULL,
    "HeaderType"    VARCHAR(100)      NOT NULL
);

INSERT INTO dbo."Header" ("MenuItemTitle", "MenuItemUrl", "Position", "HeaderType")
VALUES ('Управление проектами', '/project-managment', 4, 'Main');