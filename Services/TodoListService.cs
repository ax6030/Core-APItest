using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Text.Json;
using Todo.Dto;
using Todo.Models;
using Todo.Parameters;

namespace Todo.Services
{
    public class TodoListService
    {
        private readonly TodoListContext _todoListContext;
        private readonly IMapper _iMapper;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TodoListService(TodoListContext todoListContext, IMapper iMapper, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _todoListContext = todoListContext;
            _iMapper = iMapper;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<TodoListSelectDto>> 取得資料(TodoSelectParameter value)
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

            var temp = await result.ToListAsync();

            return  temp.Select(a => ItemToDto(a)).ToList();
        }

        public TodoListSelectDto 取得單筆資料(Guid TodoId)
        {
            var result = (from a in _todoListContext.TodoLists
                          where a.TodoId == TodoId
                          select a)
                          .Include(a => a.InsertEmployee)
                          .Include(a => a.UpdateEmployee)
                          .Include(a => a.UploadFiles)
                          .SingleOrDefault();

            if(result != null)
            {
                return ItemToDto(result);
            }
            
            return null;

            //子母資料一起取
            //var result = (from a in _todoListContext.TodoLists
            //              where a.TodoId == id
            //              select new TodoListSelectDto
            //              {
            //                  Enable = a.Enable,
            //                  InsertEmployeeName = a.InsertEmployee.Name,
            //                  InsertTime = a.InsertTime,
            //                  Name = a.Name,
            //                  Orders = a.Orders,
            //                  TodoId = a.TodoId,
            //                  UpdateEmployeeName = a.UpdateEmployee.Name,
            //                  UpdateTime = a.UpdateTime,
            //                  UploadFiles = (from b in _todoListContext.UploadFiles
            //                                 where a.TodoId == b.TodoId
            //                                 select new UploadFileDto
            //                                 {
            //                                     Name = b.Name,
            //                                     Src = b.Src,
            //                                     TodoId = b.TodoId,
            //                                     UploadFileId = b.UploadFileId
            //                                 }).ToList()
            //              }).SingleOrDefault();
            //return Ok(result);
        }

        public IEnumerable<TodoListSelectDto> 使用AutoMapper取得資料(TodoSelectParameter value)
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
            return _iMapper.Map<IEnumerable<TodoListSelectDto>>(result);
        }

        public TodoListSelectDto 使用AutoMapper取得單筆資料(Guid TodoId)
        {
            //沒設定外鍵要用join加入取資料表
            var result = (from a in _todoListContext.TodoLists
                          where a.TodoId == TodoId
                          select a).Include(a => a.UpdateEmployee).Include(a => a.InsertEmployee).SingleOrDefault();

            return _iMapper.Map<TodoListSelectDto>(result);
        }

        public IEnumerable<TodoList> 使用SQL取得資料(string name)
        {
            var sql = "select * from todolist where 1=1";

            if (!string.IsNullOrWhiteSpace(name))
            {
                sql += "and name like N'%" + name + "%'";
            }

            return _todoListContext.TodoLists.FromSqlRaw(sql);
        }

        public IEnumerable<TodoListSelectDto> 使用SQLDto取得資料(string name)
        {
            var sql = @"SELECT [TodoId]
,a.[Name]
,[InsertTime]
,[UpdateTime]
,[Enable]
,[Orders]
,b.Name as InsertEmployeeName
,c.Name as UpdateEmployeeName
From [TodoList] a
join Employee b on a.InsertEmployeeId=b.EmployeeId
join Employee c on a.UpdateEmployeeId=c.EmployeeId where 1=1";

            if (!string.IsNullOrWhiteSpace(name))
            {
                sql += "and a.name like N'%" + name + "%'";
            }
            //正常寫法加入DTO欄位取值對應
            //var result = _todoListContext.TodoListSelectDto.FromSqlRaw(sql);

            //大神建立的方法使用
            return _todoListContext.ExecSQL<TodoListSelectDto>(sql);

        }



