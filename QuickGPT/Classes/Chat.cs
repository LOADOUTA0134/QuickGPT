using System.IO;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuickGPT.Classes;

namespace QuickGPT.Logic
{
    internal class Chat
    {
        private readonly ChatWindow chatWindow;
        private readonly Settings settings;

        private readonly HttpClient httpClient;
        private readonly List<Dictionary<string, string>> messages;

        /**
         * Constructor is called by chat window when new chat is being opened
         * Creates http client with headers & message history object
         */
        public Chat(ChatWindow chatWindow)
        {
            this.chatWindow = chatWindow;
            settings = SettingsManager.GetSettings();

            // Setup httpClient with headers
            httpClient = new();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {settings.OPENAI_API_KEY}");

            // Setup messages
            messages = [];
            AddMessageToHistory("system", settings.SYSTEM_MESSAGE);
        }

        /**
         * This is the big function that gives the app it's functionality
         * Creates the request, adds user & assistant message to history and calls callback method in chat window
         */
        public async Task StreamResponseAsync(string message)
        {
            // Add message to history
            AddMessageToHistory("user", message);

            // Create request
            var requestBody = new
            {
                model = settings.OPENAI_MODEL,
                messages,
                stream = true
            };

            using HttpRequestMessage request = new(HttpMethod.Post, settings.OPENAI_API_URL)
            {
                Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            };

            // Make request
            using HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);

            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = $"Request failed with status code {response.StatusCode}. Make sure the settings are correct.";
                chatWindow.ErrorCallback(errorMessage);
                return;
            }

            // Get response
            using Stream stream = await response.Content.ReadAsStreamAsync();
            using StreamReader reader = new(stream);

            // Using a task for stream reading so it doesn't take all resources
            await Task.Run(async () =>
            {
                // Read stream until end
                StringBuilder answerStringBuilder = new();
                StringBuilder chunkStringBuilder = new();
                while (!reader.EndOfStream)
                {
                    string? line = await reader.ReadLineAsync();
                    if (!string.IsNullOrWhiteSpace(line) && line.StartsWith("data: "))
                    {
                        string jsonStr = line["data: ".Length..].Trim();
                        if (jsonStr == "[DONE]")
                        {
                            if (chunkStringBuilder.Length > 0)
                            {
                                await chatWindow.StreamResponseCallback(chunkStringBuilder.ToString());
                            }
                            AddMessageToHistory("assistant", answerStringBuilder.ToString());
                            break;
                        }

                        string chunk = GetContentFromJsonStr(jsonStr);
                        if (!string.IsNullOrEmpty(chunk))
                        {
                            chunkStringBuilder.Append(chunk);
                            answerStringBuilder.Append(chunk);
                            if (chunkStringBuilder.Length > settings.UPDATE_INTERVAL)
                            {
                                await chatWindow.StreamResponseCallback(chunkStringBuilder.ToString());
                                chunkStringBuilder = new StringBuilder();
                            }
                        }
                    }
                }
            });
        }

        /**
         * Adds message to history
         */
        private void AddMessageToHistory(string role, string message)
        {
            message = message.Trim();
            messages.Add(
                new()
                {
                    { "role", role },
                    { "content", message }
                }
            );
        }

        /**
         * Gets the content from the openai json string
         * Returns either the chunk or an empty string
         */
        private string GetContentFromJsonStr(string jsonStr)
        {
            try
            {
                JObject json = JObject.Parse(jsonStr);
                JToken? delta = json["choices"]?[0]?["delta"];
                JToken? content = delta?["content"];
                if (delta != null && content != null)
                {
                    string chunk = content.ToString();
                    return chunk;
                }
                return "";
            }
            catch (JsonReaderException)
            {
                return "";
            }
        }
    }
}
