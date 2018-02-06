using System;
using System.Security.Claims;
using JasonWebTokenDemo.Core;

namespace JasonWebTokenDemo.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        /// <summary>
        /// 取得當前 Token 的 UserId
        /// </summary>
        /// <param name="claim"></param>
        /// <returns></returns>
        public static Guid GetUserId(this ClaimsPrincipal claim)
        {
            var id = claim.FindFirst(Constants.Strings.JwtClaimIdentifiers.UserId).Value;
            return new Guid(id);
        }

        /// <summary>
        /// 取得當前 Token 的 UserName
        /// </summary>
        /// <param name="claim"></param>
        /// <returns></returns>
        public static string GetUserName(this ClaimsPrincipal claim)
        {
            return claim.FindFirst(Constants.Strings.JwtClaimIdentifiers.UserName).Value;
        }

        /// <summary>
        /// 取得當前 Token 的 Email
        /// </summary>
        /// <param name="claim"></param>
        /// <returns></returns>
        public static string GetEmail(this ClaimsPrincipal claim)
        {
            // 用以下註解的這行程式碼會找不到 "email" Claim Type
            //return claim.FindFirst(Helpers.Constants.Strings.JwtClaimIdentifiers.Email).Value;

            // JWT 會將 "email" Claim Type 自動轉為以下url字串的 Claim Type，只有小寫 "email" 才會自動轉，首字母大寫的 "Email" 則不會！ 
            return claim.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
        }
    }
}
