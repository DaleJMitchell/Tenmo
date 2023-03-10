DELETE FROM transfer
DELETE FROM tenmo_user
DELETE FROM account


INSERT INTO tenmo_user(username, password_hash, salt)
VALUES ('dog', 'doggie', '4(j3Li95'),
('cat', 'kitty', '3456Kf('),
('duck', 'mooooooooo', '47fhgK('),
('rabbit', 'ribbit', '8293hgT('),
('crocodile', 'aaaah', '8d38dsE')

SELECT * FROM tenmo_user



transfer_type_id int NOT NULL,
	transfer_status_id int NOT NULL,
	account_from int NOT NULL,
	account_to int NOT NULL,
	amount decimal(13, 2) NOT NULL


	INSERT INTO transfer(transfer_type_id, transfer_status_id, account_from, account_to, amount)
	VALUES('1', '2', '2013', '2014', '150'),
	('1', '2', '2014', '2015', '200'),
	('2', '1', '2017', '2016', '350'),
	('1', '2', '2016', '2015', '200'),
	('2', '3', '2017', '2014', '250')

	SELECT * FROM transfer

	account_id int IDENTITY(2001,1) NOT NULL,
	user_id int NOT NULL,
	balance decimal(13, 2) NOT NULL,

	INSERT INTO account(user_id ,balance)
	VALUES(', '500'),
	('1006', '20'),
	('1005', '999999999.00'),
	('1004', '5000'),
	('1001', '1000')

	DELETE FROM account

	INSERT INTO account(user_id, balance)
	VALUES ('1008', '500'),
	('1009', '999999999'),
	('1010', '2500'),
	('1011', '7549'),
	('1012', '3000')


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