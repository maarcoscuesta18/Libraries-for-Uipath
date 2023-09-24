using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Activities;
using Newtonsoft.Json;
namespace Vercel.DNS
{
    internal class GetRecordsDNS : CodeActivity
    {
        [Category("Input")]
        [RequiredArgument]
        [Description("Vercel API Token")]
        public InArgument<string> Token { get;  }

        [Category("Input")]
        [RequiredArgument]
        [Description("Domain name, e.g. example.com")]
        public InArgument<string> Domain { get;  }

        [Category("Output")]
        public OutArgument<string> Response { get; set; }

        protected override async void Execute(CodeActivityContext context)
        {
            var token = Token.Get(context);
            var domain = Domain.Get(context);

            if (string.IsNullOrEmpty(token)) throw new InvalidOperationException("Token not provided in VercelScope.");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer ", token);
            
            var response = await client.GetAsync($"https://api.vercel.com/v2/domains/{domain}/records");
            response.EnsureSuccessStatusCode();
            
            var data = await response.Content.ReadAsStringAsync();
            
            Console.WriteLine(data);
            Response.Set(context, data);

        }
    }
}
