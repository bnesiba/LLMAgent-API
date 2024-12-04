using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenAIConnector.Models;
using OpenAIConnector.Extensions;

namespace ToolManagementFlow.Models
{
    public class ToolRequestParameters
    {
        public string ToolName { get; set; }
        public string ToolRequestId { get; set; }
        public Dictionary<string,string>? StringParameters { get; set; }

        public Dictionary<string, object>? ObjectParameters { get; set; }
        public Dictionary<string, List<string>>? StringArrayParameters { get; set; }
        public Dictionary<string, List<object>>? ObjectArrayParameters { get; set; }

        public ToolRequestParameters(string name, string id, Dictionary<string,string>? strParams,Dictionary<string, object>? objectParameters,  Dictionary<string, List<string>>? stringArrayParams, Dictionary<string, List<object>>? objectArrayParameters)
        {
            ToolName = name;
            ToolRequestId = id;
            StringParameters = strParams;
            ObjectParameters = objectParameters;
            StringArrayParameters = stringArrayParams;
            ObjectArrayParameters = objectArrayParameters;
        }

        public ToolRequestParameters(IToolDefinition toolDef, OpenAIToolCall toolCall): 
            this(toolDef.Name, toolCall.id,
                toolDef.GetToolRequestStringParameters(toolCall), 
                toolDef.GetToolRequestObjectParameters(toolCall),
                toolDef.GetToolRequestStringArrayParameters(toolCall), 
                toolDef.GetToolRequestObjArrayParameters(toolCall))
        {
        }

        public string? GetStringParam(string key)
        {
            if (StringParameters == null) return null;
            return StringParameters.ContainsKey(key) ? StringParameters[key] : null;
        }

        public T? GetObjectParam<T>(string key)
        {
            if (ObjectParameters == null) return default(T);
            if (ObjectParameters.ContainsKey(key))
            {
                var valueObject = ObjectParameters[key];
                T? convertedObject = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(valueObject));
                return convertedObject;
            }
            return default(T);
        }

        public List<string>? GetStringArrayParam(string key)
        {
            if (StringArrayParameters == null) return null;
            return StringArrayParameters.ContainsKey(key) ? StringArrayParameters[key] : null;
        }

        public List<T>? GetObjectArrayParam<T>(string key)
        {
            if (ObjectArrayParameters == null) return null;
            if (ObjectArrayParameters.ContainsKey(key))
            {
                var objectList = ObjectArrayParameters[key];
                List<T>? convertedList = JsonConvert.DeserializeObject<List<T>>(JsonConvert.SerializeObject(objectList));
                return convertedList;
            }

            return null;
        }
    }
}
