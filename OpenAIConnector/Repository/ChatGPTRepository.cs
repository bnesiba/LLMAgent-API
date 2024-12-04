using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OpenAIConnector.Models;

namespace OpenAIConnector.Repository
{
    public class ChatGPTRepository
    {
        private readonly HttpClient _httpClient;
        private Uri _baseUrl;

        public ChatGPTRepository(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();

            var authToken = configuration.GetSection("OpenAISettings").GetValue<string>("AuthToken") ?? string.Empty;
            _baseUrl = new Uri(configuration.GetSection("OpenAISettings").GetValue<string>("ChatGPTUri") ?? string.Empty);

            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + authToken);

        }

        public OpenAIModelsResponse? GetModels()
        {
            var response = _httpClient.GetAsync(_baseUrl + "models").Result;
            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;
                var responseString = responseContent.ReadAsStringAsync().Result;
                var models = JsonConvert.DeserializeObject<OpenAIModelsResponse>(responseString);
                return models;
            }
            else
            {
                return null;
            }
        }

        public OpenAIChatResponse? SimpleChat(string message)
        {
            var chatRequest = new OpenAIChatRequest()
            {
                model = "gpt-4o-mini",
                messages = new List<IOpenAIChatMessage>
                {
                    new OpenAIUserMessage(message)
                }
            };
            return Chat(chatRequest);
        }

        public OpenAIChatResponse? AdvancedChat(string systemMessage, string userMessage,
            List<IOpenAIChatMessage> currentMessages)
        {
            currentMessages.Add(new OpenAISystemMessage(systemMessage));
            currentMessages.Add(new OpenAIUserMessage(userMessage));
            var chatRequest = new OpenAIChatRequest()
            {
                model = "gpt-4o-mini",
                messages = currentMessages
            };
            return Chat(chatRequest);
        }

        public OpenAIChatResponse? Chat(OpenAIChatRequest chatRequest)
        {
            var json = JsonConvert.SerializeObject(chatRequest, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = _httpClient.PostAsync(_baseUrl + "chat/completions", content).Result;
            var responseContent1 = response.Content.ReadAsStringAsync().Result; //whole message as a string for debugging - openai gets fairly descriptive with errors


            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;
                var responseString = responseContent1;
                var chatResponse = JsonConvert.DeserializeObject<OpenAIChatResponse>(responseString);
                return chatResponse;
            }
            else
            {
                //TODO: should probably throw here?
                return null;
            }
        }

        //TODO: find a way to avoid having to use the image request
        public OpenAIChatResponse? Chat(OpenAIImageChatRequest chatRequest)
        {
            var json = JsonConvert.SerializeObject(chatRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = _httpClient.PostAsync(_baseUrl + "chat/completions", content).Result;
            var responseContent1 = response.Content.ReadAsStringAsync().Result;


            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;
                var responseString = responseContent1;
                var chatResponse = JsonConvert.DeserializeObject<OpenAIChatResponse>(responseString);
                return chatResponse;
            }
            else
            {
                //TODO: should probably throw here?
                return null;
            }
        }
    }
}
