using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ToolManagementFlow.Models
{
    public class ToolProperty
    {
        public string name { get; init; }
        public virtual string type => "string";
        public string description { get; init; }

        [IgnoreDataMember]
        public bool IsRequired { get; init; } = true;
    }

    public class ObjectToolProperty : ToolProperty
    {
        public override string type => "object";
        public List<ToolProperty> properties { get; init; }
    }

    public class ArrayToolProperty : ToolProperty
    {
        public override string type => "array";

        public ToolProperty items { get; init; }
    }

    public class EnumToolProperty : ToolProperty
    {
        [JsonProperty("enum")]
        public List<string> enumValues { get; set; }
    }


    //↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ These are the models used by OpenAI. If you change them you might get errors ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓

    public class ToolParameter
    {
        public string type { get; init; }
        public string description { get; init; }
    }

    public class ArrayToolParameter : ToolParameter
    {
        public ToolParameter items { get; init; }
    }

    public class EnumToolParameter : ToolParameter
    {
        [JsonProperty("enum")]
        public List<string> enumValues { get; init; }
    }

    public class ObjectToolParameter : ToolParameter
    {
        public Dictionary<string, ToolParameter> properties { get; init; }
        public bool additionalProperties { get; set; } = false; //TODO: figure out -> required for object parameters using strict i think? (this could be for optional properties maybe?)
        
        public string[] required { get; init; }

    }
}
