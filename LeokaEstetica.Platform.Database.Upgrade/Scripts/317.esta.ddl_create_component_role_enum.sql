DO
$$
BEGIN
        IF NOT EXISTS(SELECT 1 FROM pg_type WHERE typname = 'component_role_enum') THEN
CREATE TYPE roles.component_role_enum AS ENUM ('job_seeker', 'owner', 'recruitment_or_outsourcing_agency');
END IF;
END
$$;