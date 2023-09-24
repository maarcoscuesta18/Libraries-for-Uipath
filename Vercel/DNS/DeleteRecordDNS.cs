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

namespace Vercel.DNS{
    public class DeleteRecordDNS : CodeActivity
    {
        [Category("Input")]
        [RequiredArgument]
        [Description("Vercel API Token")]
        public InArgument<string> Token { get;  }

        [Category("Input")]
        public InArgument<string> TeamID { get;  }

        [Category("Input")]
        [RequiredArgument]
        [Description("Domain name, e.g. example.com")]
        public InArgument<string> Domain { get;  }

        [Category("Input")]
        [RequiredArgument]
        [Description("ID of the record. For MX and SRV records, this field is the subdomain, e.g. www")]
        public InArgument<string> RecordID { get; }

        [Category("Output")]
        public OutArgument<string> Response { get; set; }


        protected override async void Execute(CodeActivityContext context){
            var token = Token.Get(context);
            var teamID = TeamID.Get(context);
            var domain = Domain.Get(context);
            var recordID = RecordID.Get(context);
            if (string.IsNullOrEmpty(token)) throw new InvalidOperationException("Token not provided in VercelScope.");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer ", token);
            var response = await client.DeleteAsync($"https://api.vercel.com/v2/domains/{domain}/records/{(teamID != "" ? recordID : recordID+"?teamId="+teamID)}");
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            Console.WriteLine(data);
            Response.Set(context, data);
        }
    }
}
