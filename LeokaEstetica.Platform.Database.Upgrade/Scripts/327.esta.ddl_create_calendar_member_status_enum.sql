DO
$$
BEGIN
        IF NOT EXISTS(SELECT 1 FROM pg_type WHERE typname = 'calendar_member_status_enum') THEN
CREATE TYPE project_management_human_resources.CALENDAR_MEMBER_STATUS_ENUM AS ENUM ('busy', 'may-be-busy', 'available');
END IF;
END
$$;