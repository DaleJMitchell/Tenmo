USE master
GO

IF DB_ID('tenmo') IS NOT NULL
BEGIN
	ALTER DATABASE tenmo SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
	DROP DATABASE tenmo;
END

CREATE DATABASE tenmo
GO

USE tenmo
GO

CREATE TABLE transfer_type (
	transfer_type_id int IDENTITY(1,1) NOT NULL,
	transfer_type_desc varchar(10) NOT NULL,
	CONSTRAINT PK_transfer_type PRIMARY KEY (transfer_type_id)
)

CREATE TABLE transfer_status (
	transfer_status_id int IDENTITY(1,1) NOT NULL,
	transfer_status_desc varchar(10) NOT NULL,
	CONSTRAINT PK_transfer_status PRIMARY KEY (transfer_status_id)
)

CREATE TABLE tenmo_user (
	user_id int IDENTITY(1001,1) NOT NULL,
	username varchar(50) NOT NULL,
	password_hash varchar(200) NOT NULL,
	salt varchar(200) NOT NULL,
	CONSTRAINT PK_user PRIMARY KEY (user_id),
	CONSTRAINT UQ_username UNIQUE (username)
)

CREATE TABLE account (
	account_id int IDENTITY(2001,1) NOT NULL,
	user_id int NOT NULL,
	balance decimal(13, 2) DEFAULT 1000,
	CONSTRAINT PK_account PRIMARY KEY (account_id),
	CONSTRAINT FK_account_user FOREIGN KEY (user_id) REFERENCES tenmo_user (user_id)
)

CREATE TABLE transfer (
	transfer_id int IDENTITY(3001,1) NOT NULL,
	transfer_type_id int NOT NULL,
	transfer_status_id int NOT NULL,
	account_from int NOT NULL,
	account_to int NOT NULL,
	amount decimal(13, 2) NOT NULL,
	CONSTRAINT PK_transfer PRIMARY KEY (transfer_id),
	CONSTRAINT FK_transfer_account_from FOREIGN KEY (account_from) REFERENCES account (account_id),
	CONSTRAINT FK_transfer_account_to FOREIGN KEY (account_to) REFERENCES account (account_id),
	CONSTRAINT FK_transfer_transfer_status FOREIGN KEY (transfer_status_id) REFERENCES transfer_status (transfer_status_id),
	CONSTRAINT FK_transfer_transfer_type FOREIGN KEY (transfer_type_id) REFERENCES transfer_type (transfer_type_id),
	CONSTRAINT CK_transfer_not_same_account CHECK  ((account_from<>account_to)),
	CONSTRAINT CK_transfer_amount_gt_0 CHECK ((amount>0))
)

DELETE FROM transfer_type
DELETE FROM transfer_status
DELETE FROM transfer
DELETE FROM account
DELETE FROM tenmo_user

INSERT INTO tenmo_user(username, password_hash, salt)
VALUES ('dog', 'doggie', '4(j3Li95'),
('cat', 'kitty', '3456Kf('),
('duck', 'mooooooooo', '47fhgK('),
('rabbit', 'ribbit', '8293hgT('),
('crocodile', 'aaaah', '8d38dsE')

SELECT * FROM tenmo_user

INSERT INTO account(user_id, balance)
VALUES ('1001', '500'),
('1002', '999999999'),
('1003', '2500'),
('1004', '7549'),
('1005', '3000')

SELECT * FROM account
	
INSERT INTO transfer_status (transfer_status_desc) VALUES ('Pending');
INSERT INTO transfer_status (transfer_status_desc) VALUES ('Approved');
INSERT INTO transfer_status (transfer_status_desc) VALUES ('Rejected');

INSERT INTO transfer_type (transfer_type_desc) VALUES ('Request');
INSERT INTO transfer_type (transfer_type_desc) VALUES ('Send');

INSERT INTO transfer(transfer_type_id, transfer_status_id, account_from, account_to, amount)
VALUES('1', '2', '2001', '2002', '150'),
('1', '1', '2002', '2003', '200'),
('2', '1', '2003', '2004', '350'),
('2', '2', '2004', '2005', '200'),
('2', '3', '2005', '2001', '250')

SELECT * FROM transfer
