using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolManagementFlow.Models;

namespace OpenAIConnector.Extensions
{
    public static class ToolPropertyExtensions
    {
        public static KeyValuePair<string, ToolParameter> GetInputParametersObject(this ToolProperty toolProperty)
        {
            if (toolProperty is ArrayToolProperty)
            {
                ArrayToolProperty arrProp = (ArrayToolProperty)toolProperty;
                return new KeyValuePair<string, ToolParameter>(toolProperty.name, new ArrayToolParameter
                {
                    description = arrProp.description,
                    type = arrProp.type,
                    items = GetInputParametersObject(arrProp.items).Value,
                });
            }
            if (toolProperty is EnumToolProperty)
            {
                EnumToolProperty enumToolProperty = (EnumToolProperty)toolProperty;
                return new KeyValuePair<string, ToolParameter>(toolProperty.name, new EnumToolParameter
                {
                    type = enumToolProperty.type,
                    description = enumToolProperty.description,
                    enumValues = enumToolProperty.enumValues
                });
            }

            if (toolProperty is ObjectToolProperty)
            {
                ObjectToolProperty objectToolProperty = (ObjectToolProperty)toolProperty;
                return new KeyValuePair<string, ToolParameter>(toolProperty.name, new ObjectToolParameter
                {
                    type = objectToolProperty.type,
                    description = objectToolProperty.description,
                    properties = objectToolProperty.properties.Select(GetInputParametersObject).ToDictionary(),
                    required = objectToolProperty.properties.Where(p => p.IsRequired).Select(p => p.name).ToArray()
                });
            }
            return new KeyValuePair<string, ToolParameter>(toolProperty.name, new ToolParameter { type = toolProperty.type, description = toolProperty.description });
        }
    }
}
