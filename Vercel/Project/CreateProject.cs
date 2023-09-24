using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Activities;

namespace Vercel
{
    public class CreateProject : CodeActivity
    {
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> Token { get; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> ProjectName { get; }

        [Category("Output")]
        public OutArgument<string> Response { get; set; }
        protected override async void Execute(CodeActivityContext context)
        {
            var token = Token.Get(context);
            var projectName = ProjectName.Get(context);
            if (string.IsNullOrEmpty(token)) throw new InvalidOperationException("Token not provided in VercelScope.");

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.vercel.com/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer ", token);

            string requestUri = $"v9/projects";
            string payload = JsonConvert.SerializeObject(new
            {
                name = "a-project-name",
                buildCommand = "SOME_STRING_VALUE",
                commandForIgnoringBuildStep = "SOME_STRING_VALUE",
                devCommand = "SOME_STRING_VALUE",
                environmentVariables = new[]
                {
                    new
                    {
                        key = "SOME_STRING_VALUE",
                        target = "production",
                        gitBranch = "feature-1",
                        type = "system",
                        value = "SOME_STRING_VALUE"
                    }
                },
                framework = "nextjs",
                gitRepository = new
                {
                    repo = "SOME_STRING_VALUE",
                    type = "github"
                },
                installCommand = "SOME_STRING_VALUE",
                outputDirectory = "SOME_STRING_VALUE",
                publicSource = true,
                rootDirectory = "SOME_STRING_VALUE",
                serverlessFunctionRegion = "SOME_STRING_VALUE",
                skipGitConnectDuringLink = true
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
