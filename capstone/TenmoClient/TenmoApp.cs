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
                ViewPreviousTransfers(tenmoApiService.UserId);
            }

            if (menuSelection == 3)
            {
                // View your pending requests
                ViewPendingRequests(tenmoApiService.UserId);
            }

            if (menuSelection == 4)
            {
                SendMoney();
                // Send TE bucks

            }

            if (menuSelection == 5)
            {
                RequestTransfer();
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
            console.PrintSuccess(balance);
            console.Pause();
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

        public void ViewPendingRequests(int user_id)
        {
            List<Transfer> transfers;
            try
            {
                transfers = tenmoApiService.GetTransfers(user_id);
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid User.");
                console.Pause();
                return;
            }

            List<Transfer> requests = new List<Transfer>();
            foreach (Transfer transfer in transfers)
            {
                if (transfer.type_Id == 1 && transfer.status_Id == 1)
                {
                    requests.Add(transfer);
                }
            }

            if (requests.Count == 0)
            {
                Console.WriteLine("You don't have any requests");
                console.Pause();
                return;
            }

            List<Transfer> pendingReceiveRequests = new List<Transfer>();
            List<Transfer> pendingSendRequests = new List<Transfer>();

            int myAccountId = GetAccountId();
            
            foreach (Transfer request in requests)
            {
                if (request.account_To == myAccountId)
                {
                    pendingReceiveRequests.Add(request);
                }
            }
            foreach (Transfer request in requests)
            {
                if (request.account_From == myAccountId)
                {
                    pendingSendRequests.Add(request);
                }
            }
            if (pendingReceiveRequests.Count != 0)
            {
                Console.WriteLine("Pending Transfer Requests (To Receive)\n");
                foreach (Transfer request in pendingReceiveRequests)
                {
                    Console.WriteLine("Transfer ID: " + request.Id);
                    Console.WriteLine("Transfer Amount: " + request.amounttoTransfer);
                    Console.WriteLine("Account From: " + request.account_From);
                    Console.WriteLine("Account To: " + request.account_To);
                    Console.WriteLine();
                }
            }
            if (pendingSendRequests.Count != 0)
            {
                Console.WriteLine("Pending Transfer Requests (To Send)\n");
                foreach (Transfer request in pendingSendRequests)
                {
                    Console.WriteLine("Transfer ID: " + request.Id);
                    Console.WriteLine("Transfer Amount: " + request.amounttoTransfer);
                    Console.WriteLine("Account From: " + request.account_From);
                    Console.WriteLine("Account To: " + request.account_To);
                    Console.WriteLine();
                }
            }
            if (pendingSendRequests.Count == 0)
            {
                console.Pause();
                return;
            }

            Console.WriteLine("Type the ID of a transfer you would like to approve or reject or press enter to continue");
            string menuOption = Console.ReadLine();
            if (menuOption == "")
            {
                console.Pause();
                return;
            }
            Transfer requestToFulfill;
            try
            {
                requestToFulfill = tenmoApiService.GetTransfer(tenmoApiService.UserId, Convert.ToInt32(menuOption));
                if (requestToFulfill.account_To == myAccountId)
                {
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
                console.PrintError("Invalid transfer ID");
                console.Pause();
                return;
            }

            Console.WriteLine("Would you like to (1) Approve or (2) Reject the transfer?");
            menuOption = Console.ReadLine();
            try
            {
                if (Convert.ToInt32(menuOption) == 1)
                {
                    tenmoApiService.Fulfill(requestToFulfill);
                    console.PrintSuccess("Transfer Sent");
                }
                else if (Convert.ToInt32(menuOption) == 2)
                {
                    tenmoApiService.Reject(requestToFulfill);
                    Console.WriteLine("Transfer Rejected");
                }
                else
                {
                    console.PrintError("Invalid Menu Choice");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error fulfilling or rejecting request");
            }
            console.Pause();
        }

        public void SendMoney()
        {
            int? accountTo = PrintAccountIdMenu();
            if (accountTo == null)
            {
                console.Pause();
                return;
            }

            Transfer transfer = new Transfer();
            transfer.account_From = GetAccountId();
            transfer.type_Id = 2;
            transfer.status_Id = 1;
            transfer.account_To = Convert.ToInt32(accountTo);
            Console.WriteLine("How much money in whole dollars would you like to send?");

            int transferAmount = 0;
            while (transferAmount <= 0)
            {
                transferAmount = Convert.ToInt32(Console.ReadLine());
                if (transferAmount <= 0)
                {
                    console.PrintError("Invalid transfer amount");
                }
            }
            transfer.amounttoTransfer = transferAmount;

            try
            {
                transfer = tenmoApiService.TransferBalance(transfer);
            }
            catch (Exception e)
            {
                console.PrintError("Transfer failed");
                console.Pause();
                return;
            }

            if (transfer.status_Id == 2)
            {
                console.PrintSuccess("Transfer approved");
            }
            if (transfer.status_Id == 3)
            {
                console.PrintError("Transfer rejected");
            }
            console.Pause();
        }

        public void RequestTransfer()
        {
            int? accountFrom = PrintAccountIdMenu();
            if (accountFrom == null)
            {
                console.Pause();
                return;
            }

            Console.WriteLine("How much money in dollars and cents would you like to request?");
            int transferAmount = 0;
            while (transferAmount <= 0)
            {
                transferAmount = Convert.ToInt32(Console.ReadLine());
                if (transferAmount <= 0)
                {
                    console.PrintError("Invalid transfer amount");
                }
            }

            Transfer transfer = new Transfer();
            transfer.account_To = GetAccountId();
            transfer.type_Id = 1;
            transfer.status_Id = 1;
            transfer.account_From = (int)accountFrom;
            transfer.amounttoTransfer = transferAmount;
            transfer = tenmoApiService.RequestTransfer(transfer);

            if (transfer.status_Id == 1)
            {
                console.PrintSuccess("Requst Sent");
            }
            if (transfer.status_Id == 3)
            {
                console.PrintError("Error on request");
            }
            console.Pause();
        }

        public void ViewPreviousTransfers(int userId)
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

            List<Transfer> previousTransfers = new List<Transfer>();
            foreach (Transfer transfer in transfers)
            {
                if (transfer.type_Id != 1 && transfer.status_Id == 2)
                {
                    previousTransfers.Add(transfer);
                }
            }

            if (previousTransfers.Count == 0)
            {
                Console.WriteLine("You haven't made any transfers");
                console.Pause();
                return;
            }
            Console.WriteLine("List of past transfers:\n");

            foreach (Transfer transfer in previousTransfers)
            {
                Console.WriteLine("Transfer ID: " + transfer.Id);
                Console.WriteLine("Transfer Amount: " + transfer.amounttoTransfer);
                Console.WriteLine("Account From: " + transfer.account_From);
                Console.WriteLine("Account To: " + transfer.account_To);
                Console.WriteLine();
            }
            console.Pause();
        }

        public List<int> ViewAccountIds()
        {
            List<int> accountIds = tenmoApiService.ViewAccountIds();
            List<int> otherAccountIds = new List<int>();

            foreach(int accountId in accountIds)
            {
                if (accountId != GetAccountId())
                {
                    otherAccountIds.Add(accountId);
                }
            }
            return otherAccountIds;
        }

        public int GetAccountId()
        {
            return tenmoApiService.GetAccountId(tenmoApiService.UserId);
        }

        public int? PrintAccountIdMenu()
        {
            Console.WriteLine("List of account IDs:\n");

            List<int> accountIds = ViewAccountIds();
            foreach (int accountId in accountIds)
            {
                Console.WriteLine(accountId);
            }
            Console.WriteLine("\nWhich account ID?");

            string accountChoice = Console.ReadLine();
            if (accountChoice == "")
            {
                console.Pause();
                return null;
            }

            return Convert.ToInt32(accountChoice);
        }
    }
}
