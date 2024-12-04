Simple C# .Net api implementation of a Tool-using-agent. 

Basic functionality includes:
- basic chat functionality
- simple and full context/history management
- tool abstraction/tool use implementation
- references returned to UI/consumer

Session History:
- Simple Sesion history (SimpleSesionChat endpoint)
    - Only adds input and output messages. Does not include messages generated during session (no tool messages)
    - Smaller context impact
    - No memory of specific tool calls or data returned

- Full Session History (sessionChat endpoint)
    - Add all messages generated during session, including tool messages.
    - More context used (depending on tools could be significantly more)
    - Agent has memory of previous tool calls and can reason about issues/changes to calls.
    - Agent can access past results, removing the need for repeated "get" tool calls.


Tool Using Agent:
- Tools injected into agent
- Tool loop allows agent to use tools in parallel or series to accomplish goals.

Tools:
- SubmitStatBlock
    - Allows the agent to create a dnd-style statblock for the user and cause a link to be returned that provides a formatted custom dnd-style statblock.

- ClassicMonsterSearch
    - Allows the agent to get official statblocks for classic D&D monsters
    - Calls D&D beyond and parses results


