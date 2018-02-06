在現實的情況下，大部分的 Web API 只允許給有使用權限的用戶呼叫以存取後端資料，而由於前端與後端 Web API 之間的連線是屬於 stateless 的，所以需要透過例如 Token-base 的方式來處理用戶驗證。
本專案將示範如何以 Jason Web Token (JWT)來處理用戶的授權驗證，只有通過驗證的用戶才能存取 Web API。

本專案乃參考 <a href="https://goblincoding.com/2016/07/03/issuing-and-authenticating-jwt-tokens-in-asp-net-core-webapi-part-i/">這篇</a> 文章實作，其中我有重構部分程式碼，且由於本專案是用 ASP.NET Core 2.0 開發，所以 Startup.cs 的一些組態設定與原文不太一樣。
另外我還有開發一些擴充方法，可以直接從 JWT 取得用戶的個人資料，這樣後端程式就可以知道目前請求的用戶是誰。

請利用 Postman 來測試：

1.取得 JWT，模擬的情境是登入成功後，後端回傳JWT到前端

POST=> http://localhost:3000/api/Account/Login

header=> Content-Type：application/json

body=> 
{
	"account": "superman",
	"password": "1234"
}

2.呼叫需要通過驗證的 WebAPI

GET=> http://localhost:3000/api/Account

header=> Authorization：Bearer {access_token}

(上面大括號的 access_token 是 JWT 的 access_token 屬性值)
