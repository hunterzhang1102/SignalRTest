using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebTest.AppStart;
using WebTest.Repository;
using WebTest.Service;

namespace WebTest
{
    public class Startup
    {
        private static List<WebSocket> SocketList = new List<WebSocket>();
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            UserRepository.connStr = Configuration.GetConnectionString("DefaultConnection");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(MyService));
            CrosDomain.AddConfigureService(services);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            HttpsRedirect.Configure(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            CrosDomain.CrosConfigures(app);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //HttpsRedirect.Use(app);
            app.UseStaticFiles();
            #region UseWebSocketsOptionsAO
            //var webSocketOptions = new WebSocketOptions()
            //{
            //    KeepAliveInterval = TimeSpan.FromSeconds(120),// 向客户端发送“ping”帧的频率，以确保代理保持连接处于打开状态。 默认值为 2 分钟。
            //    ReceiveBufferSize = 4 * 1024// 用于接收数据的缓冲区的大小。 高级用户可能需要对其进行更改，以便根据数据大小调整性能。 默认值为 4 KB。
            //};
            //webSocketOptions.AllowedOrigins.Add("https://client.com");
            //webSocketOptions.AllowedOrigins.Add("https://www.client.com");
            //app.UseWebSockets(webSocketOptions);
            app.UseWebSockets();
            #endregion
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        SocketList.Add(webSocket);
                        await Echo(context, webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else if (context.Request.Path == "/all")
                {
                    WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    SocketList.Add(webSocket);
                    await EchoAll(context);
                }
                else
                {
                    await next();
                }
            });
            app.UseMvc();
        }

        private async Task EchoAll(HttpContext context)
        {
            string str = "通知你们全部";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str);
            SocketList = SocketList.Where(s => s.State == WebSocketState.Open).ToList();
            foreach (WebSocket socket in SocketList)
            {
                await socket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), 
                    WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        private async Task Echo(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                //var test = new ArraySegment<byte>(buffer, 0, result.Count);
                //string str = System.Text.Encoding.UTF8.GetString(test);
                //byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
