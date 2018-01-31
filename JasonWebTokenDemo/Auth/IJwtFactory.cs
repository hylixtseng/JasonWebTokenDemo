using System;
using System.Security.Claims;
using System.Threading.Tasks;
using JasonWebTokenDemo.Models;

namespace JasonWebTokenDemo.Auth
{
    public interface IJwtFactory
    {
        /// <summary>
        /// 產生要傳到前端的JWT
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<JwtFactory.JwtView> GenerateAsync(IdentityUser user);
    }
}
