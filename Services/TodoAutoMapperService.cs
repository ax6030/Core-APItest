using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Todo.Dto;
using Todo.Interface;
using Todo.Models;
using Todo.Parameters;

namespace Todo.Services
{
    public class TodoAutoMapperService : ITodoListService
    {
        private readonly TodoListContext _todoListContext;
        private readonly IMapper _iMapper;
        public TodoAutoMapperService(TodoListContext todoListContext, IMapper mapper)
        {
            _todoListContext = todoListContext;
            _iMapper = mapper;
        }

        public string type => "automapper";

        public List<TodoListSelectDto> 取得資料(TodoSelectParameter value)
        {
            //有設定外鍵include取資料
            var result = _todoListContext.TodoLists
                .Include(a => a.UpdateEmployee)
                .Include(a => a.InsertEmployee)
                .Include(a => a.UploadFiles)
                        .Select(a => a);
            if (!string.IsNullOrWhiteSpace(value.name))
            {
                result = result.Where(a => a.Name.Contains(value.name));
            }
            if (value.enable != null)
            {
                result = result.Where(a => a.Enable == value.enable);
            }
            if (value.InsertTime != null)
            {
                result = result.Where(a => a.InsertTime.Date == value.InsertTime);
            }
            if (value.minOrder != null && value.maxOrder != null)
            {
                result = result.Where(a => a.Orders >= value.minOrder && a.Orders <= value.maxOrder);
            }
            return _iMapper.Map<IEnumerable<TodoListSelectDto>>(result).ToList();
        }
    }
}
