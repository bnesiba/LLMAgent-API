using Newtonsoft.Json;
using OpenAIConnector.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolManagementFlow.Models;

namespace OpenAIConnector.Extensions
{
    public static class IToolDefinitionExtensions
    {
        public static OpenAITool GetToolRequestDefinition(this IToolDefinition toolDefinition)
        {
            Dictionary<string, ToolParameter> toolParameters = toolDefinition.InputParameters.Select(p => p.GetInputParametersObject()).ToDictionary();

            OpenAiToolFunction toolFunction = new OpenAiToolFunction()
            {
                description = toolDefinition.Description,
                name = toolDefinition.Name,
                parameters = new
                {
                    type = "object",
                    properties = toolParameters,
                    additionalProperties = false,
                    required = toolDefinition.InputParameters.Where(p => p.IsRequired).Select(p => p.name).ToArray()
                },
                strict = true
            };

            return new OpenAITool()
            {
                type = "function",
                function = toolFunction
            };
        }

        public static Dictionary<string, string>? GetToolRequestStringParameters(this IToolDefinition toolDefinition,
            OpenAIToolCall toolCall)
        {
            Dictionary<string, object>? requestParameters = JsonConvert.DeserializeObject<Dictionary<string, object>>(toolCall.function.arguments);

            Dictionary<string, string> resultsDictionary = new Dictionary<string, string>();
            if (requestParameters != null)
            {
                toolDefinition.InputParameters.Where(p => p.type != "array" && p.type != "object").ToList().ForEach(p =>
                {
                    string? valueString = requestParameters[p.name].ToString();
                    if (valueString != null)
                    {
                        resultsDictionary.Add(p.name, valueString);
                    }
                });

            }

            return resultsDictionary;
        }

        public static Dictionary<string, object>? GetToolRequestObjectParameters(this IToolDefinition toolDefinition,
            OpenAIToolCall toolCall)
        {
            Dictionary<string, object>? requestParameters = JsonConvert.DeserializeObject<Dictionary<string, object>>(toolCall.function.arguments);

            Dictionary<string, object> resultsDictionary = new Dictionary<string, object>();
            if (requestParameters != null)
            {
                toolDefinition.InputParameters.Where(p => p.type != "array" && p.type == "object").ToList().ForEach(p =>
                {
                    if (requestParameters.TryGetValue(p.name, out object valueString))
                    {
                        resultsDictionary.Add(p.name, valueString);
                    }
                });

            }

            return resultsDictionary;
        }

        public static Dictionary<string, List<string>>? GetToolRequestStringArrayParameters(this IToolDefinition toolDefinition,
            OpenAIToolCall toolCall)
        {
            Dictionary<string, object>? requestParameters = JsonConvert.DeserializeObject<Dictionary<string, object>>(toolCall.function.arguments);

            Dictionary<string, List<string>> resultsDictionary = new Dictionary<string, List<string>>();
            if (requestParameters != null)
            {
                toolDefinition.InputParameters.Where(p => p.type == "array" && ((ArrayToolProperty)p).items.type != "object").ToList().ForEach(p =>
                {
                    if (requestParameters.ContainsKey(p.name))
                    {
                        var arrayThing = JsonConvert.DeserializeObject<List<string>>(requestParameters[p.name].ToString() ?? string.Empty);
                        if (arrayThing != null)
                        {
                            resultsDictionary.Add(p.name, arrayThing);
                        }
                    }
                });
            }
            return resultsDictionary;
        }

        public static Dictionary<string, List<object>>? GetToolRequestObjArrayParameters(this IToolDefinition toolDefinition,
            OpenAIToolCall toolCall)
        {
            Dictionary<string, object>? requestParameters = JsonConvert.DeserializeObject<Dictionary<string, object>>(toolCall.function.arguments);

            Dictionary<string, List<object>> resultsDictionary = new Dictionary<string, List<object>>();
            if (requestParameters != null)
            {
                toolDefinition.InputParameters.Where(p => p.type == "array" && ((ArrayToolProperty)p).items.type == "object").ToList().ForEach(p =>
                {
                    if (requestParameters.ContainsKey(p.name))
                    {
                        var arrayThing = JsonConvert.DeserializeObject<List<object>>(requestParameters[p.name].ToString() ?? string.Empty);
                        if (arrayThing != null)
                        {
                            resultsDictionary.Add(p.name, arrayThing);
                        }
                    }
                });
            }
            return resultsDictionary;
        }

        ///<summary>
        /// Due to improvements in Chatgpt, these methods may no longer be necessary.
        /// They were built to force a retry if the agent failed to include required fields in a tool-call. This no longer seems to happen.
        /// They could be useful if you want to turn 'strict' off and allow fuzzier inputs. 
        /// </summary>
        public static bool RequestArgumentsValid<T>(this IToolDefinition toolDefinition,
            Dictionary<string, T>? requestParameters)
        {
            if (requestParameters == null)
            {
                return false;
            }

            foreach (var parameter in toolDefinition.InputParameters)
            {
                if (parameter.IsRequired && !requestParameters.ContainsKey(parameter.name))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool RequestArgumentsValid<T1, T2>(this IToolDefinition toolDefinition,
            Dictionary<string, T1>? requestStringParameters, Dictionary<string, T2>? requestArrayParameters)
        {

            foreach (var parameter in toolDefinition.InputParameters)
            {
                if (parameter.IsRequired)
                {
                    if ((requestStringParameters == null || !requestStringParameters.ContainsKey(parameter.name))
                        && (requestArrayParameters == null || !requestArrayParameters.ContainsKey(parameter.name)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool RequestArgumentsValid<T1, T2, T3, T4>(this IToolDefinition toolDefinition,
            Dictionary<string, T1>? requestStringParameters, Dictionary<string, T2>? requestObjParameters, Dictionary<string, T3>? requestStringArrayParameters, Dictionary<string, T4>? requestObjArrayParameters)
        {

            foreach (var parameter in toolDefinition.InputParameters)
            {
                if (parameter.IsRequired)
                {
                    if ((requestStringParameters == null || !requestStringParameters.ContainsKey(parameter.name))
                        && (requestObjParameters == null || !requestObjParameters.ContainsKey(parameter.name))
                        && (requestStringArrayParameters == null || !requestStringArrayParameters.ContainsKey(parameter.name))
                        && (requestObjArrayParameters == null || !requestObjArrayParameters.ContainsKey(parameter.name)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
