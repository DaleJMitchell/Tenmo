using System;
using System.Collections.Generic;
using TenmoClient.Models;
using TenmoClient.Services;

namespace TenmoClient
{
    public class TenmoApp
    {
        private readonly TenmoConsoleService console = new TenmoConsoleService();
        private readonly TenmoApiService tenmoApiService;
        private ApiUser apiUser;


        public TenmoApp(string apiUrl)
        {
            tenmoApiService = new TenmoApiService(apiUrl);
        }

        public void Run()
        {
            bool keepGoing = true;
            while (keepGoing)
            {
                // The menu changes depending on whether the user is logged in or not
                if (tenmoApiService.IsLoggedIn)
                {
                    keepGoing = RunAuthenticated();
                }
                else // User is not yet logged in
                {
                    keepGoing = RunUnauthenticated();
                }
            }
        }

        private bool RunUnauthenticated()
        {
            console.PrintLoginMenu();
            int menuSelection = console.PromptForInteger("Please choose an option", 0, 3, 1);
            while (true)
            {
                if (menuSelection == 0)
                {
                    return false;   // Exit the main menu loop
                }

                if (menuSelection == 1)
                {
                    // Log in
                    Login();
                    return true;    // Keep the main menu loop going
                }

                if (menuSelection == 2)
                {
                    // Register a new user
                    Register();
                    return true;    // Keep the main menu loop going
                }
                if (menuSelection == 3)
                {
                    // Register a new user
                    ViewUsers();
                    return true;    // Keep the main menu loop going
                }
                console.PrintError("Invalid selection. Please choose an option.");
                console.Pause();
            }
        }

        private bool RunAuthenticated()
        {
            console.PrintMainMenu(tenmoApiService.Username);
            int menuSelection = console.PromptForInteger("Please choose an option", 0, 6);
            if (menuSelection == 0)
            {
                // Exit the loop
                return false;
            }

            if (menuSelection == 1)
            {
                ViewBalance(tenmoApiService.UserId);
            }

            if (menuSelection == 2)
            {
                // View your past transfers
                ViewTransfers(tenmoApiService.UserId);
            }

            if (menuSelection == 3)
            {
                // View your pending requests
                GetRequests(tenmoApiService.UserId);
            }

            if (menuSelection == 4)
            {
                SendMoney();
                // Send TE bucks

            }

            if (menuSelection == 5)
            {
                RequestTransfer(tenmoApiService.Id);
                // Request TE bucks

            }

            if (menuSelection == 6)
            {
                // Log out
                tenmoApiService.Logout();
                console.PrintSuccess("You are now logged out");
            }

            return true;    // Keep the main menu loop going
        }

        private void Login()
        {
            LoginUser loginUser = console.PromptForLogin();
            if (loginUser == null)
            {
                return;
            }

            try
            {
                apiUser = tenmoApiService.Login(loginUser);
                if (apiUser == null)
                {
                    console.PrintError("Login failed.");
                }
                else
                {
                    console.PrintSuccess("You are now logged in");
                }
            }
            catch (Exception)
            {
                console.PrintError("Login failed.");
            }
            console.Pause();
        }



        private void Register()
        {
            LoginUser registerUser = console.PromptForLogin();
            if (registerUser == null)
            {
                return;
            }
            try
            {
                bool isRegistered = tenmoApiService.Register(registerUser);
                if (isRegistered)
                {
                    console.PrintSuccess("Registration was successful. Please log in.");
                }
                else
                {
                    console.PrintError("Registration was unsuccessful.");
                }
            }
            catch (Exception)
            {
                console.PrintError("Registration was unsuccessful.");
            }
            console.Pause();
        }

        public void ViewBalance(int userId)
        {
            int balance = tenmoApiService.ViewBalance(userId);
            Console.WriteLine(balance + "\nPress enter to continue");
            Console.ReadLine();
        }

        public void GetTransfer(int userId, int transfer_id)
        {
            Transfer transfer = tenmoApiService.GetTransfer(userId, transfer_id);
            Console.WriteLine("TransferID: " + transfer.Id);
            Console.WriteLine("From: " + transfer.account_From);
            Console.WriteLine("To: " + transfer.account_To);
            Console.WriteLine("Type: " + transfer.type_Id);
            Console.WriteLine("Status: " + transfer.status_Id);
            Console.WriteLine("Transfer Amount " + transfer.amounttoTransfer);
        }

