CREATE TABLE roles.component_roles
(
    component_role_id       BIGSERIAL                 NOT NULL,
    component_role_name     VARCHAR(70)               NOT NULL,
    component_role_sys_name roles.COMPONENT_ROLE_ENUM NOT NULL,
    position                INT                       NOT NULL,
    CONSTRAINT pk_component_roles_component_role_id PRIMARY KEY (component_role_id)
);