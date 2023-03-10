INSERT INTO tenmo_user(username, password_hash, salt)
VALUES ('duck', 'mooooooooo', 'password5000')

SELECT * FROM tenmo_user



transfer_type_id int NOT NULL,
	transfer_status_id int NOT NULL,
	account_from int NOT NULL,
	account_to int NOT NULL,
	amount decimal(13, 2) NOT NULL


	INSERT INTO transfer(transfer_type_id, transfer_status_id, account_from, account_to, amount)
	VALUES('1', '2', '2001', '2004', '5000')

	SELECT * FROM transfer

	account_id int IDENTITY(2001,1) NOT NULL,
	user_id int NOT NULL,
	balance decimal(13, 2) NOT NULL,

	INSERT INTO account(user_id ,balance)
	VALUES('1007', '4975')

	SELECT * FROM account

	SELECT balance FROM account
	SELECT balance FROM account WHERE user_id = '1001';

	"INSERT INTO transfer (account_from, account_to, amount, transfer_type_id, transfer_status_id) " +
                                                    "OUTPUT INSERTED.transfer_id " +
                                                    "VALUES (@account_from, @account_to, @amount, @transfer_type_id, @transfer_status_id);", conn);

INSERT INTO transfer(account_from, account_to, amount, transfer_type_id, transfer_status_id)
VALUES(2001, 2004, 150, 2, 2)


List all Test
SELECT * FROM transfer JOIN account ON transfer.account_to = account.user_id WHERE user_id = 1001
UNION
SELECT * FROM transfer JOIN account ON transfer.account_from = account.account_id WHERE user_id = 1001

GetTransferById test
SELECT * FROM transfer JOIN account ON transfer.account_to = account.account_id WHERE transfer_id = 3005 AND user_id = 1006
UNION
SELECT * FROM transfer JOIN account ON transfer.account_from = account.account_id WHERE transfer_id = 3005 AND user_id = 1001;


USER ID question. //Only need one
3005, acc 2001 to 2004
user id 1001 to 1006

UPDATE transfer SET transfer_status_id = 3 WHERE transfer_id = 3004;

UPDATE transfer_status JOIN transfer ON transfer_status.transfer_status_id = transfer.transfer_status_id SET transfer_status_desc = 1
WHERE transfer_status_id = 2