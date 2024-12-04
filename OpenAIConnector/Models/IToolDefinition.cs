using Newtonsoft.Json;
using OpenAIConnector.Models;

namespace ToolManagementFlow.Models
{ 
    public interface IToolDefinition
    {
        public string Name { get; }
        public string Description { get; }

        public List<ToolProperty> InputParameters { get; }

        public OpenAIToolMessage ExecuteTool(List<IOpenAIChatMessage> chatContext, ToolRequestParameters toolRequestParameters);

    }
}
