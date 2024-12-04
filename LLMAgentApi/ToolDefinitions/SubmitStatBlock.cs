using LLMAgentApi.DataStorage.StatblockStorage;
using Newtonsoft.Json;
using OpenAIConnector.Models;
using ToolManagementFlow.Models;

namespace LLMAgentApi.ToolDefinitions
{
    public class SubmitStatBlock : IToolDefinition
    {
        private StatblockStorage _statblockStorage;
        private ChatReferenceService _referenceService;
        public SubmitStatBlock(ChatReferenceService referenceService, StatblockStorage statblockStorage) 
        {
            _referenceService = referenceService;
            _statblockStorage = statblockStorage;
        }

        public string Name => "SubmitStatBlock";

        public string Description => "Submit a stat block to save it and send it back to the user. **USE THIS TOOL ANY TIME YOU CREATE A STATBLOCK**";

        public List<ToolProperty> InputParameters => new List<ToolProperty>()
        {
            new ToolProperty()
            {
                name = "name",
                description = "The name of the monster. ex. 'Goblin'"
            },
            new EnumToolProperty()
            {
                name = "size",
                description = "The size of the monster. ex. 'small'",
                enumValues = new List<string>{"tiny", "small", "medium", "large", "huge", "garganguan"}
            },
            new ToolProperty()
            {
                name = "type",
                description = "The type of the monster. ex. 'humanoid'"
            },
            new ToolProperty()
            {
                name = "alignment",
                description = "The alignment of the monster. ex. 'neutral evil'"
            },
            new ToolProperty()
            {
                name = "armorClass",
                description = "The armor class of the monster. Armor class is a number. ex. '13''"
            },
            new ToolProperty()
            {
                name = "hitPoints",
                description = "The hitpoints and hit dice of the monster. ex. '7 (2d6)'"
            },
             new ToolProperty()
            {
                name = "speed",
                description = "The speed of the monster. ex. '30 ft.'"
            },
             new ToolProperty()
            {
                name = "challengeRating",
                description = "The challege rating of the monster. This should be a number. ex. '1/8'",

            },
            new ObjectToolProperty() {
                name = "attributes",
                description = "The attributes of the monster",
                properties = new List<ToolProperty>()
                {
                    new ObjectToolProperty()
                    {
                        name = "STR",
                        description = "the STRength of the monster",
                        properties = new List<ToolProperty>()
                        {
                            new ToolProperty()
                            {
                                name = "value",
                                description = "the value of the attribute. ex 10"
                            },
                            new ToolProperty()
                            {
                                name = "modifier",
                                description = "the attribute modifier. ex. +1"
                            }
                        }
                    },
                    new ObjectToolProperty()
                    {
                        name = "DEX",
                        description = "the DEXterity of the monster",
                        properties = new List<ToolProperty>()
                        {
                            new ToolProperty()
                            {
                                name = "value",
                                description = "the value of the attribute. ex 10"
                            },
                            new ToolProperty()
                            {
                                name = "modifier",
                                description = "the attribute modifier. ex. +1"
                            }
                        }
                    },
                    new ObjectToolProperty()
                    {
                        name = "CON",
                        description = "the CONstitution of the monster",
                        properties = new List<ToolProperty>()
                        {
                            new ToolProperty()
                            {
                                name = "value",
                                description = "the value of the attribute. ex 10"
                            },
                            new ToolProperty()
                            {
                                name = "modifier",
                                description = "the attribute modifier. ex. +1"
                            }
                        }
                    },
                    new ObjectToolProperty()
                    {
                        name = "INT",
                        description = "the INTeligence of the monster",
                        properties = new List<ToolProperty>()
                        {
                            new ToolProperty()
                            {
                                name = "value",
                                description = "the value of the attribute. ex 10"
                            },
                            new ToolProperty()
                            {
                                name = "modifier",
                                description = "the attribute modifier. ex. +1"
                            }
                        }
                    },
                    new ObjectToolProperty()
                    {
                        name = "WIS",
                        description = "the WISdom of the monster",
                        properties = new List<ToolProperty>()
                        {
                            new ToolProperty()
                            {
                                name = "value",
                                description = "the value of the attribute. ex 10"
                            },
                            new ToolProperty()
                            {
                                name = "modifier",
                                description = "the attribute modifier. ex. +1"
                            }
                        }
                    },
                    new ObjectToolProperty()
                    {
                        name = "CHA",
                        description = "the CHArisma of the monster",
                        properties = new List<ToolProperty>()
                        {
                            new ToolProperty()
                            {
                                name = "value",
                                description = "the value of the attribute. ex 10"
                            },
                            new ToolProperty()
                            {
                                name = "modifier",
                                description = "the attribute modifier. ex. +1"
                            }
                        }
                    },
                }
            },
            new ArrayToolProperty()
            {
                name = "actions",
                description = "actions the monster can take in combat",
                items = new ObjectToolProperty()
                {
                    name = "action",
                    description = "an action the monster can take in combat",
                    properties = new List<ToolProperty>()
                    {
                        new ToolProperty()
                        {
                            name = "name",
                            description = "the name of the action. ex. 'Bite'"
                        },
                        new ToolProperty()
                        {
                            name = "description",
                            description = "a detailed description of the action. ex. 'Melee Weapon Attack: +4 to hit, reach 5 ft., one target. Hit: 5 (1d6 + 2) slashing damage.'"
                        }
                    }
                }
            }
        };

        public OpenAIToolMessage ExecuteTool(List<IOpenAIChatMessage> chatContext, ToolRequestParameters toolRequestParameters)
        {
            var generatedStatblock = MapToStatblock(toolRequestParameters);
            var storageId = _statblockStorage.InsertStatblock(generatedStatblock);
            _referenceService.InsertReference(storageId, "statblock");

            return new OpenAIToolMessage($"SubmitStatBlock: \r\n" + "Statblock added!", toolRequestParameters.ToolRequestId);
        }

        private MonsterStatblock MapToStatblock(ToolRequestParameters toolRequestParameters)
        {
            return new MonsterStatblock
            {
                name = toolRequestParameters.GetStringParam("name") ?? string.Empty,
                size = toolRequestParameters.GetStringParam("size") ?? string.Empty,
                type = toolRequestParameters.GetStringParam("type") ?? string.Empty,
                alignment = toolRequestParameters.GetStringParam("alignment") ?? string.Empty,
                armorClass = toolRequestParameters.GetStringParam("armorClass") ?? string.Empty,
                hitPoints = toolRequestParameters.GetStringParam("hitPoints") ?? string.Empty,
                speed = toolRequestParameters.GetStringParam("speed") ?? string.Empty,
                challengeRating = toolRequestParameters.GetStringParam("challengeRating") ?? string.Empty,
                attributes = toolRequestParameters.GetObjectParam<StatblockAttributes>("attributes"),
                actions = toolRequestParameters.GetObjectArrayParam<StatblockAction>("actions") ?? new List<StatblockAction>()
            };
        }
    }
}
