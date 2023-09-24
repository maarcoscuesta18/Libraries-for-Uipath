using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Vercel
{
    public class DeleteProject : CodeActivity
    {
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> Token { get; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> ProjectId { get; }

        [Category("Output")]
        public OutArgument<string> Response { get; set; }

        protected override async void Execute(CodeActivityContext context){
            var token = Token.Get(context);
            var projectID = ProjectId.Get(context);
            if (string.IsNullOrEmpty(token)) throw new InvalidOperationException("Token not provided in VercelScope.");

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.vercel.com/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer ", token);

            string requestUri = $"v9/projects/{projectID}";
              
            HttpResponseMessage response = await client.DeleteAsync(requestUri);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            Response.Set(context, responseBody);
        }
    }
}
