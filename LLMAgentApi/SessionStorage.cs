using OpenAIConnector.Models;

namespace LLMAgentApi
{
    public class SessionStorage
    {
        private Dictionary<Guid, List<IOpenAIChatMessage>> _sessionStorage;

        public SessionStorage()
        {
            _sessionStorage = new Dictionary<Guid, List<IOpenAIChatMessage>>();
        }

        public Guid StartSession()
        {
            var sessionGuid = Guid.NewGuid();
            _sessionStorage.Add(sessionGuid, new List<IOpenAIChatMessage>());
            return sessionGuid;
        }

        public Guid StartSession(string systemPrompt)
        {
            var sessionGuid = Guid.NewGuid();
            _sessionStorage.Add(sessionGuid, new List<IOpenAIChatMessage>() { new OpenAISystemMessage(systemPrompt)});
            return sessionGuid;
        }

        public List<IOpenAIChatMessage> GetSessionData(Guid sessionId)
        {
            return _sessionStorage.ContainsKey(sessionId) ? _sessionStorage[sessionId] : new List<IOpenAIChatMessage>();
        }

        public void SetSessionData(Guid guid, List<IOpenAIChatMessage> data) 
        {
            _sessionStorage[guid] = data; 
        }

        public void AddMessageToSession(Guid guid, IOpenAIChatMessage message)
        {
            _sessionStorage[guid].Add(message);
        }

        public void AddMessagesToSession(Guid guid, List<IOpenAIChatMessage> messages)
        {
            _sessionStorage[guid].AddRange(messages);
        }

        public void AddUserMessageToSession(Guid guid, string message)
        {
            AddMessageToSession(guid, new OpenAIUserMessage(message));
        }

        public void AddAssistantMessageToSession(Guid guid, string message)
        {
            AddMessageToSession(guid, new OpenAIAssistantMessage(message));
        }
    }
}