        public async Task<TodoList> 新增資料(TodoListPostDto value)
        {
            //第一種取得使用者資料寫法
            //var Claim = _httpContextAccessor.HttpContext.User.Claims.ToList();
            //var employeeId = Claim.Where(x => x.Type == "EmployeeId").First().Value;
            //第二種取得使用者資料寫法
            var employeeId = _httpContextAccessor.HttpContext.User.FindFirstValue("EmployeeId");

            var email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            TodoList insert = new TodoList()
            {
                InsertTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                InsertEmployeeId = Guid.Parse(employeeId),
                UpdateEmployeeId = Guid.Parse(employeeId),
            };

            _todoListContext.TodoLists.Add(insert).CurrentValues.SetValues(value);   //內建函式自動對應資料
            _todoListContext.SaveChanges();

            foreach (var temp in value.UploadFiles)
            {
                _todoListContext.UploadFiles.Add(new UploadFile
                {
                    TodoId = insert.TodoId
                }).CurrentValues.SetValues(temp);            //內建函式自動對應資料
            }

            await _todoListContext.SaveChangesAsync();

            return insert;


            //手動撰寫相對應格式進入
            //List<UploadFile> up1 = new List<UploadFile>();

            //foreach (var temp in value.UploadFiles)
            //{
            //    UploadFile up = new UploadFile()
            //    {
            //        Name = temp.Name,
            //        Src = temp.Src
            //    };
            //    up1.Add(up);
            //}

            ////轉譯篩選讓使用者始能輸入部分資料，系統自動給資料
            //TodoList insert = new TodoList()
            //{
            //    Name = value.Name,
            //    Enable = value.Enable,
            //    Orders = value.Orders,
            //    InsertTime = DateTime.Now,
            //    UpdateTime = DateTime.Now,
            //    InsertEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            //    UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            //    UploadFiles = up1      //有設定外鍵可以直接關聯新增值
            //};

            //_todoListContext.TodoLists.Add(insert);
            //_todoListContext.SaveChanges();
            //return "value";
        }

        public void 使用FromForm新增資料([FromForm]TodoListPostUpDto value)
        {
            TodoList insert = new TodoList()
            {
                InsertTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                InsertEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            };

            _todoListContext.TodoLists.Add(insert).CurrentValues.SetValues(value.TodoList);   
            _todoListContext.SaveChanges();

           

            string rootRoot = _env.ContentRootPath + @"\wwwroot\UploadFiles\" + insert.TodoId + "\\";

            if (!Directory.Exists(rootRoot))
            {
                Directory.CreateDirectory(rootRoot);
            }
            foreach (var file in value.files)
            {
                if (file.Length > 0)
                {
                    var fileName = file.FileName;

                    using (var stream = System.IO.File.Create(rootRoot + fileName))
                    {
                        file.CopyTo(stream);

                        var insert2 = new UploadFile
                        {
                            Name = fileName,
                            Src = "/UploadFiles/" + insert.TodoId + "/" + fileName,
                            TodoId = insert.TodoId
                        };

                        _todoListContext.UploadFiles.Add(insert2);
                    }
                }
            }
            _todoListContext.SaveChanges();
        }

        public TodoList 沒有外鍵新增父子資料(TodoListPostDto value)
        {
            //轉譯篩選讓使用者始能輸入部分資料，系統自動給資料
            TodoList insert = new TodoList()
            {
                Name = value.Name,
                Enable = value.Enable,
                Orders = value.Orders,
                InsertTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                InsertEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            };

            _todoListContext.TodoLists.Add(insert);
            _todoListContext.SaveChanges();

            //沒設外鍵要輸入新增子資料
            foreach (var temp in value.UploadFiles)
            {
                UploadFile insert2 = new UploadFile
                {
                    Name = temp.Name,
                    Src = temp.Src,
                    TodoId = insert.TodoId
                };
                _todoListContext.UploadFiles.Add(insert2);
            }

            _todoListContext.SaveChanges();

            return insert;
        }

