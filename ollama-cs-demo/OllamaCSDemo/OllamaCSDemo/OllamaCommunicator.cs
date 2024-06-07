using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OllamaCSDemo
{
    class OllamaCommunicator
    {
        private string model;
        private string setupPrompt;

        public OllamaCommunicator(string model, string setupPrompt)
        {
            this.model = model;
            this.setupPrompt = setupPrompt;
        }

        public async Task<string> GenerateCompletionAsync(string model, string prompt)
        {
            using (HttpClient client = new HttpClient())
            {
                var requestBody = new
                {
                    model = model,
                    prompt = prompt
                };
                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("http://localhost:11434/api/generate", content);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                // combine multiple responses into one
                var responses = responseBody.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                var finalResponse = new StringBuilder();

                foreach (var res in responses)
                {
                    var jsonResponse = JObject.Parse(res);
                    finalResponse.Append(jsonResponse["response"].ToString());
                }


                return finalResponse.ToString();
            }
        }

        public async Task<string> ListLocalModelsAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync("http://localhost:11434/api/list");
                response.EnsureSuccessStatusCode();
                
                string responseBody = await response.Content.ReadAsStringAsync();

                return responseBody;
            }
        }
    }
}
