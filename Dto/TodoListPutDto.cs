using System.ComponentModel.DataAnnotations;
using Todo.Abstracts;
using Todo.ValidationAttributes;

namespace Todo.Dto
{
    public class TodoListPutDto : TodoListEditDtoAbstract
    {
        public Guid TodoId { get; set; }
    }
}
