using LLMAgentApi.DataStorage.StatblockStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static LLMAgentApi.Controllers.ChatController;

namespace LLMAgentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReferencesController : ControllerBase
    {
        private StatblockStorage _statblockStorage;
        public ReferencesController(StatblockStorage statblockStorage) 
        {
            _statblockStorage = statblockStorage;
        }

        //Get references by type and id
        //for now, there's only one type of reference: "statblock".
        [HttpGet("{type}/{id}")]
        public object GetReferenceByTypeAndId([FromRoute] string type, [FromRoute] string id)
        {
            if(type == "statblock")//TODO: make consts?
            {
                return _statblockStorage.GetStatblock(id);
            }
            return NotFound(id);
        }
    }
}
