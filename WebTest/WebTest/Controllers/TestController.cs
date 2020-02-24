using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebTest.Service;

namespace WebTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly MyService _myService;
        public TestController(MyService myService)
        {
            _myService = myService;
        }

        [HttpGet("{id}")]
        public async void Get(int id)
        {
            Response.ContentType = "text/event-stream";
            int count = 0;

            do
            {
                count = _myService.GetLastedCount();
                Thread.Sleep(1000);

                if (count % 3 == 0)
                {
                    await HttpContext.Response.WriteAsync($"data:{count}\n\n");
                    await HttpContext.Response.Body.FlushAsync();
                }
            } while (count < 10);

            Response.Body.Close();
        }
    }
}
