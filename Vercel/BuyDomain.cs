using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Vercel
{
    public class BuyDomain : CodeActivity{
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> Token { get; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> DomainName { get;}

        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> ExpectedPrice { get; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<Boolean> Renew { get; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> TeamId { get;  }

        [Category("Output")]
        public OutArgument<string> Response { get; set; }

        protected override async void Execute(CodeActivityContext context)
        {
            var token = Token.Get(context);
            var domainName = DomainName.Get(context);
            var teamId = TeamId.Get(context);
            var ExpectedPrice = TeamId.Get(context);
            var renew = TeamId.Get(context);
            if (string.IsNullOrEmpty(token)) throw new InvalidOperationException("Token not provided in VercelScope.");

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.vercel.com/");

                // Set the Authorization header
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer ", token);

                // Set the request URI
                string requestUri = $"v5/domains/{domainName}/buy?teamId={teamId}";
                string payload = JsonConvert.SerializeObject(new
                {
                    username = domainName,
                    password = ExpectedPrice,
                    token = renew.ToString(),
                });
                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(requestUri, content);

                if (response.IsSuccessStatusCode)
                    Response.Set(context, await response.Content.ReadAsStringAsync());
                else
                    // Handle the error or throw an exception
                    throw new HttpRequestException($"Request failed: {response.StatusCode}");
            }
        }   
    }
}
