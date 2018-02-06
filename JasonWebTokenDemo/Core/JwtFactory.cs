using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using JasonWebTokenDemo.Helpers;
using JasonWebTokenDemo.Models;
using JasonWebTokenDemo.Options;

namespace JasonWebTokenDemo.Core
{
    public class JwtFactory : IJwtFactory
    {
        private readonly JwtIssuerOptions _jwtOptions;
        
        public JwtFactory(IOptionsSnapshot<JwtIssuerOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);
        }

        /// <summary>
        /// 產生要傳到前端的JWT
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<JwtView> GenerateAsync(IdentityUser user)
        {
            var result = new JwtView()
            {
                User = new JwtView.UserInfo()
                {
                    Account = user.UserName,
                    Email = user.Email,
                    Id = user.Id
                },
                AccessToken = await this.EncodeTokenAsync(user),
                ExpiresIn = (int)_jwtOptions.ValidFor.TotalSeconds
            };

            return result;
        }

        private async Task<string> EncodeTokenAsync(IdentityUser user)
        {
            //  Token 可以存放用戶的一些基本資料
            var identity = new ClaimsIdentity(new GenericIdentity(user.UserName, "Token"), new[]
            {
                new Claim(Constants.Strings.JwtClaimIdentifiers.UserId, user.Id.ToString()),
                new Claim(Constants.Strings.JwtClaimIdentifiers.UserName, user.UserName),
                new Claim(Constants.Strings.JwtClaimIdentifiers.Email, user.Email),
                new Claim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.ApiAccess)
            });

            var claims = new[]
            {
                 new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                 new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                 new Claim(JwtRegisteredClaimNames.Iat, DateTimeHelper.ConvertToUnixTimeSeconds(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                 identity.FindFirst(Constants.Strings.JwtClaimIdentifiers.UserId),
                 identity.FindFirst(Constants.Strings.JwtClaimIdentifiers.UserName),
                 identity.FindFirst(Constants.Strings.JwtClaimIdentifiers.Email),
                 identity.FindFirst(Constants.Strings.JwtClaimIdentifiers.Rol)
            };

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedToken = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedToken;
        }
        
        /// <summary>
        /// 檢查注入的 JwtIssuerOptions 物件的屬性值是否有誤
        /// </summary>
        /// <param name="options"></param>
        private void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }

        /// <summary>
        /// 要傳送到前端的 JWT 格式，User 屬性視需求帶入，其他則為必要回傳的屬性
        /// </summary>
        public class JwtView
        {
            /// <summary>
            /// 會員的個人基本資料(非必須，請視專案的需求帶入相關值)
            /// </summary>
            [JsonProperty("user")]
            public UserInfo User { get; set; }

            /// <summary>
            /// Access Token
            /// </summary>
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            /// <summary>
            /// Token 的類型
            /// </summary>
            [JsonProperty("token_type")]
            public string TokenType => "Bearer";

            /// <summary>
            /// JWT 的有效時間長度(秒)，幾秒後過期
            /// </summary>
            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }
            
            public class UserInfo
            {
                /// <summary>
                /// 會員 Id
                /// </summary>
                public Guid Id { get; set; }

                /// <summary>
                /// 會員帳號
                /// </summary>
                public string Account { get; set; }

                /// <summary>
                /// 電子信箱
                /// </summary>
                public string Email { get; set; }
            }
        }
    }
}
