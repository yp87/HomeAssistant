using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Supervisor.Controllers
{
    [ApiController]
    [Route("Automation")]
    public class AutomationWebhookController : ControllerBase
    {
        [Route("Webhook")]
        [HttpPost()]
        public async Task<IActionResult> ReceiveWebhookAsync()
        {
            using var eventReader = new StreamReader(Request.Body);
            var eventPayload = await eventReader.ReadToEndAsync();
            Console.WriteLine(eventPayload);
            return Ok();
        }
    }
}
