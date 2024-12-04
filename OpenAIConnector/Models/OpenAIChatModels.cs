using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenAIConnector.Models
{
    public class OpenAIModelsResponse
    {
        public List<OpenAIModelInfo> data { get; set; }
    }


    public class OpenAIModelInfo
    {
        public string id { get; set; }

        [JsonPropertyName("object")]
        public string objType { get; set; }
        public int created { get; set; }
        public string owned_by { get; set; }
    }

    public static class OpenAIMessageRoles
    {
        public static string system = "system";
        public static string user = "user";
        public static string assistant = "assistant";
    }


    public class OpenAIChatRequest
    {
        public string model { get; set; }
        public List<IOpenAIChatMessage> messages { get; set; }

        //randomness/chaos 0-2
        public int? temperature { get; set; }

        public OpenAITool[]? tools { get; set; }
        public OpenAITool? tool_choice { get; set; }

        public int? max_tokens { get; set; }

        public OpenAIChatRequest Copy()
        {
            var copiedRequest = new OpenAIChatRequest()
            {
                model = this.model,
                temperature = this.temperature,
                tools = this.tools,
                tool_choice = this.tool_choice,
                messages = new List<IOpenAIChatMessage>(),
                max_tokens = this.max_tokens
            };
            this.messages.ForEach(m => copiedRequest.messages.Add(m));
            return copiedRequest;
        }
    }

    //Apparently the vision preview completions blow up if the tools parameter exists at all (even if null)
    public class OpenAIImageChatRequest
    {
        public string model { get; set; }
        public List<IOpenAIChatMessage> messages { get; set; }

        //randomness/chaos 0-2
        public int? temperature { get; set; }

        public int? max_tokens { get; set; }
    }



    public class OpenAITool
    {
        public string type { get; set; }
        public OpenAiToolFunction function { get; set; }

    }

    public class OpenAiToolFunction
    {
        public string description { get; set; }
        public string name { get; set; }
        public object parameters { get; set; }

        public bool strict { get; set; }
    }

    public interface IOpenAIChatMessage
    {
        public string role { get; set; }

        //content can be an array rather than a string if needed

        public object content { get; set; }

    }

    public class OpenAISystemMessage : IOpenAIChatMessage
    {
        public string role { get; set; }
        public object content { get; set; }

        public string? name { get; set; }

        public OpenAISystemMessage(string s) : this("system", s)
        {
        }

        public OpenAISystemMessage(string? name, string content)
        {
            this.role = OpenAIMessageRoles.system;
            this.content = content;
            this.name = name;
        }
    }

    public class OpenAIToolMessage : IOpenAIChatMessage
    {
        public string role { get; set; }
        public object content { get; set; }
        public string tool_call_id { get; set; }
        //public string? name { get; set; }

        public OpenAIToolMessage()
        {
        }

        public OpenAIToolMessage(string content/*,string name*/, string tool_call_id)
        {
            this.role = "tool";
            this.content = content;
            //this.name = name;
            this.tool_call_id = tool_call_id;
        }
    }

    public class OpenAIUserMessage : IOpenAIChatMessage
    {
        public string role { get; set; }
        public object content { get; set; }

        public OpenAIUserMessage()
        {
        }

        public OpenAIUserMessage(string content)
        {
            this.role = OpenAIMessageRoles.user;
            this.content = content;
        }
    }

    public class OpenAIUserImageMessage : IOpenAIChatMessage
    {
        public string role { get; set; }
        public object content { get; set; }

        public OpenAIUserImageMessage()
        {
            role = OpenAIMessageRoles.user;
            content = new List<OpenAIMessageContent>();
        }

        public OpenAIUserImageMessage(string content, string urlOrBase64)
        {
            this.role = OpenAIMessageRoles.user;
            this.content = new List<OpenAIMessageContent>()
            {
                new OpenAITextContent(content),
                new OpenAIImageContent(urlOrBase64, true)//high detail
            };
        }

        public OpenAIUserImageMessage(string content, List<string> urlsOrBase64s, bool highDetail = false)
        {
            this.role = OpenAIMessageRoles.user;
            this.content = new List<OpenAIMessageContent>()
            {
                new OpenAITextContent(content)
            };
            urlsOrBase64s.ForEach(urlOrBase64 => ((List<OpenAIMessageContent>)this.content).Add(new OpenAIImageContent(urlOrBase64, highDetail)));
        }
    }

    public interface OpenAIMessageContent
    {
        public string type { get; set; }
    }

    public class OpenAITextContent : OpenAIMessageContent
    {
        public string type { get; set; }
        public string text { get; set; }

        public OpenAITextContent()
        {
            type = "text";
            text = string.Empty;
        }

        public OpenAITextContent(string text)
        {
            type = "text";
            this.text = text;
        }
    }

    public class OpenAIImageContent : OpenAIMessageContent
    {
        public string type { get; set; }
        public OpenAIImagePart image_url { get; set; }

        public OpenAIImageContent()
        {
            type = "image_url";
            image_url = new OpenAIImagePart();
        }

        public OpenAIImageContent(string urlOrBase64, bool highDetail = false)
        {
            type = "image_url";
            image_url = new OpenAIImagePart(urlOrBase64, highDetail);
        }

    }

    public class OpenAIImagePart
    {
        public string url { get; set; }
        public string detail { get; set; }

        public OpenAIImagePart()
        {
            url = string.Empty;
            detail = "low";
        }

        public OpenAIImagePart(string url, bool highDetail = false)
        {
            this.url = url;
            detail = highDetail ? "high" : "low";
        }
    }


    public class OpenAIAssistantMessage : IOpenAIChatMessage
    {
        public string role { get; set; }
        public object? content { get; set; }
        //public string? name { get; set; }
        public List<OpenAIToolCall>? tool_calls { get; set; }

        public OpenAIAssistantMessage()
        {
        }
        public OpenAIAssistantMessage(/*string? name, */string? content = null, List<OpenAIToolCall>? toolCalls = null)
        {
            this.role = OpenAIMessageRoles.assistant;
            this.content = content;
            //this.name = name;
            this.tool_calls = toolCalls;
        }
    }

    public class OpenAIToolCall
    {
        public string id { get; set; }
        public string type { get; set; }
        public OpenAIFunctionCall function { get; set; }
    }

    public class OpenAIFunctionCall
    {
        public string name { get; set; }
        public string arguments { get; set; }
    }


    public class OpenAIChatResponse
    {
        public string id { get; set; }
        public string objType { get; set; }
        public int created { get; set; }
        public string model { get; set; }
        public string system_fingerprint { get; set; }
        public List<OpenAIChatChoice> choices { get; set; }
        public OpenAIChatUsage usage { get; set; }
    }

    public class OpenAIChatChoice
    {
        public int index { get; set; }
        public OpenAIAssistantMessage message { get; set; }
        public string logprobs { get; set; }
        public string finish_reason { get; set; }
    }

    public class OpenAIChatUsage
    {
        public int prompt_tokens { get; set; }
        public int completion_tokens { get; set; }
        public int total_tokens { get; set; }
    }

    public static class OpenAIModelExtensions
    {
        public static bool HasToolCalls(this OpenAIChatResponse response, out List<OpenAIToolCall> toolCalls)
        {
            toolCalls = new List<OpenAIToolCall>();
            if (response == null) return false;

            if (response != null && response.choices.Any())
            {
                toolCalls = response.choices[0].message.tool_calls ?? new List<OpenAIToolCall>();
            }

            return toolCalls.Any();
        }

    }
}
