namespace LLMAgentApi
{
    public class ChatReferenceService
    {
        private List<ChatReference> _chatReferences = new List<ChatReference>();

        public ChatReferenceService() { }

        public void InsertReference(string id, string type)
        {
            var newReference = new ChatReference()
            {
                refId = id,
                refType = type,
                refDate = DateTime.Now,
            };

            _chatReferences.Add( newReference);
        }

        public List<ChatReference> GetChatReferences()
        {
            return _chatReferences;
        }

    }

    public class ChatReference
    {
        public string refId { get; set; }
        public string refType { get; set; }
        public DateTime refDate { get; set; }
    }
}
