using Todo.Dto;
using Todo.Parameters;

namespace Todo.Interface
{
    public interface ITodoListService
    {
        string type { get; }
        public List<TodoListSelectDto> 取得資料(TodoSelectParameter value);
    }
}
