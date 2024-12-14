using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Intelligent_Generator
{
    public static class Generator
    {
        public const string ApiKeySite = "https://aistudio.google.com/app/apikey";
        public const string ApiKeyPrefix = "AIzaSy";

        public static string ApiKey;
        private static readonly HttpClient _client = new HttpClient();

        public static async Task<string> GenerateContent(string apiKey, string instruction, string query, sbyte creativity)
        {
            var modelName = "gemini-2.0-flash-exp";
            var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{modelName}:generateContent?key={apiKey}";

            var request = new
            {
                systemInstruction = new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = instruction,
                        }
                    }
                },
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = query
                            }
                        }
                    }
                },
                safetySettings = new[]
                {
                    new
                    {
                        category = "HARM_CATEGORY_DANGEROUS_CONTENT",
                        threshold = "BLOCK_NONE"
                    },
                    new
                    {
                        category = "HARM_CATEGORY_HARASSMENT",
                        threshold = "BLOCK_NONE"
                    },
                    new
                    {
                        category = "HARM_CATEGORY_HATE_SPEECH",
                        threshold = "BLOCK_NONE"
                    },
                    new
                    {
                        category = "HARM_CATEGORY_SEXUALLY_EXPLICIT",
                        threshold = "BLOCK_NONE"
                    }
                },
                generationConfig = new
                {
                    temperature = creativity / 100f,
                    topP = 0.8,
                    topK = 10,
                    responseMimeType = "text/plain"
                }
            };

            var body = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(endpoint, body).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var responseDTO = JsonConvert.DeserializeObject<ApiResponseDto.Response>(responseData);

            return responseDTO.Candidates[0].Content.Parts[0].Text;
        }

        public static bool CanBeGeminiApiKey(string apiKey)
        {
            if (ApiKeyPrefix.StartsWith(apiKey, StringComparison.OrdinalIgnoreCase) || apiKey.StartsWith(ApiKeyPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        public static async Task<bool> IsValidApiKey(string apiKey)
        {
            try
            {
                await GenerateContent(apiKey.Trim(), "You are my friend.", "Say 'Hello World' to me!", 10);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
