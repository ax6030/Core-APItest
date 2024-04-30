using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Todo.Dto;
using Todo.Interface;
using Todo.Models;
using Todo.Parameters;

namespace Todo.Services
{
    public class TodoLinqService : ITodoListService
    {
        private readonly TodoListContext _todoListContext;

        public string type => "fun";

        public TodoLinqService(TodoListContext todoListContext)
        {
            _todoListContext = todoListContext;
        }
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

            return result.ToList().Select(a => ItemToDto(a)).ToList();
        }

        private static TodoListSelectDto ItemToDto(TodoList a)
        {
            List<UploadFileDto> updto = new List<UploadFileDto>();

            foreach (var temp in a.UploadFiles)
            {
                UploadFileDto up = new UploadFileDto
                {
                    Name = temp.Name,
                    Src = temp.Src,
                    TodoId = temp.TodoId,
                    UploadFileId = temp.UploadFileId
                };
                updto.Add(up);
            }

            return new TodoListSelectDto
            {
                Enable = a.Enable,
                InsertEmployeeName = a.InsertEmployee.Name,
                InsertTime = a.InsertTime,
                Name = a.Name + " (use fun) ",
                Orders = a.Orders,
                TodoId = a.TodoId,
                UpdateEmployeeName = a.UpdateEmployee.Name,
                UpdateTime = a.UpdateTime,
                UploadFiles = updto
            };
        }
    }
}
