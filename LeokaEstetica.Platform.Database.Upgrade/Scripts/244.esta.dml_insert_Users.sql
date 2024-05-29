INSERT INTO dbo."Users" ("UserId", "LastName", "FirstName", "SecondName", "Login", "UserIcon", "DateRegister", "Email",
                         "EmailConfirmed",
                         "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnd",
                         "LockoutEnabled",
                         "UserCode", "LockoutEnabledDate", "LockoutEndDate", "ConfirmEmailCode", "IsVkAuth", "VkUserId",
                         "LastAutorization", "IsMarkDeactivate", "DateCreatedMark", "SubscriptionStartDate",
                         "SubscriptionEndDate", "IsShortLastName")
VALUES (-1, 'Scrum Master AI', 'Scrum Master AI', 'Scrum Master AI', 'scrum_master_ai', NULL, NOW(),
        'scrum_master_ai@mail.ru', TRUE, 'scrum_master_ai', '11111111111', TRUE, FALSE, FALSE, FALSE,
        (SELECT uuid_generate_v4()), NULL, NULL, (SELECT uuid_generate_v4()), FALSE, NULL, NOW(), FALSE, NOW(), NULL,
        NULL, FALSE);