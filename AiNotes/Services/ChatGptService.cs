

using AiNotes.Mappers;
using AiNotes.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Text;

namespace AiNotes.Services
{
    public interface IChatGptService
    {
        Task<string> GetResponseAsync(string transcription);
        void SetNoteTypeInstructions(string noteInstruction);
    }
    public class ChatGptService : IChatGptService
    {
        private readonly AppSettings appSettings;
        private readonly HttpClient _httpClient;
        private string noteTypeInstructions;

        public ChatGptService(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {this.appSettings.ChatGptSettings.ApiKey}");
        }

        public void SetNoteTypeInstructions(string noteInstruction)
        {
            //Map noteInstructions to the actual note description

            noteTypeInstructions = noteInstruction;
        }

        public async Task<string> GetResponseAsync(string transcription)
        {
            var description = NoteTypeMapper.GetDescription(noteTypeInstructions);

            var requestPayload = new
            {
                messages = new[]
                {
                    new { role = "system", content = description },
                    new { role = "user", content = transcription }
                },
                max_tokens = appSettings.ChatGptSettings.MaxTokens,
                temperature = appSettings.ChatGptSettings.Temperature,
                top_p = appSettings.ChatGptSettings.TopP,
                model = appSettings.ChatGptSettings.Model
            };


            var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload);
            var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(appSettings.ChatGptSettings.EndPoint, httpContent);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                JObject parsedResponse = JObject.Parse(jsonResponse);
                return parsedResponse["choices"][0]["message"]["content"].ToString();

            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve response: {response.ReasonPhrase}. Response content: {errorContent}");
            }

        }
    }
}
