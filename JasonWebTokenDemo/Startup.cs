using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JasonWebTokenDemo.Auth;
using JasonWebTokenDemo.Options;

namespace JasonWebTokenDemo
{
    public class Startup
    {
        private readonly SymmetricSecurityKey _signingKey;

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            
            // 取得 JWT 的密鑰
            _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["SecretKey:SymmetricSecurityKey"]));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 若有程式需要注入 IOptions<T>，則要先做以下設定
            services.AddOptions();

            // JwtFactory 類別需要注入 IOptions<JwtIssuerOptions>，這裡設定 JwtIssuerOptions 的初始值
            ConfigureJwtIssuerOptions(services);

            // Register JwtFactory
            services.AddScoped<IJwtFactory, JwtFactory>();

            services.AddMvc(config =>
            {
                // 一般情境下，後端資源通常需要有存取權限的用戶才能使用，
                // 所以這裡設定所有 Controller 的 Action 都需要驗證使用者，而本專案將使用 JWT 來做用戶驗證。
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }

        private void ConfigureJwtIssuerOptions(IServiceCollection services)
        {
            // 從 appsettings.json 取得 JwtIssuerOptions 節點的屬性值
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });
        }
    }
}
