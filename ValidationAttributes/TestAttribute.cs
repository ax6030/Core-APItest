using System.ComponentModel.DataAnnotations;
using Todo.Dto;
using Todo.Models;

namespace Todo.ValidationAttributes
{
    public class TestAttribute : ValidationAttribute
    {
        private string _tvalue;
        public string Tvalue = "de1";
        public TestAttribute(string tvalue = "de") 
        { 
            _tvalue = tvalue;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var st = (TodoListPostDto)value;
            return new ValidationResult(Tvalue, new string[] { "tvalue" });
        }
    }
}
