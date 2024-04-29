CREATE TABLE dbo."Fon"
(
    "FonId" SERIAL
        PRIMARY KEY,
    "Title" VARCHAR(200) NOT NULL,
    "Text"  TEXT         NOT NULL
);