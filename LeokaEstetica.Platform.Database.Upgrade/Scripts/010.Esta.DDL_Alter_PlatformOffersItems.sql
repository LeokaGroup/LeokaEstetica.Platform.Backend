create schema dbo;

CREATE TABLE dbo."PlatformOffersItems"
(
    "ItemId"   SERIAL
        PRIMARY KEY,
    "ItemText" VARCHAR(400)          NOT NULL,
    "ItemIcon" TEXT,
    "Position" INTEGER               NOT NULL,
    "IsLater"  BOOLEAN DEFAULT FALSE NOT NULL
);

INSERT INTO dbo."PlatformOffersItems" ("ItemText", "ItemIcon", "Position", "IsLater")
VALUES ('Проектировать схемы различных бизнес-процессов и по разным шаблонам под Ваши задачи с удобным импортом из разных форматов в нашу платформу и экспортом в нужные Вам форматы. Наш искусственный интеллект поможет Вам в этом.',
        NULL, 14, TRUE);