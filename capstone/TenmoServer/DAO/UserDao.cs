using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class UserDao
    {
        public string connectionString;

        public UserDao(string dbConnectionString) 
        {
            connectionString = dbConnectionString;
        }


        //public User GetUser(string username)
        //{
        //    User user = new User();

        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        conn.Open();
        //        SqlCommand cmd = new SqlCommand("SELECT * FROM tenmo_user WHERE username = @username");
        //        cmd.Parameters.AddWithValue("@username", username);
        //        SqlDataReader sdr = cmd.ExecuteReader();

        //        if (sdr.Read())
        //        {
        //            user = GetUserFromReader(sdr);
        //        }
        //    }
        //    return user;
        //}

        //public User AddUser(string username, string password)
        //{
        //    throw new NotImplementedException();
        //}

        //public List<User> GetUsers()
        //{
        //    throw new NotImplementedException();
        //}

        //public int GetBalance(int userId)
        //{
        //    int balance = 0;

        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        conn.Open();
        //        SqlCommand cmd = new SqlCommand("SELECT balance FROM account WHERE user_id = @user_id");
        //        cmd.Parameters.AddWithValue("@user_id", userId);
        //        SqlDataReader sdr = cmd.ExecuteReader();

        //        if (sdr.Read())
        //        {
        //            balance = Convert.ToInt32(sdr["balance"]);
        //        }
        //    }
        //    return balance;
        //}

        //public User GetUserFromReader(SqlDataReader sdr)
        //{
        //    User user = new User();
        //    user.UserId = Convert.ToInt32(sdr["user_id"]);
        //    user.Username = Convert.ToString(sdr["username"]);
        //    user.PasswordHash = Convert.ToString(sdr["password"]);
        //    user.Salt = Convert.ToString(sdr["salt"]);

        //    return user;
        //}
    }
}
