using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JasonWebTokenDemo.Extensions;
using JasonWebTokenDemo.Models;
using JasonWebTokenDemo.Core;

namespace JasonWebTokenDemo.Controllers
{
    [Produces("application/json")]
    [Route("api/Account")]
    public class AccountController : Controller
    {
        private readonly IJwtFactory _jwtFactory;

        public AccountController(IJwtFactory jwtFactory)
        {
            _jwtFactory = jwtFactory;
        }

        /// <summary>
        /// 會員登入，登入成功即發送 JWT
        /// POST http://localhost:3000/api/Account/Login
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Route("Login")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody]LoginInput input)
        {
            // 這裡請自行實作用戶的帳密驗證
            if (input.Account.ToLower() == "superman" && input.Password == "1234")
            {
                // 帳密驗證成功，從資料庫取得該用戶的會員資料
                var user = new IdentityUser()
                {
                    Id = Guid.NewGuid(),
                    Email = "superman@mymail.com",
                    UserName = "Superman"
                };

                // 產生 JWT
                var jwt = await _jwtFactory.GenerateAsync(user);

                return Ok(jwt);
            }

            return BadRequest("帳號或密碼輸入錯誤，請重新登入。");
        }

        /// <summary>
        /// 本 API 需要通過驗證才可以取得會員個人資訊
        /// GET http://localhost:3000/api/Account
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserInfo()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                // 程式能夠跑到 Action 內表示 Token 已經驗證成功，所以通常不需要再用 IsAuthenticated 屬性去判斷是否驗證成功
                var name = this.User.Identity.Name;
            }

            // 因為我們有在 JWT 塞一些會員個人資料，所以可以透過以下擴充方法取得這些資料
            var userId = this.User.GetUserId();
            var userName = this.User.GetUserName();
            var email = this.User.GetEmail();
            
            var user = new
            {
                UserId = userId,
                UserName = userName,
                Email = email
            };

            return Ok(user);
        }
    }
}