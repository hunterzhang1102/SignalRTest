using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace WebTest.AppStart
{
    /// <summary>
    /// 跨域
    /// </summary>
    public class CrosDomain
    {
        public static void AddConfigureService(IServiceCollection services)
        {
            #region 跨域 
            services.AddCors(options =>
                options.AddPolicy("any",
                builder => builder.WithOrigins("*")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin()
                .AllowCredentials())
            );
            #endregion
        }

        public static void CrosConfigures(IApplicationBuilder app)
        {
            app.UseCors("any");
        }
    }
}
