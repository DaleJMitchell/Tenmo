using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class AccountSqlDao : IAccountDao
    {
        public string connectionString;

        public AccountSqlDao(string dbConnectionString) 
        {
            connectionString = dbConnectionString;
        }
        public int GetBalance(int userId)
        {
            int balance = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT balance FROM account WHERE user_id = @user_id", conn);
                    cmd.Parameters.AddWithValue("@user_id", userId);
                    SqlDataReader sdr = cmd.ExecuteReader();

                    if (sdr.Read())
                    {
                        balance = Convert.ToInt32(sdr["balance"]);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return balance;
        }


    }
}
