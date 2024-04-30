using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Todo.Dto;
using Todo.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Todo.Controllers
{
    [Route("api/Todo/{TodoId}/UploadFile")]
    [ApiController]
    public class TodoUploadFileController : ControllerBase
    {
        private readonly TodoListContext _todoListContext;
        private readonly IMapper _iMapper;
        public TodoUploadFileController(TodoListContext todoListContext, IMapper iMapper)
        {
            _todoListContext = todoListContext;
            _iMapper = iMapper;
        }

        // GET: api/<TodoUploadFileController>
        [HttpGet]
        public ActionResult<IEnumerable<UploadFileDto>> Get(Guid TodoId)
        {
            if(!_todoListContext.TodoLists.Any(a => a.TodoId == TodoId))
            {
                return NotFound("找不到該事項");
            }
            var result = _todoListContext.UploadFiles.Where(a => a.TodoId == TodoId)
                .Select(a => new UploadFileDto
                {
                    Name = a.Name,
                    Src = a.Src,
                    TodoId = a.TodoId,
                    UploadFileId = a.UploadFileId
                }).ToList();

            //var result = from a in _todoListContext.UploadFiles
            //             where a.TodoId == TodoId
            //             select new UploadFileDto
            //             {
            //                 Name = a.Name,
            //                 Src = a.Src,
            //                 TodoId = a.TodoId,
            //                 UploadFileId = a.UploadFileId
            //             };

            if (result == null || result.Count() == 0)
            {
                return NotFound("找不到該檔案");
            }

            return Ok(result);
        }

        // GET api/<TodoUploadFileController>/5
        [HttpGet("{UploadFileId}")]
        public ActionResult<IEnumerable<UploadFileDto>> Get(Guid TodoId, Guid UploadFileId)
        {
            if (!_todoListContext.TodoLists.Any(a => a.TodoId == TodoId))
            {
                return NotFound("找不到該事項");
            }

            var result = (from a in _todoListContext.UploadFiles
                         where a.TodoId == TodoId
                         && a.UploadFileId == UploadFileId
                         select new UploadFileDto
                         {
                             Name = a.Name,
                             Src = a.Src,
                             TodoId = a.TodoId,
                             UploadFileId = a.UploadFileId
                         }).SingleOrDefault();

            if(result == null)
            {
                return NotFound("找不到檔案");
            }

            return Ok(result);
        }

        // POST api/<TodoUploadFileController>
        [HttpPost]
        public string Post(Guid TodoId, [FromBody] UploadFilePostDto value)
        {
            if(!_todoListContext.TodoLists.Any(a =>a.TodoId == TodoId))
            {
                return "找不到該事項";
            }

            UploadFile insert = new UploadFile
            {
                Name = value.Name,
                Src = value.Src,
                TodoId = TodoId
            };

            _todoListContext.UploadFiles.Add(insert);
            _todoListContext.SaveChanges();

            return "OK";
        }

        [HttpPost("AutoMapper")]
        public string PostAutoMapper(Guid TodoId, [FromBody] UploadFilePostDto value)
        {
            if (!_todoListContext.TodoLists.Any(a => a.TodoId == TodoId))
            {
                return "找不到該事項";
            }

            var map = _iMapper.Map<UploadFile>(value);
            map.TodoId = TodoId;

            _todoListContext.UploadFiles.Add(map);
            _todoListContext.SaveChanges();

            return "OK";
        }

        // PUT api/<TodoUploadFileController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TodoUploadFileController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
