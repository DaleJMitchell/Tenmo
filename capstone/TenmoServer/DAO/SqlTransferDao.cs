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

        public Transfer SendMoney(Transfer transfer)
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
                    cmd.Parameters.AddWithValue("@transfer_type_id", 2);
                    cmd.Parameters.AddWithValue("@transfer_status_id", 1);
                    int transferId = Convert.ToInt32(cmd.ExecuteScalar());
                    transfer.Id = transferId;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("transfer did not go through");
                }
                bool transferValidity = CheckTransferValidity(transfer);
                if (!transferValidity)
                {
                    transfer = RejectTransfer(transfer);
                    return transfer;
                }
                else
                {
                    transfer = AcceptTransfer(transfer);
                }
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    SqlCommand cmd2 = new SqlCommand("UPDATE account SET balance -= @amount WHERE account_id = @account_from", conn);
                    cmd2.Transaction = transaction;
                    cmd2.Parameters.AddWithValue("@amount", transfer.amounttoTransfer);
                    cmd2.Parameters.AddWithValue("@account_from", transfer.account_From);
                    cmd2.ExecuteNonQuery();

                    SqlCommand cmd3 = new SqlCommand("UPDATE account SET balance += @amount WHERE account_id = @account_to", conn);
                    cmd3.Transaction = transaction;
                    cmd3.Parameters.AddWithValue("@amount", transfer.amounttoTransfer);
                    cmd3.Parameters.AddWithValue("@account_to", transfer.account_To);
                    cmd3.ExecuteNonQuery();

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



        public bool CheckTransferValidity(Transfer transfer)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    //Check both account IDS
                    int numberOfAccounts = 0;

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

                    numberOfAccounts = 0;

                    SqlCommand cmd2 = new SqlCommand("SELECT * FROM account WHERE account_id = @account_to;", conn);
                    cmd2.Parameters.AddWithValue("@account_to", transfer.account_To);

                    sdr = cmd2.ExecuteReader();

                    if (sdr.Read())
                    {
                        numberOfAccounts++;
                    }
                    if (numberOfAccounts == 0)
                    {
                        throw new Exception("Invalid receiving account");
                    }

                    //Check the balance of sending account
                    SqlCommand cmd3 = new SqlCommand("SELECT balance FROM account WHERE account_id = @account_from;", conn);
                    cmd3.Parameters.AddWithValue("@account_from", transfer.account_From);

                    sdr = cmd3.ExecuteReader();

                    if (sdr.Read())
                    {
                        int balance = Convert.ToInt32(sdr["balance"]);
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

        public Transfer RejectTransfer(Transfer transfer)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd4 = new SqlCommand("UPDATE transfer SET transfer_status_id = 3");


                cmd4.ExecuteNonQuery();
            }
            return transfer;

        }

        public Transfer AcceptTransfer(Transfer transfer)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd4 = new SqlCommand("UPDATE transfer SET transfer_status_id = 2");

                cmd4.ExecuteNonQuery();
            }

            return transfer;
        }

        public Transfer Get(int id)
        {
            Transfer transfer = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT transfer_id, transfer_type_id, transfer_status_id" +
                    "account_from, account_to, amount FROM transfer WHERE transfer_id = @transfer_id", conn);
                cmd.Parameters.AddWithValue("@transfer_id", id);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    transfer = CreateTransferFromReader(reader);
                }
            }
            return transfer;
        }

        public List<Transfer> List()
        {
            List<Transfer> list = new List<Transfer>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT transfer_id, transfer_type_id, transfer_status_id" +
                                                "account_from, account_to, amount FROM transfer WHERE transfer_id = @transfer_id", conn);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Transfer transfer = CreateTransferFromReader(reader);
                    list.Add(transfer);
                }
            }
            return list;
        }
        public Transfer MakeRequest(Transfer transfer)
        {
            try
            {
                transfer = new Transfer();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();


                    
                        SqlCommand cmd = new SqlCommand("UPDATE transfer SET transfer_status_id = '1', transfer_type_id = '1')" +
                            "WHERE transfer = @pendingTransfer", conn);
                        cmd.Parameters.AddWithValue("@pendingTransfer", transfer.status_Id);
                        cmd.ExecuteNonQuery();

                    

                }
            }
            catch (Exception ex)
            {


            }

            return transfer;
        }

        public Transfer FullfillRequest(Transfer transfer)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE transfer_status_id JOIN transfer ON transfer_status.transfer_status_id = transfer.transfer_status_id\" +\r\n  " +
                        "SET transfer_status_desc = 'Approved' WHERE transfer.transfer_status_id = @status_id", conn);
                    cmd.Parameters.AddWithValue("@status_id", transfer.status_Id);

                }
            }
            catch
            {
                return null;
            }
            return null;
}

        private Transfer CreateTransferFromReader(SqlDataReader reader)
        {
            Transfer transfer = new Transfer();
            transfer.Id = Convert.ToInt32(reader["transfer_type_id"]);
            transfer.status_Id = Convert.ToInt32(reader["transfer_status_id"]);
            transfer.account_From = Convert.ToInt32(reader["account_from"]);
            transfer.account_To = Convert.ToInt32(reader["account_to"]);
            transfer.amounttoTransfer = Convert.ToDecimal(reader["amount"]);

            return transfer;
        }


    }
}
