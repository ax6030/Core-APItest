using System.ComponentModel.DataAnnotations;
using Todo.Dto;
using Todo.Models;

namespace Todo.Abstracts
{
    public abstract class TodoListEditDtoAbstract : IValidatableObject
    {
        public string Name { get; set; } = null!;

        public bool Enable { get; set; }

        public int Orders { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public List<UploadFilePostDto> UploadFiles { get; set; }

        public TodoListEditDtoAbstract()
        {
            UploadFiles = new List<UploadFilePostDto>();
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //DI注入資料庫物件
            TodoListContext _todoListContext = (TodoListContext)validationContext.GetService(typeof(TodoListContext));


            var findName = _todoListContext.TodoLists.Where(x => x.Name == Name);

            if (this.GetType() == typeof(TodoListPutDto))
            {
                var dtoUpdate = (TodoListPutDto)this;
                findName = findName.Where(x => x.TodoId != dtoUpdate.TodoId);  //排除掉自己那筆
            }

            if (findName.FirstOrDefault() != null)
            {
                yield return new ValidationResult("已存在相同的代辦事項", new string[] { "Name" });
            }

            if (StartTime >= EndTime)
            {
                yield return new ValidationResult("開始時間不能大於結束時間", new string[] { "Time" });
            }
        }
    }
}
