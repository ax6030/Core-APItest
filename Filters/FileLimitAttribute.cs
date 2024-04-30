using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Todo.Dto;

namespace Todo.Filters
{
    public class FileLimitAttribute : Attribute, IResultFilter
    {
        public long Size = 100000;
        public string Type;
        public void OnResultExecuted(ResultExecutedContext context)
        {
            //結束後的判斷寫在這
        }

        public FileLimitAttribute(long size, string type = "txt")
        {
            Size = size;
            Type = type;
        }
        public void OnResultExecuting(ResultExecutingContext context)
        {
            //進入時的判斷寫在這

            var files = context.HttpContext.Request.Form.Files;
            foreach(var temp in files)
            {
                if(temp.Length > (1024 * 1024 * Size))
                {
                    context.Result = new JsonResult(new ReturnJson
                    {
                        Data = "test1",
                        HttpCode = 400,
                        ErrorMessage = "檔案超過1MB，太大了無法上傳"
                    });
                    var state = context.ModelState; 
                }

                if (Path.GetExtension(temp.FileName) != Type)
                {
                    context.Result = new JsonResult(new ReturnJson
                    {
                        Data = "test2",
                        HttpCode = 400,
                        ErrorMessage = "只允許上傳" + Type
                    }) ;
                    var state = context.ModelState;

                }
            }
        }
    }
}
