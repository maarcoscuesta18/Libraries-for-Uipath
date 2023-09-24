using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Activities;
namespace Vercel.Project
{
    public class ListAllProjects : CodeActivity{
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> Token { get; }

        [Category("Output")]
        public OutArgument<string> Response { get; set; }
        protected override async void Execute(CodeActivityContext context){
            var token = Token.Get(context);
            if (string.IsNullOrEmpty(token)) throw new InvalidOperationException("Token not provided in VercelScope.");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer ", token);
            var response = await client.GetAsync("https://api.vercel.com/v4/projects");
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            Console.WriteLine(data);
            Response.Set(context, data);
        }
    }
}
