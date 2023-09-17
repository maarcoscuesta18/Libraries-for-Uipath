using Newtonsoft.Json.Linq;
using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Vercel
{
    [Designer("Your.Designer.Namespace.If.You.Create.Custom.Designer")]
    public class ListAllDomains : CodeActivity
    {
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> Token { get; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> teamId { get; set; }


        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> limit { get; set; }

        [Category("Output")]
        public OutArgument<string> Response { get; set; }

        protected override async void Execute(CodeActivityContext context)
        {
            var token = Token.Get(context);
            var projectName = teamId.Get(context);
            if (string.IsNullOrEmpty(token)) throw new InvalidOperationException("Token not provided in VercelScope.");

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.vercel.com/");

                // Set the Authorization header
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer ", token);

                // Set the request URI
                string requestUri = $"v5/domains?limit={limit}&since=1609499532000&teamId={teamId}&until=1612264332000";

                HttpResponseMessage response = await client.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    Response.Set(context,await response.Content.ReadAsStringAsync());
                }
                else
                {
                    // Handle the error or throw an exception
                    throw new HttpRequestException($"Request failed: {response.StatusCode}");
                }
            }
        }
    }
}
