
using DataModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProtoBuf;
using Serilog;

namespace CoreHost.Controllers
{
    [Route("api/values")]
    public class ValueController : Controller
    {
        
        // GET
        [HttpGet("GetString")]
        public TestResponseDto Index()
        {
            Log.Information("hahahah ({name})","12121");
            return new TestResponseDto
            {
                Name = "1",
                FirstName = "2",
                LastName = "3"
            };
        }

        [HttpPost("Input")]
        public TestResponseDto Input(TestResponseDto request)
        {
            Log.Information($"{JsonConvert.SerializeObject(request)}");
            return new TestResponseDto
            {
                Name = "this ",
                FirstName = "is ",
                LastName = "post "
            };
        }

    }
}