using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using Todo.Abstracts;
using Todo.Models;
using Todo.ValidationAttributes;

namespace Todo.Dto
{
    //包裝類別
    public class TodoListPostUpDto
    {
        [ModelBinder(BinderType = typeof(FormDataJsonBinder))]
        public TodoListPostDto TodoList {  get; set; }
        public IFormFileCollection files { get; set; }
    }
}
