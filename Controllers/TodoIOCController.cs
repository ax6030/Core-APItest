using Microsoft.AspNetCore.Mvc;
using Todo.Dto;
using Todo.Interface;
using Todo.Parameters;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Todo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoIOCController : ControllerBase
    {
        //注入多個實作的服務
        private readonly IEnumerable<ITodoListService> _todoListService;

        public TodoIOCController(IEnumerable<ITodoListService> todoListService)
        {
            _todoListService = todoListService;
        }
        // GET: api/<TodoIOCController>
        [HttpGet]
        public IEnumerable<TodoListSelectDto> Get([FromQuery] TodoSelectParameter value)
        {
            //區別兩種Service，並使用此Service取得資料
            ITodoListService _todo;
            if(value.type == "fun")
            {
                _todo = _todoListService.Where(a => a.type == "fun").Single();
            }
            else
            {
                _todo = _todoListService.Where(a => a.type == "automapper").Single();
            }

            return _todo.取得資料(value);
        }

        
        
    }
}
