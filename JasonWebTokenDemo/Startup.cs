using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JasonWebTokenDemo.Core;
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

            // 從 appsettings.json 取得 JWT 的密鑰
            // 在真實情境下，密鑰請放在安全的地方，例如伺服器的環境變數
            _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["SecretKey:SymmetricSecurityKey"]));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 若有程式需要注入 IOptions<T>，則要先做以下設定
            services.AddOptions();

            // JwtFactory 類別需要注入 IOptions<JwtIssuerOptions>，這裡設定 JwtIssuerOptions 的初始值
            ConfigureJwtIssuerOptions(services);

            // 設定 Authentication middleware 如何去驗證 JWT
            ConfigureJwtAuthentication(services);

            // Register JwtFactory
            services.AddScoped<IJwtFactory, JwtFactory>();
            
            services.AddMvc(config =>
            {
                // 一般情境下，後端資源通常只能給具有存取權限的用戶使用，所以這裡設定全域套用 AuthorizeAttribute
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

            // 設定 Authentication middleware 負責自動驗證遠端的請求，本專案是負責 JWT Bearer 的驗證
            app.UseAuthentication();

            app.UseMvc();
        }


        #region private method

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
        
        private void ConfigureJwtAuthentication(IServiceCollection services)
        {
            // 從 appsettings.json 取得 JwtIssuerOptions 節點的屬性值
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // sets up the validation parameters that we’re going to pass to ASP.NET’s JWT bearer authentication middleware.
            var tokenValidationParameters = new TokenValidationParameters
            {
                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                // The signing key must match
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                // Validate the token expiry
                RequireExpirationTime = true,
                ValidateLifetime = true,

                // 設定這個屬性，User.Identity.Name 才會有值，這裡也可以指定其他 Claim Type
                NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",

                ClockSkew = TimeSpan.Zero
            };
            
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = tokenValidationParameters;
            });
        }

        #endregion private method
    }
}
