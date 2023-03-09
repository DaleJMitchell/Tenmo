using RestSharp;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using TenmoClient.Models;


namespace TenmoClient.Services
{
    public class TenmoApiService : AuthenticatedApiService
    {
        public readonly string ApiUrl;

        public TenmoApiService(string apiUrl) : base(apiUrl) { }

        // Add methods to call api here...

        public int ViewBalance(int id)
        {
            RestRequest request = new RestRequest($"users/{id}");
            IRestResponse<int> response = client.Get<int>(request);

            if (!response.IsSuccessful)
            {
                throw new HttpRequestException($"Account Balance Could Not Be Located");
            }
            return response.Data;
        }



        public List<User> ViewUsers()
        {
            RestRequest request = new RestRequest("users");
            IRestResponse<List<User>> response = client.Get<List<User>>(request);

            if (!response.IsSuccessful)
            {
                throw new HttpRequestException($"Could not find users");
            }
            return response.Data;
        }

        public Transfer TransferBalance(Transfer transfer)
        {
            RestRequest request = new RestRequest("transfer");
            IRestResponse<Transfer> response = client.Post<Transfer>(request);

            if (!response.IsSuccessful)
            {
                throw new HttpRequestException($"Transfer failed");
            }
            return response.Data;


        }

        public Transfer RequestTransfer(Transfer transfer)
        {
            RestRequest request = new RestRequest("transfer/request");
            IRestResponse<Transfer> response = client.Post<Transfer>(request);

            if (!response.IsSuccessful)
            {
                throw new HttpRequestException($"Request failed");
            }
            return response.Data;

        }

        public Transfer Fulfill(Transfer transfer)
        {
            RestRequest request = new RestRequest("transfer/request/fulfill");
            IRestResponse<Transfer> response = client.Put<Transfer>(request);

            if (!response.IsSuccessful)
            {
                throw new HttpRequestException($"Fulfill failed");
            }
            return response.Data;   

        }

        public Transfer Reject(Transfer transfer)
        {
            RestRequest request = new RestRequest("transfer/request/reject");
            IRestResponse<Transfer> response = client.Put<Transfer>(request);

            if (!response.IsSuccessful)
            {
                throw new HttpRequestException($"Reject failed");
            }
            return response.Data;

        }

        public List<Transfer> GetTransfers(int user_id)
        {
            RestRequest request = new RestRequest($"transfer/{user_id}");
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);

            if (!response.IsSuccessful)
            {
                throw new HttpRequestException($"Failed to get transfers");
            }
            return response.Data;

        }
        public Transfer GetTransfer(int user_id, int transfer_id)
        {
            RestRequest request = new RestRequest($"transfer/{user_id}/{transfer_id}");
            IRestResponse<Transfer> response = client.Get<Transfer>(request);

            if (!response.IsSuccessful)
            {
                throw new HttpRequestException($"Failed to get user transfer");
            }
            return response.Data;

        }

        public List<Transfer> GetRequests(int user_id)
        {
            RestRequest request = new RestRequest($"transfer/request/{user_id}/");
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);

            if (!response.IsSuccessful)
            {
                throw new HttpRequestException($"Failed to get requests");
            }
            return response.Data;

        }





    }
}
