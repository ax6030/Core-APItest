using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Todo.Filters
{
    public class TodoActionFilter : IActionFilter
    {
        private readonly IWebHostEnvironment _env;
        public TodoActionFilter(IWebHostEnvironment env)
        {
            _env = env;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            string rootRoot = _env.ContentRootPath + @"\Log\";

            if (!Directory.Exists(rootRoot))
            {
                Directory.CreateDirectory(rootRoot);
            }

            var employeeid = context.HttpContext.User.FindFirst("EmployeeId");
            var path = context.HttpContext.Request.Path;
            var method = context.HttpContext.Request.Method;
            string text = "結束 :" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " path:" + path + " method:" + method + " " + employeeid + "\n";
            File.AppendAllText(rootRoot + DateTime.Now.ToString("yyyyMMdd") + ".txt", text);
            //也可以存在Log的資料表內insert進去  有資料表檔案的話
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            string rootRoot = _env.ContentRootPath + @"\Log\";

            if (!Directory.Exists(rootRoot))
            {
                Directory.CreateDirectory(rootRoot);
            }

            var employeeid = context.HttpContext.User.FindFirst("EmployeeId");
            var path = context.HttpContext.Request.Path;
            var method = context.HttpContext.Request.Method;
            string text = "開始 :" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " path:" + path + " method:" + method + " " + employeeid + "\n";
            File.AppendAllText(rootRoot + DateTime.Now.ToString("yyyyMMdd") + ".txt", text);
            //也可以存在Log的資料表內insert進去  有資料表檔案的話
        }
    }
}
