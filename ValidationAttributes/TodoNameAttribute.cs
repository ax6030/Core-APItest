using System.ComponentModel.DataAnnotations;
using Todo.Dto;
using Todo.Models;

namespace Todo.ValidationAttributes
{
    public class TodoNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //DI注入資料庫物件
            TodoListContext _todoListContext = (TodoListContext)validationContext.GetService(typeof(TodoListContext));

            var name = (string)value;

            var findName = _todoListContext.TodoLists.Where(x => x.Name == name);

            var dto = validationContext.ObjectInstance;

            if(dto.GetType() == typeof(TodoListPutDto))
            {
                var dtoUpdate = (TodoListPutDto)dto;
                findName = findName.Where(x => x.TodoId != dtoUpdate.TodoId);  //排除掉自己那筆
            }
            if(findName.FirstOrDefault() != null)
            {
                return new ValidationResult("已存在相同的代辦事項");
            }

            return ValidationResult.Success;
        }
    }
}
