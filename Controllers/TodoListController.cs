using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Todo.Dto;
using Todo.Filters;
using Todo.Models;
using Todo.Parameters;
using Todo.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Todo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoListController : ControllerBase
    {
        private readonly TodoListContext _todoListContext;
        private readonly TodoListService _todoListService;
        private readonly IMapper _iMapper;
        public TodoListController(TodoListContext todoListContext, TodoListService todoListService, IMapper iMapper)
        {
            _todoListContext = todoListContext;
            _todoListService = todoListService;
            _iMapper = iMapper;
        }
        // GET: api/<TodoListController>
        [HttpGet]
        //[Authorize(Roles = "select")]
        //[TodoAuthorizationFilter]  要放進去全域進行驗證     //[TypeFilter(typeof(TodoAuthorizationFilter))]  繼承Attribute 可以簡寫
        [TodoAuthorizationFilter2(Roles = "select")]
        public async Task<IActionResult> Get([FromQuery] TodoSelectParameter value)
        {
            var result = await _todoListService.取得資料(value);
            if (result == null || result.Count() <= 0)
            {
                return NotFound("找不到資源");
            }
            return Ok(result);

        }

        [HttpGet("AutoMapper")]
        //[Authorize(Roles = "automapper")]
        public IEnumerable<TodoListSelectDto> GetAutoMapper([FromQuery] TodoSelectParameter value)
        {
            return _todoListService.使用AutoMapper取得資料(value);
        }


        [HttpGet("{TodoId}")]
        //[Authorize(Roles = "nono")]

        public ActionResult<TodoListSelectDto> GetOne(Guid TodoId)
        {
            var result = _todoListService.取得單筆資料(TodoId);

            if (result == null)
            {
                return NotFound("找不到Id :" + TodoId);
            }

            return result;



        }

        // GET api/<TodoListController>/5
        [HttpGet("AutoMapper/{TodoId}")]
        public TodoListSelectDto GetAutoMapper(Guid TodoId)
        {
            return _todoListService.使用AutoMapper取得單筆資料(TodoId);
        }


        [HttpGet("From/{id}")]
        public dynamic GetFrom([FromRoute] string id, [FromQuery] string id2
           , [FromBody] string id3)
        {
            List<dynamic> result = new List<dynamic>();

            result.Add(id);
            result.Add(id2);
            result.Add(id3);

            return result;
        }

        [HttpGet("GetSQL")]
        public IEnumerable<TodoList> GetSQL(string name)
        {
            return _todoListService.使用SQL取得資料(name);
        }

        [HttpGet("GetSQLDto")]
        public IEnumerable<TodoListSelectDto> GetSQLDto(string name)
        {
            return _todoListService.使用SQLDto取得資料(name);
        }





        // POST api/<TodoListController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TodoListPostDto value)
        {
            var insert = await _todoListService.新增資料(value);

            return CreatedAtAction(nameof(GetOne), new { TodoId = insert.TodoId }, insert);
        }

        [HttpPost("up")]
        //原本的寫法，FromForm可以丟入字串進行轉換取值新增資料
        //public void PostUp([FromForm][ModelBinder(BinderType = typeof(FormDataJsonBinder))] string value, [FromForm] IFormFileCollection files)
        public void PostUp([FromForm] string value, [FromForm] IFormFileCollection files)
        //擴充方法的寫法，新增一個TodoListPostUpDto 將參數及擴充方法寫到另一個類別包裝起來
        //public void PostUp([FromForm] TodoListPostUpDto value)
        {
            //原本的轉換寫法，可以將字串轉為Json格式
            TodoList aa = JsonSerializer.Deserialize<TodoList>(value);

            //_todoListService.使用FromForm新增資料(value);

            //TodoList insert = new TodoList()
            //{
            //    InsertTime = DateTime.Now,
            //    UpdateTime = DateTime.Now,
            //    InsertEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            //    UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            //};

            //_todoListContext.TodoLists.Add(insert).CurrentValues.SetValues(value.TodoList);
            //_todoListContext.SaveChanges();



            //string rootRoot = _env.ContentRootPath + @"\wwwroot\UploadFiles\" + insert.TodoId + "\\";

            //if (!Directory.Exists(rootRoot))
            //{
            //    Directory.CreateDirectory(rootRoot);
            //}
            //foreach (var file in value.files)
            //{
            //    if (file.Length > 0)
            //    {
            //        var fileName = file.FileName;

            //        using (var stream = System.IO.File.Create(rootRoot + fileName))
            //        {
            //            file.CopyTo(stream);

            //            var insert2 = new UploadFile
            //            {
            //                Name = fileName,
            //                Src = "/UploadFiles/" + insert.TodoId + "/" + fileName,
            //                TodoId = insert.TodoId
            //            };

            //            _todoListContext.UploadFiles.Add(insert2);
            //        }
            //    }
            //}
            //_todoListContext.SaveChanges();
        }

        [HttpPost("nofk")]
        public TodoList Postnofk([FromBody] TodoListPostDto value)
        {
            return _todoListService.沒有外鍵新增父子資料(value);
        }

        [HttpPost("AutoMapper")]
        public TodoList PostAutoMapper([FromBody] TodoListPostDto value)
        {
            return _todoListService.使用AutoMapper新增資料(value);
        }

        [HttpPost("postSQL")]
        public TodoList PostSQL([FromBody] TodoListPostDto value)
        {
            return _todoListService.使用SQL新增資料(value);

        }






        // PUT api/<TodoListController>/5
        [HttpPut]
        public IActionResult Put([FromBody] TodoListPutDto value)
        {
            var update = _todoListContext.TodoLists
                .Where(x => x.TodoId == value.TodoId).SingleOrDefault();

            if (update != null)
            {
                update.UpdateTime = DateTime.Now;
                update.UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001");

                update.Name = value.Name;
                update.Orders = value.Orders;
                update.Enable = value.Enable;

                _todoListContext.SaveChanges();

                return Ok(update);
            }
            return BadRequest("找不到此TodoId");
        }

        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody] TodoListPutDto value)
        {
            if(id != value.TodoId)
            {
                return BadRequest();
            }

            if (_todoListService.修改資料(id, value) == 0)
            {
                return NotFound();
                
            }

            return NoContent();

        }

        [HttpPut("AutoMapper/{id}")]
        public IActionResult PutAutoMapper(Guid id, [FromBody] TodoListPutDto value)
        {

            if (_todoListService.使用AutoMapper修改資料(id, value) == 0)
            {
                return BadRequest("找不到此TodoId");
            }
            return NoContent();
        }






        //需要先安裝 Microsoft.AspNetCore.JsonPatch 、 Microsoft.AspNetCore.Mvc.NewtonsoftJson 套件
        [HttpPatch("{id}")]
        public IActionResult Patch(Guid id, [FromBody] JsonPatchDocument value)
        {

            if (_todoListService.修改單項資料(id, value) == 0)
            {
                return BadRequest("找不到此TodoId");
            }
            return NoContent();
        }






        // DELETE api/<TodoListController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            if(_todoListService.刪除資料(id) == 0)
            {
                return NotFound("找不到刪除的資料");
            }

            return NoContent();
        }

        [HttpDelete("nofk/{id}")]
        public IActionResult Deletenofk(Guid id)
        {
            if (_todoListService.刪除沒有外鍵資料(id) == 0)
            {
                return NotFound("找不到刪除的資料");
            }

            return NoContent();
        }
        
        //刪除多筆資料
        [HttpDelete("list/{ids}")]
        public IActionResult Deletelist(string ids)
        {
            if (_todoListService.刪除多筆資料(ids) == 0)
            {
                return NotFound("找不到刪除的資料或輸入值有問題");
            }

            return NoContent();


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
                Name = a.Name,
                Orders = a.Orders,
                TodoId = a.TodoId,
                UpdateEmployeeName = a.UpdateEmployee.Name,
                UpdateTime = a.UpdateTime,
                UploadFiles = updto
            };
        }
    }
}
