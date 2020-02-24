using Microsoft.AspNetCore.SignalR;
using SingalRTest.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SingalRTest.hub
{
    public class CountHub : Hub
    {
        private readonly CountService _countService;

        public CountHub(CountService countService)
        {
            _countService = countService;
        }

        public async Task GetLatestCount(string random)
        {
            Console.WriteLine(random);
            int count;
            do
            {
                //var userName = Context.User.Identity.Name;
                count = _countService.GetLatestCount();
                Thread.Sleep(1000);
                await Clients.All.SendAsync("ReceiveUpdate", count);
            } while (count < 10);

            await Clients.All.SendAsync("Finished");
        }

        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            await Clients.Client(connectionId).SendAsync("someFunc", new { random = "on start" });
            //await Clients.AllExcept(connectionId).SendAsync("someFunc");

            //await Groups.AddToGroupAsync(connectionId, "MyGroup");
            //await Groups.RemoveFromGroupAsync(connectionId, "MyGroup");

            //await Clients.Group("MyGroup").SendAsync("someFunc");
        }
    }
}
