using OpenAIConnector.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolManagementFlow.Models;

namespace OpenAIConnector.Extensions
{
    public static class ToolArrayExtensions
    {
        public static OpenAITool[] GetToolDefinitions(this List<IToolDefinition> tools)
        {
            return tools.Select(t => t.GetToolRequestDefinition()).ToArray();
        }
    }
}