        public TodoList 使用AutoMapper新增資料(TodoListPostDto value)
        {
            var map = _iMapper.Map<TodoList>(value);

            map.InsertTime = DateTime.Now;
            map.UpdateTime = DateTime.Now;
            map.InsertEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            map.UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001");

            _todoListContext.TodoLists.Add(map);
            _todoListContext.SaveChanges();

            return map;
        }

        public TodoList 使用SQL新增資料(TodoListPostDto value)
        {
            //防止SQL Injection，參數化的寫法(加入變數使用變數值丟入，相對應)
            var name = new SqlParameter("name", value.Name);

            string sql = @"INSERT INTO [dbo].[TodoList]
           ([Name]
           ,[InsertTime]
           ,[UpdateTime]
           ,[Enable]
           ,[Orders]
           ,[InsertEmployeeId]
           ,[UpdateEmployeeId])
     VALUES
           (@name, '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + value.Enable + "', '" + value.Orders + "', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001')";

            var temp = _todoListContext.Database.ExecuteSqlRaw(sql, name);
            _todoListContext.SaveChanges();

            var result = _todoListContext.TodoLists.Where(x => x.Name == value.Name).FirstOrDefault();

            return result;
        }




        public int 修改資料(Guid id, TodoListPutDto value)
        {
            //第一種更新方法
            //_todoListContext.Entry(value).State = EntityState.Modified;
            //_todoListContext.Update(value);   // 第二種更新方法
            //_todoListContext.SaveChanges();

            //var update = _todoListContext.TodoLists.Find(id);  //id是主Key才能這樣用
            var update = _todoListContext.TodoLists
                .Where(x => x.TodoId == id).SingleOrDefault();

            if (update != null)
            {
                update.UpdateTime = DateTime.Now;
                update.UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001");

                //update.Name = value.Name;
                //update.Orders = value.Orders;
                //update.Enable = value.Enable;

                //使用內建函示進行自動對應
                _todoListContext.TodoLists.Update(update).CurrentValues.SetValues(value);
            }
            return _todoListContext.SaveChanges();
        }

        public int 使用AutoMapper修改資料(Guid id, TodoListPutDto value)
        {
            //var update = _todoListContext.TodoLists.Find(id);  //id是主Key才能這樣用

            var update = _todoListContext.TodoLists
                .Where(x => x.TodoId == value.TodoId).SingleOrDefault();

            if (update != null)
            {
                _iMapper.Map(value, update);
            }
            return _todoListContext.SaveChanges();
        }

        public int 修改單項資料(Guid id, JsonPatchDocument value)
        {
            var update = _todoListContext.TodoLists
                .Where(x => x.TodoId == id).SingleOrDefault();

            if (update != null)
            {
                update.UpdateTime = DateTime.Now;
                update.UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001");
                value.ApplyTo(update);
            }
            return _todoListContext.SaveChanges();
        }



        public int 刪除資料(Guid id)
        {
            var delete = _todoListContext.TodoLists
               .Where(x => x.TodoId == id).Include(x => x.UploadFiles).SingleOrDefault();
            if (delete != null)
            {
                _todoListContext.Remove(delete);
            }

            return _todoListContext.SaveChanges(true);
        }

        public int 刪除沒有外鍵資料(Guid id)
        {
            var child = _todoListContext.UploadFiles
                .Where(x => x.TodoId == id);
            if (child != null)
            {
                _todoListContext.UploadFiles.RemoveRange(child);
                _todoListContext.SaveChanges();
            }

            var delete = _todoListContext.TodoLists
                .Where(x => x.TodoId == id).SingleOrDefault();
            if (delete != null)
            {
                _todoListContext.Remove(delete);
                
            }
            return _todoListContext.SaveChanges();
        }

        public int 刪除多筆資料(string ids)
        {
            //字串格式轉換為Guid清單
            var deleteList = JsonSerializer.Deserialize<List<Guid>>(ids);

            var delete = _todoListContext.TodoLists
                .Where(x => deleteList.Contains(x.TodoId)).Include(x => x.UploadFiles);
            if (delete != null)
            {
                _todoListContext.RemoveRange(delete);
                
            }
            return _todoListContext.SaveChanges();
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
