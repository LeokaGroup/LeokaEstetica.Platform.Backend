CREATE TABLE "Communications"."WishesOffers"
(
    "WisheOfferId"   BIGSERIAL
        CONSTRAINT "PK_WishesOffers_WisheOfferId"
            PRIMARY KEY,
    "ContactEmail"   VARCHAR(200)            NOT NULL,
    "WisheOfferText" TEXT                    NOT NULL,
    "DateCreated"    TIMESTAMP DEFAULT NOW() NOT NULL
);