using RestSharp;
using System.Collections.Generic;
using System.Net.Http;
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

        

    }
}
