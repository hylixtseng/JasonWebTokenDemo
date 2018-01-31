using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JasonWebTokenDemo.Models
{
    public class LoginInput
    {
        /// <summary>
        /// 帳號
        /// </summary>
        [Required(ErrorMessage = "帳號欄位請不要空白。")]
        public string Account { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        [Required(ErrorMessage = "密碼欄位請不要空白。")]
        public string Password { get; set; }
    }
}
