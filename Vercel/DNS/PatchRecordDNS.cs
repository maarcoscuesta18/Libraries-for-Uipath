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
    internal class PatchRecordDNS : CodeActivity
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

        [Category("Input")]
        [RequiredArgument]
        [Description("Name of the record. For MX and SRV records, this field is the subdomain, e.g. www")]
        public InArgument<string> Name { get; }

        [Category("Input")]
        [RequiredArgument]
        [Description("Type of the record. Valid values are A, AAAA, ALIAS, CAA, CNAME, MX, NS, PTR, SRV, TXT")]
        public InArgument<string> Type { get; }

        [Category("Input")]
        [RequiredArgument]
        [Description("Value of the record. For MX and SRV records, this field is a priority followed by a space and the target, e.g. 10 mail.example.com")]
        public InArgument<string> Value { get; }

        [Category("Input")]
        [RequiredArgument]
        [Description("Priority of the record. Only valid for MX and SRV records.")]
        public InArgument<string> MxPriority { get; }

        [Category("Input")]
        [RequiredArgument]
        [DefaultValue("60")]
        [Description("Time to live in seconds")]
        public InArgument<string> TTL { get; }

        [Category("Input")]
        [RequiredArgument]
        [Description("Comment for the record")]
        public InArgument<string> Comment { get; }

        [Category("Input")]
        [RequiredArgument]
        [Description("SRV target")]
        public InArgument<string> SrvTarget { get; }

        [Category("Input")]
        [RequiredArgument]
        [Description("SRV weight")]
        public InArgument<string> SrvWeight { get; }

        [Category("Input")]
        [RequiredArgument]
        [Description("SRV port")]
        public InArgument<string> SrvPort { get; }

        [Category("Input")]
        [RequiredArgument]
        [Description("SRV priority")]
        public InArgument<string> SrvPriority { get; }



        [Category("Output")]
        public OutArgument<string> Response { get; set; }
        protected override async void Execute(CodeActivityContext context)
        {
            var token = Token.Get(context);
            var teamID = TeamID.Get(context);
            var domain = Domain.Get(context);
            var recordID = RecordID.Get(context);
            var name = Name.Get(context);
            var type = Type.Get(context);
            var value = Value.Get(context);
            var mxpriority = MxPriority.Get(context);
            var ttl = TTL.Get(context);
            var comment = Comment.Get(context);
            var srvtarget = SrvTarget.Get(context);
            var srvweight = SrvWeight.Get(context);
            var srvport = SrvPort.Get(context);
            var srvpriority = SrvPriority.Get(context);

            if (string.IsNullOrEmpty(token)) throw new InvalidOperationException("Token not provided in VercelScope.");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer ", token);

            var srv = new Dictionary<string, string>
            {
                { "target", srvtarget },
                {"weight", srvweight},
                {"port", srvport},
                {"priority", srvpriority }
            };

            var data = new Dictionary<string, string>
            {
                { "name", name },
                { "type", type },
                { "value", value },
                { "mxPriority", mxpriority },
                { "ttl", ttl },
                { "comment", comment },
                { "srv", JsonConvert.SerializeObject(srv) }
            };

            var json = JsonConvert.SerializeObject(data);

            var response = await client.PutAsync($"https://api.vercel.com/v2/domains/{domain}/records/{(teamID != "" ? recordID : recordID+"?teamId="+teamID)}", new StringContent(json, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);
            Response.Set(context, result);

        }
    }
}
