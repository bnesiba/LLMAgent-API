using DnDBeyondConnector.Repository;
using Newtonsoft.Json;
using OpenAIConnector.Models;
using System.Net.Http;
using ToolManagementFlow.Models;

namespace LLMAgentApi.ToolDefinitions
{
    public class ClassicMonsterSearch : IToolDefinition
    {
        private DnDBeyondMonsterSearchRepo _monsterRepo;
        public ClassicMonsterSearch(DnDBeyondMonsterSearchRepo monsterRepo) {
            _monsterRepo = monsterRepo;
        }

        public string Name => "ClassicMonsterSearch";

        public string Description => "Search for stats and information on classic D&D monsters";

        public List<ToolProperty> InputParameters => new List<ToolProperty>()
        {
            new EnumToolProperty()
            {
                name = "MonsterName",
                description = "the classic monster to search for",
                enumValues = new List<string>{"Red Dragon", "Goblin", "Kobold", "Orc", "Elemental"}
            }
        };

        public OpenAIToolMessage ExecuteTool(List<IOpenAIChatMessage> chatContext, ToolRequestParameters toolRequestParameters)
        {
            var monsterName = toolRequestParameters.GetStringParam("MonsterName") ?? string.Empty;
            var monsterResults = _monsterRepo.SearchForMonster(monsterName);
            var resultsString = JsonConvert.SerializeObject(monsterResults);
            return new OpenAIToolMessage($"ClassicMonsterSearch: \r\n" + resultsString, toolRequestParameters.ToolRequestId);
        }
    }
}
