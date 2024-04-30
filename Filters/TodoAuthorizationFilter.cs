using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System.Net;
using Todo.Dto;

namespace Todo.Filters
{
    public class TodoAuthorizationFilter : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool tokenFlag = context.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues outValue);

            //自行寫的Filter 因為不是內建 需要特別忽略 AllowAnonymousAttribute 才能進入為登入頁面
            var ignore = context.ActionDescriptor.EndpointMetadata
                .Where(x => x.GetType() == typeof(AllowAnonymousAttribute)).FirstOrDefault();

            if(ignore == null) 
            {
                if (tokenFlag)
                {
                    //可以放入解析JWT Token程式碼驗證

                    if (outValue != "123")
                    {
                        context.Result = new JsonResult(new ReturnJson()
                        {
                            Data = "test1",
                            HttpCode = 401,
                            ErrorMessage = "沒有登入"
                        });
                    }
                }
                else
                {
                    context.Result = new JsonResult(new ReturnJson()
                    {
                        Data = "test2",
                        HttpCode = 401,
                        ErrorMessage = "沒有登入"
                    });
                }
            }
        }
            
    }
}
