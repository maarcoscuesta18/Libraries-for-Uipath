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

namespace Vercel
{
    public class CreateTeam : CodeActivity{
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> Token { get; }

        [Category("Input")]
        [RequiredArgument]
        [Description("The name of the team. Max 256 char")]
        public InArgument<string> Name { get; }

        [Category("Input")]
        [RequiredArgument]
        [Description("The slug of the team. Max 48 char")]
        public InArgument<string> Slug { get; }

        [Category("Output")]
        public OutArgument<string> Response { get; set; }

        protected override async void Execute(CodeActivityContext context)
        {
            var token = Token.Get(context);
            var name = Name.Get(context);
            var slug = Slug.Get(context);
            if (string.IsNullOrEmpty(token)) throw new InvalidOperationException("Token not provided in VercelScope.");

            var client = new HttpClient();
            string payload = JsonConvert.SerializeObject(new
            {
                slug = slug,
                name = name,
            });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsync($"https://api.vercel.com/v1/teams", new StringContent(payload, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            Console.WriteLine(data);
            Response.Set(context, data);
        }
    }
}
