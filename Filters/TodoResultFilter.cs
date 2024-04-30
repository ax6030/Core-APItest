using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Todo.Dto;

namespace Todo.Filters
{
    public class TodoResultFilter : IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
            
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            var contextResult = context.Result as ObjectResult;

            if(context.ModelState.IsValid)
            {
                context.Result = new JsonResult(new ReturnJson
                {
                    Data = contextResult.Value,
                    User = "kai"
                });
            }
            else
            {
                context.Result = new JsonResult(new ReturnJson
                {
                    Error = contextResult.Value,
                    User = "kai"
                });
            }

            
        }
    }
}
