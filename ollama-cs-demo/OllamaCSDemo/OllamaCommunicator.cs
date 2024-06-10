using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Diagnostics;


namespace OllamaCSDemo
{
    class OllamaCommunicator
    {
        private Process ollamaProcess;
        private string model;
        private string systemPrompt;
        private bool bRespondInStream = true;
        private float temperature;
        public OllamaCommunicator(string model, string setupPrompt,bool bRespondInStream = true, float temperature = 0.7f)
        {
            this.model = model;
            this.systemPrompt = setupPrompt;
            this.bRespondInStream = bRespondInStream;
            this.temperature = temperature;

            StartOllamaProcess();
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        }

        ~OllamaCommunicator()
        {
            StopOllamaProcess();
        }

        private void StartOllamaProcess()
        {
            ollamaProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ollama",
                    Arguments = "start",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            ollamaProcess.Start();
        }

        private void StopOllamaProcess()
        {
            if (ollamaProcess != null && !ollamaProcess.HasExited)
            {
                ollamaProcess.Kill();
                ollamaProcess.Dispose();
            }
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            StopOllamaProcess();
        }

        public async IAsyncEnumerable<string> GenerateAnswerAsync(string prompt)
        {
            if (string.IsNullOrEmpty(prompt)) throw new ArgumentException("Prompt cannot be null or empty", nameof(prompt));

            using (HttpClient client = new HttpClient())
            {
                var requestBody = new
                {
                    model = this.model,
                    prompt = prompt,
                    system = this.systemPrompt,
                    stream = this.bRespondInStream,
                    options = new
                    {
                        temperature = this.temperature
                    }
                };
                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Use SendAsync to use HttpCompletionOption
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:11434/api/generate")
                {
                    Content = content
                };

                using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var reader = new StreamReader(stream))
                    {
                        while (!reader.EndOfStream)
                        {
                            var line = await reader.ReadLineAsync();
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                var jsonResponse = JObject.Parse(line);
                                yield return jsonResponse["response"]?.ToString();
                            }
                        }
                    }
                }
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
