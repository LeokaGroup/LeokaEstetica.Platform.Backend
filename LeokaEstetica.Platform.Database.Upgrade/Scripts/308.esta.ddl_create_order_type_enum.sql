DO
$$
BEGIN
        IF NOT EXISTS(SELECT 1 FROM pg_type WHERE typname = 'order_type_enum') THEN
CREATE TYPE commerce.ORDER_TYPE_ENUM AS ENUM ('CreateVacancy', 'FareRule', 'OpenResume');
END IF;
END
$$;