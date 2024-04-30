using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Todo.Filters;
using Todo.Models;

namespace Todo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;   //可以找到專案的位置
        private readonly TodoListContext _todoListContext;
        public FileUploadController(IWebHostEnvironment env, TodoListContext todoListContext)
        {
            _env = env;
            _todoListContext = todoListContext;
        }
        [HttpPost]
        [FileLimit(5)]
        public async void Post([FromForm] List<IFormFile> files, [FromForm]Guid id)
        {
            string rootRoot = _env.ContentRootPath + @"\wwwroot\UploadFiles\" + id +"\\";

            if(!Directory.Exists(rootRoot))
            {
                Directory.CreateDirectory(rootRoot);
            }
            foreach(var file in files)
            {
                if (file.Length > 0)
                {
                    var fileName = file.FileName;

                    using (var stream = System.IO.File.Create(rootRoot + fileName))
                    {
                        file.CopyTo(stream);

                        var insert = new UploadFile
                        {
                            Name = fileName,
                            Src = "/UploadFiles/" + id + "/" + fileName,
                            TodoId = id
                        };

                        _todoListContext.UploadFiles.Add(insert);
                    }
                }
            }


            _todoListContext.SaveChanges();
        }
    }
}
