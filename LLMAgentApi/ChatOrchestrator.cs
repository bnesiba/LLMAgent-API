using OpenAIConnector.Extensions;
using OpenAIConnector.Models;
using OpenAIConnector.Repository;
using ToolManagementFlow.Models;

namespace LLMAgentApi
{
    public class ChatOrchestrator
    {
        private ChatGPTRepository _llmRepository;
        private SessionStorage _sessionStorage;
        private List<IToolDefinition> _toolDefinitions;

        public ChatOrchestrator(ChatGPTRepository llmRepo, SessionStorage sessionStorage, IEnumerable<IToolDefinition> definedTools )
        {
            _llmRepository = llmRepo;
            _sessionStorage = sessionStorage;
            _toolDefinitions = definedTools.ToList();
        }

        public string? SimpleChat(string message)
        {
            return _llmRepository.SimpleChat(message)?.choices[0].message?.content?.ToString();
        }

        public string? ContextChat(string message, List<IOpenAIChatMessage> context)
        {
            List<IOpenAIChatMessage> newContext =
            [
                .. context,
                new OpenAIUserMessage(message)
            ];
            return ContextChat(newContext)?.Last().content.ToString() ?? string.Empty;
        }

        public List<IOpenAIChatMessage>? ContextChat(List<IOpenAIChatMessage> context)
        {
            List<IOpenAIChatMessage> newContext =
            [
                .. context
            ];

            OpenAIChatRequest chatRequest = new OpenAIChatRequest
            {
                model = "gpt-4o-mini",
                messages = newContext,
                temperature = 1,
                tools = _toolDefinitions.GetToolDefinitions()
            };

            var chatResult = _llmRepository.Chat(chatRequest);
            if (chatResult != null)
            {
                //add message to context;
                newContext.Add(chatResult.choices[0].message);

                List<OpenAIToolCall> toolCalls;
                if (chatResult.HasToolCalls(out toolCalls))
                {
                    foreach (var call in toolCalls)
                    {
                        //find the tool to call
                        var toolCalled = _toolDefinitions.First(t => t.Name == call.function.name);
                        //run the tool
                        var toolMessage = toolCalled.ExecuteTool(newContext, new ToolRequestParameters(toolCalled, call));
                        //add the ToolMessage to thecontext
                        newContext.Add(toolMessage);
                    }
                    //recursively run ContextChat so that it can use the tool response(s) in the assistant response or call more tools
                    //I mostly avoid this recursion in my "gptManager" project by using a different architecture. 
                    return ContextChat(newContext);
                }
                else
                {
                    return newContext;
                }
            }
            return null;
        }

        public string? SessionContextChat(Guid sessionId, string message)
        {
            List<IOpenAIChatMessage> newContext =
                [
                    new OpenAIUserMessage(message),
                ];
            return SessionContextChat(sessionId, newContext);
        }

        public string? SessionContextChat(Guid sessionId, List<IOpenAIChatMessage> additionalContext)
        {
            var currentContext = _sessionStorage.GetSessionData(sessionId);

            List<IOpenAIChatMessage> newContext =
                [
                    .. currentContext,
                    ..additionalContext
                ];

            var contextResponse = ContextChat(newContext);
            if (contextResponse != null)
            {
                _sessionStorage.SetSessionData(sessionId, contextResponse);
                return contextResponse.Last().content.ToString();
            }
            return null;
        }

    }
}
