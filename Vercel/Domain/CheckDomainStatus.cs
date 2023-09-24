using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Activities;

namespace Vercel.Domain
{
    public class CheckDomainStatus : CodeActivity
    {
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> Token { get; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> DomainName { get; set; }

        [Category("Output")]
        public OutArgument<string> Response { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var token = Token.Get(context);
            var domainName = DomainName.Get(context);
            if (string.IsNullOrEmpty(token)) throw new InvalidOperationException("Token not provided in VercelScope.");

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.vercel.com/");

                // Set the Authorization header
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer ", token);

                // Set the request URI
                string requestUri = $"v4/domains/status?name={domainName}";

                HttpResponseMessage response = client.GetAsync(requestUri).Result;
                response.EnsureSuccessStatusCode();
                Response.Set(context, response.Content.ReadAsStringAsync().Result);
            }
        }
    }
}
