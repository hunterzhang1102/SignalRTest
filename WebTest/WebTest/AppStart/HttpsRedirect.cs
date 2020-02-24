using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace WebTest.AppStart
{
    /// <summary>
    /// Https 配置
    /// </summary>
    public class HttpsRedirect
    {
        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="services"></param>
        public static void Configure(IServiceCollection services)
        {
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });

            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                options.HttpsPort = 5002;
            });
        }

        /// <summary>
        /// 使用
        /// </summary>
        /// <param name="app"></param>
        public static void Use(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
        }
    }
}
