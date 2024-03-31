CREATE SEQUENCE transition_id_seq MINVALUE 1;
ALTER TABLE templates.project_management_transition_intermediate_templates
    ALTER transition_id SET DEFAULT NEXTVAL('transition_id_seq');