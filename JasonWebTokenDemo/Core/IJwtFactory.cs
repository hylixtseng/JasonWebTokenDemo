using System;
using System.Threading.Tasks;
using JasonWebTokenDemo.Models;

namespace JasonWebTokenDemo.Core
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
