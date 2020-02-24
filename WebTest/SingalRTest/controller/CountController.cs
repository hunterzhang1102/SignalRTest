using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SingalRTest.hub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SingalRTest.controller
{
    [Route("api/count")]
    public class CountController : Controller
    {
        private readonly IHubContext<CountHub> _countHub;

        public CountController(IHubContext<CountHub> countHub)
        {
            _countHub = countHub;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            await _countHub.Clients.All.SendAsync("someFunc", new { random = "abcd" });
            return Accepted(1); // 202: 请求已被接受并处理，但是还没有处理完成
        }
    }
}