        public void GetRequests(int user_id)
        {
            List<Transfer> requests;
            try
            {
                requests = tenmoApiService.GetTransfers(user_id);
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid User.");
                Console.WriteLine("Press enter to continue\n");
                Console.ReadLine();
                return;
            }

            if (requests.Count == 0)
            {
                Console.WriteLine("You don't have any requests");
                Console.WriteLine("Press enter to continue\n");
                Console.ReadLine();
                return;
            }


            foreach (Transfer transfer in requests)
            {
                if (transfer.type_Id == 1)
                {
                    continue;
                }
                Console.WriteLine("Transfer ID: " + transfer.Id);
                Console.WriteLine("Transfer Amount: " + transfer.amounttoTransfer);
                Console.WriteLine("Account From: " + transfer.account_From);
                Console.WriteLine("Account To: " + transfer.account_To);
                Console.WriteLine();
            }
            Console.WriteLine("Press enter to continue\n");
            Console.ReadLine();
        }

        public void SendMoney()
        {
            Transfer transfer = new Transfer();
            transfer.account_From = tenmoApiService.UserId;
            transfer.type_Id = 2;
            transfer.status_Id = 1;
            Console.WriteLine("Please enter user ID that you wish to send to.");
            transfer.account_To = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("How much money in dollars and cents would you like to send?");
            transfer.amounttoTransfer = Convert.ToInt32(Console.ReadLine());
            transfer = tenmoApiService.TransferBalance(transfer);

            if (transfer.status_Id == 2)
            {
                console.PrintSuccess("Transfer approved");
            }
            if(transfer.status_Id == 3)
            {
                console.PrintError("Status rejected");
            }
            console.Pause();

           
            
        }

        public void RequestTransfer(Transfer transfer)
        {
            Console.WriteLine("Please enter user ID that you are requesting money from.");
            Console.ReadLine();
            Console.WriteLine("How much money in dollars and cents would you like to request?");
            Console.ReadLine();
            transfer = tenmoApiService.RequestTransfer(transfer);
            Console.WriteLine("TransferID: " + transfer.Id);
            Console.WriteLine("From: " + transfer.account_From);
            Console.WriteLine("To: " + transfer.account_To);
            Console.WriteLine("Type: " + transfer.type_Id);
            Console.WriteLine("Status: " + transfer.status_Id);
            Console.WriteLine("Transfer Amount " + transfer.amounttoTransfer);
            Console.WriteLine("Press enter to continue\n");
            Console.ReadLine();
        }

        public void Request(Transfer transfer)
        {
            transfer = tenmoApiService.RequestTransfer(transfer);
            Console.WriteLine("TransferID: " + transfer.Id);
            Console.WriteLine("From: " + transfer.account_From);
            Console.WriteLine("To: " + transfer.account_To);
            Console.WriteLine("Type: " + transfer.type_Id);
            Console.WriteLine("Status: " + transfer.status_Id);
            Console.WriteLine("Transfer Amount " + transfer.amounttoTransfer);
        }

        public void ViewTransfers(int userId)
        {
            List<Transfer> transfers;

            try
            {
                transfers = tenmoApiService.GetTransfers(userId);
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid User.");
                Console.WriteLine("Press enter to continue\n");
                Console.ReadLine();
                return;
            }


            if (transfers.Count == 0)
            {
                Console.WriteLine("You haven't made any transfers");
                Console.WriteLine("Press enter to continue\n");
                Console.ReadLine();
                return;
            }
            Console.WriteLine("List of past transfers:\n");

            foreach (Transfer transfer in transfers)
            {
                if (transfer.type_Id == 1 || transfer.status_Id == 1 || transfer.status_Id == 3)
                {
                    continue;
                }
                Console.WriteLine("Transfer ID: " + transfer.Id);
                Console.WriteLine("Transfer Amount: " + transfer.amounttoTransfer);
                Console.WriteLine("Account From: " + transfer.account_From);
                Console.WriteLine("Account To: " + transfer.account_To);
                Console.WriteLine();
            }
            Console.WriteLine("Press enter to continue\n");
            Console.ReadLine();
        }

        public void ViewUsers()
        {
            List<User> users = tenmoApiService.ViewUsers();

            Console.WriteLine("List of Users: \n");

            foreach (User user in users)
            {
                Console.WriteLine(user.Username);
            }

            Console.WriteLine("Press enter to continue.");
            Console.ReadLine();
        }
    }
}
