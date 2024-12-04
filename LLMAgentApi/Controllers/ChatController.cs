using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LLMAgentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private ChatOrchestrator _chatOrchestrator;
        private SessionStorage _sessionStorage;
        private ChatReferenceService _referenceService;
        private string defaultSystemPrompt = "You are a friendly assistant named Helpy. You use the tools you have access to resolve user queries. " +
            "If you don't have a tool that would help, you resolve it yourself without using any tools";

        public ChatController(ChatOrchestrator orch, SessionStorage storage, ChatReferenceService referenceService)
        {
            _chatOrchestrator = orch;
            _sessionStorage = storage;
            _referenceService = referenceService;
        }

        [HttpPost("simpleChat")]
        public string? SimpleChat([FromBody] string message)
        {
            return _chatOrchestrator.SimpleChat(message);
        }

        [HttpGet("session")]
        public Guid StartSession()
        {
            return _sessionStorage.StartSession(defaultSystemPrompt);
        }

        [HttpGet("systemSession/{systemPrompt}")]
        public Guid StartSessionWithSystemPrompt([FromRoute] string systemPrompt)
        {
            return _sessionStorage.StartSession(systemPrompt);
        }

        //This session chat only includes user and assistant messages in the session
        //saving tokens in context but losing some information about tool calls
        [HttpPost("simpleSessionChat/{sessionId}")]
        public ResponseMessage SimpleSessionChat([FromRoute] Guid sessionId, [FromBody] string message)
        {
            var sessionState = _sessionStorage.GetSessionData(sessionId);

            var chatResult = _chatOrchestrator.ContextChat(message, sessionState);
            if (chatResult != null)
            {
                _sessionStorage.AddUserMessageToSession(sessionId, message);
                _sessionStorage.AddAssistantMessageToSession(sessionId, chatResult);
                var chatReferences = _referenceService.GetChatReferences();
                return new ResponseMessage(chatResult, chatReferences);
            }
            return new ResponseMessage(string.Empty, new List<ChatReference>());
        }

        //This session chat includes all messages from the session
        //allowing the agent to recall specific tool uses, but using more tokens on each call
        [HttpPost("sessionChat/{sessionId}")]
        public ResponseMessage SessionChat([FromRoute]Guid sessionId, [FromBody] string message)
        {
            var chatResult = _chatOrchestrator.SessionContextChat(sessionId,message);
            var chatReferences = _referenceService.GetChatReferences();
            return new ResponseMessage(chatResult ?? string.Empty, chatReferences);
        }

        //Specific startup method for the demo
        //UI passes information that gets incorporated into the inital message
        //Uses the optional parameter in StartSession to set a custom system prompt.
        [HttpGet("init/{userName}")]
        public WelcomeMessage InitAgentChat([FromRoute] string userName)
        {
            //var sessionId = _sessionStorage.StartSession(); // <-- uses defaultSystemPrompt (see above)
            var sessionId =  _sessionStorage.StartSession("You are Helpy (do not mention your name unless directly asked) a helpful D&D assistant. " +
                "You help set up encounters, create statblocks, and generally assist with setting up encounters.\n"
                +"You use your available tools whenever relevant, and if a request cannot be solved with tools or doesn't need them, you resolve it yourself.\n" +
                "you response in short, concise, accurate responses. Avoid verbosity unless directed otherwise" );
            var chatResult = _chatOrchestrator.SessionContextChat(sessionId, $"The user's name is {userName}. \nPlease welcome them and list the tools you have access to");
            return new WelcomeMessage(sessionId.ToString(), chatResult ?? "");
        }


        public class WelcomeMessage: ResponseMessage
        {
            public string SessionId { get; set; }

            public WelcomeMessage(string sessionId, string message): base(message, new List<ChatReference>()) { SessionId = sessionId; }
        }

        public class ResponseMessage
        {
            public string Message { get; set; }
            public List<ChatReference> ChatReferences { get; set; }

            public ResponseMessage(string message, List<ChatReference> chatReferences)
            {
                Message = message;
                ChatReferences = chatReferences;
            }
        }
    }
}
