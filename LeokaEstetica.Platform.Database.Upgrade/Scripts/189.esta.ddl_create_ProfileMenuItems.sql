CREATE TABLE "Profile"."ProfileMenuItems"
(
    "ProfileMenuItemId" SERIAL
        CONSTRAINT "PK_ProfileMenuItems_ProfileMenuItemId"
            PRIMARY KEY,
    "ProfileMenuItems"  JSONB NOT NULL
);