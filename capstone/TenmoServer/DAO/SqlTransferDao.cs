using System;
using System.Data.SqlClient;
using TenmoServer.Models;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;

namespace TenmoServer.DAO
{
    public class SqlTransferDao : ITransferDao
    {
        private readonly string connectionString;

        public SqlTransferDao(string connString)
        {
            connectionString = connString;
        }
        private Transfer transferRequest = new Transfer();

        //#MAIN method. Creating a transfer OR request
        public Transfer CreateTransfer(Transfer transfer)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO transfer (account_from, account_to, amount, transfer_type_id, transfer_status_id) " +
                                                    "OUTPUT INSERTED.transfer_id " +
                                                    "VALUES (@account_from, @account_to, @amount, @transfer_type_id, @transfer_status_id);", conn);
                    cmd.Parameters.AddWithValue("@account_from", transfer.account_From);
                    cmd.Parameters.AddWithValue("@account_to", transfer.account_To);
                    cmd.Parameters.AddWithValue("@amount", transfer.amounttoTransfer);
                    cmd.Parameters.AddWithValue("@transfer_type_id", transfer.type_Id);
                    cmd.Parameters.AddWithValue("@transfer_status_id", 1);
                    int transferId = Convert.ToInt32(cmd.ExecuteScalar());
                    transfer.Id = transferId;
                }
                catch (Exception)
                {
                    Console.WriteLine("transfer did not go through");
                }
            }
            if (transfer.type_Id == 2)
            {
                transfer = FulfillRequest(transfer);
            }
            return transfer;
        }

        //#HELPER Called when attempting to create a new transfer or fulfill a request
        public Transfer AttemptTransaction(Transfer transfer)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    SqlCommand cmd1 = new SqlCommand("UPDATE account SET balance -= @amount WHERE account_id = @account_from", conn);
                    cmd1.Transaction = transaction;
                    cmd1.Parameters.AddWithValue("@amount", transfer.amounttoTransfer);
                    cmd1.Parameters.AddWithValue("@account_from", transfer.account_From);
                    cmd1.ExecuteNonQuery();

                    SqlCommand cmd2 = new SqlCommand("UPDATE account SET balance += @amount WHERE account_id = @account_to", conn);
                    cmd2.Transaction = transaction;
                    cmd2.Parameters.AddWithValue("@amount", transfer.amounttoTransfer);
                    cmd2.Parameters.AddWithValue("@account_to", transfer.account_To);
                    cmd2.ExecuteNonQuery();

                    transaction.Commit();
                    transfer = AcceptTransfer(transfer);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transfer = RejectTransfer(transfer);
                    return transfer;
                }
                return transfer;
            }
        }

        //#HELPER Checks both IDS to see if they are valid and that there is enough money in the FROM account
        public bool CheckTransferValidity(Transfer transfer)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    //Check both account IDS
                    int numberOfAccounts = 0;
                    conn.Open();
                    SqlCommand cmd1 = new SqlCommand("SELECT * FROM account WHERE account_id = @account_from;", conn);
                    cmd1.Parameters.AddWithValue("@account_from", transfer.account_From);

                    SqlDataReader sdr = cmd1.ExecuteReader();

                    if (sdr.Read())
                    {
                        numberOfAccounts++;
                    }
                    if (numberOfAccounts == 0)
                    {
                        throw new Exception("Invalid sending account");
                    }

                }
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    int numberOfAccounts = 0;
                    conn.Open();
                    SqlCommand cmd2 = new SqlCommand("SELECT * FROM account WHERE account_id = @account_to;", conn);
                    cmd2.Parameters.AddWithValue("@account_to", transfer.account_To);

                    SqlDataReader sdr2 = cmd2.ExecuteReader();

                    if (sdr2.Read())
                    {
                        numberOfAccounts++;
                    }
                    if (numberOfAccounts == 0)
                    {
                        throw new Exception("Invalid receiving account");
                    }
                }
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    //Check the balance of sending account
                    SqlCommand cmd3 = new SqlCommand("SELECT balance FROM account WHERE account_id = @account_from;", conn);
                    cmd3.Parameters.AddWithValue("@account_from", transfer.account_From);

                    SqlDataReader sdr3 = cmd3.ExecuteReader();

                    if (sdr3.Read())
                    {
                        int balance = Convert.ToInt32(sdr3["balance"]);
                        if (balance < transfer.amounttoTransfer)
                        {
                            throw new Exception("Insufficient Funds");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        //#HELPER AND MAIN method. Rejects a transfer
        public Transfer RejectTransfer(Transfer transfer)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd4 = new SqlCommand("UPDATE transfer SET transfer_status_id = 3 WHERE transfer_id = @transfer_id;", conn);
                cmd4.Parameters.AddWithValue("@transfer_id", transfer.Id);
                cmd4.ExecuteNonQuery();
            }
            transfer.status_Id = 3;
            return transfer;

        }

        //#HELPER AND MAIN method. Accepts a transfer
        public Transfer AcceptTransfer(Transfer transfer)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd4 = new SqlCommand("UPDATE transfer SET transfer_status_id = 2 WHERE transfer_id = @transfer_id;", conn);
                    cmd4.Parameters.AddWithValue("@transfer_id", transfer.Id);
                    cmd4.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            transfer.status_Id = 2;
            return transfer;
        }

        //#MAIN method. 
        public Transfer GetTransferById(int userId, int transferId)
        {
            Transfer transfer = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM transfer " +
                                                "JOIN account ON transfer.account_to = account.account_id " +
                                                "WHERE transfer_id = @transfer_id AND account.user_id = @user_id " +
                                                "UNION " +
                                                "SELECT * FROM transfer " +
                                                "JOIN account ON transfer.account_from = account.account_id " +
                                                "WHERE transfer_id = @transfer_id AND account.user_id = @user_id ", conn);
                cmd.Parameters.AddWithValue("@transfer_id", transferId);
                cmd.Parameters.AddWithValue("@user_id", userId);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    transfer = CreateTransferFromReader(reader);
                }
            }
            return transfer;
        }

        //#MAIN method. Lists all transfers for a specific user
        public List<Transfer> ListAllTransfers(int userId)
        {
            List<Transfer> list = new List<Transfer>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM transfer " +
                                                "JOIN account ON transfer.account_to = account.account_id " +
                                                "WHERE account.user_id = @user_id " +
                                                "UNION " +
                                                "SELECT * FROM transfer " +
                                                "JOIN account ON transfer.account_from = account.account_id " +
                                                "WHERE account.user_id = @user_id ", conn);
                cmd.Parameters.AddWithValue("@user_id", userId);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Transfer transfer = CreateTransferFromReader(reader);
                    list.Add(transfer);
                }
            }
            return list;
        }

        //#HELPER and MAIN method. Fulfills a requested transfer 
        public Transfer FulfillRequest(Transfer transfer)
        {
            bool transferValidity = CheckTransferValidity(transfer);
            if (!transferValidity)
            {
                transfer = RejectTransfer(transfer);
                return transfer;
            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE transfer SET transfer_status_id = 2, transfer_type_id = 2 WHERE transfer_id = @transfer_id", conn);
                cmd.Parameters.AddWithValue("@transfer_id", transfer.Id);
                cmd.ExecuteNonQuery();
            }
            transfer = AttemptTransaction(transfer);
            return transfer;
        }

        //#HELPER. Creates a transfer object from a database row
        private Transfer CreateTransferFromReader(SqlDataReader reader)
        {
            Transfer transfer = new Transfer();
            transfer.Id = Convert.ToInt32(reader["transfer_id"]);
            transfer.status_Id = Convert.ToInt32(reader["transfer_status_id"]);
            transfer.type_Id = Convert.ToInt32(reader["transfer_type_id"]);
            transfer.account_From = Convert.ToInt32(reader["account_from"]);
            transfer.account_To = Convert.ToInt32(reader["account_to"]);
            transfer.amounttoTransfer = Convert.ToDecimal(reader["amount"]);

            return transfer;
        }
    }
}
