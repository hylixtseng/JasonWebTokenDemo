﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        /// 取得會員個人資訊
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserInfo()
        {
            var user = new
            {
                Email = "superman@mymail.com",
                UserName = "Superman"
            };

            return Ok(user);
        }
    }
}