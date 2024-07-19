alter table project_management.wiki_tree
    drop constraint fk_user_projects_project_id;

alter table project_management.wiki_tree
    add constraint fk_user_projects_project_id
        foreign key (project_id) references "Projects"."UserProjects"
            on delete cascade;