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
                    SqlCommand cmd = new SqlCommand("INSERT INTO transfer(account_from, account_to, amount " +
                    "OUTPUT INSERTED.transfer_id " +
                    "VALUES (SELECT account_id WHERE account_id = @account_from]) , (SELECT account_id WHERE account_id = @account_to);" ,conn) ;
                    cmd.Parameters.AddWithValue("@account_from", transfer.account_From);

                    
                    transfer = Convert.ToInt32(cmd.ExecuteScalar()) ;
                }
            }
            catch (Exception ex)
            {

            }
            return transfer;
        }

        private Transfer CreateTransferFromReader(SqlDataReader reader)
        {
            Transfer transfer = new Transfer();
            Transfer.Id = Convert.ToInt32(reader["transfer_type_id"]);
            Transfer.status_Id = Convert.ToInt32(reader["transfer_status_id"]);
            Transfer.account_From = Convert.ToInt32(reader["account_from"])
            Transfer.account_To = Convert.ToInt32(reader["account_to"]);
            Transfer.amount = Convert.ToDecimal(reader["amount"]);

            return transfer;
        }

            
    }
}
