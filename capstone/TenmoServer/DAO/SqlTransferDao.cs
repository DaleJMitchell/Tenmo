using System;
using System.Data.SqlClient;
using TenmoServer.Models;
using System.Collections.Generic;

namespace TenmoServer.DAO
{
    public class SqlTransferDao : ITransferDao
    {
        private readonly string connectionString;

        public SqlTransferDao(string connString)
        {
            connectionString = connString;
        }

        public Transfer SendMoney(Transfer transfer)
        {
            try
            {
                transfer = null;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();
                    try
                    {
                        SqlCommand cmd = new SqlCommand("INSERT INTO transfer(account_from, account_to, amount " +
                   "OUTPUT INSERTED.transfer_id " +
                   "VALUES (SELECT account_id WHERE account_id = @account_from]) , (SELECT account_id WHERE account_id = @account_to);", conn);
                        cmd.Transaction = transaction;
                        cmd.Parameters.AddWithValue("@account_from", transfer.account_From);
                        cmd.Parameters.AddWithValue("@account_to", transfer.account_To);
                        cmd.ExecuteNonQuery();
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
                        SqlCommand cmd4 = new SqlCommand("UPDATE transfer_status_id JOIN transfer ON transfer_status.transfer_status_id = transfer.transfer_status_id" +
                            "SET transfer_status_desc = 'Approved' WHERE transfer.transfer_status_id = @status_id");
                        cmd4.Parameters.AddWithValue("@status_id", transfer.status_Id);
                        cmd4.ExecuteNonQuery();
                    }
                    catch(Exception ex)
                    {
                        transaction.Rollback();
                    }
                }
            }
            catch (Exception ex)
            {

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
