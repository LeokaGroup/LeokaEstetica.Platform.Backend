alter table project_management.organization_projects
    drop constraint fk_user_projects_project_id;

alter table project_management.organization_projects
    add constraint fk_user_projects_project_id
        foreign key (project_id) references "Projects"."UserProjects"
            on delete cascade;