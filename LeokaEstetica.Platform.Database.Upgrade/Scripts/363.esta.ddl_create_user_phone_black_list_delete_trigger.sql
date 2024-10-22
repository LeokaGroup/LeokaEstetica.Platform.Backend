﻿CREATE OR REPLACE FUNCTION access.user_phone_black_list_history_delete()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO access.user_phone_black_list_history (date_created, action_text, action_sys_name, 
													  user_id, phone_number)
    VALUES (now(), 'Пользователь удален из ЧС.', 'Delete', OLD.user_id, OLD.phone_number);
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER user_phone_black_list_history_delete_trigger
	AFTER DELETE 
	ON access.user_phone_black_list
	FOR EACH ROW
	EXECUTE FUNCTION access.user_phone_black_list_history_delete()