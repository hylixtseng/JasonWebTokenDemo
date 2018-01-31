using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JasonWebTokenDemo.Core
{
    public static class Constants
    {
        public static class Strings
        {
            /// <summary>
            /// Jwt 個人身份的宣告屬性名稱
            /// </summary>
            public static class JwtClaimIdentifiers
            {
                public const string Id = "id";
                public const string UserName = "uname";
                public const string Email = "email";
                public const string Rol = "rol";
            }

            /// <summary>
            /// Jwt 的宣告屬性名稱
            /// </summary>
            public static class JwtClaims
            {
                public const string ApiAccess = "api_access";
            }
        }
    }
}
