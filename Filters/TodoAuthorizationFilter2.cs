using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System.Net;
using Todo.Dto;
using Todo.Models;

namespace Todo.Filters
{
    public class TodoAuthorizationFilter2 : Attribute, IAuthorizationFilter
    {
        

        public string Roles = "";
        
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //因為要加入Roles建構值，將資料庫物件使用RequestService方式取得
            TodoListContext _todoListContext = (TodoListContext)context.HttpContext.RequestServices.GetService(typeof(TodoListContext));

            var role = _todoListContext.Roles.Where(x=>x.Name == Roles).FirstOrDefault();

            if (role == null) 
            {
                context.Result = new JsonResult(new ReturnJson()
                {
                    Data = Roles,
                    HttpCode = 401,
                    ErrorMessage = "沒有登入"
                });
            }
        }
            
    }
}
