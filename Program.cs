using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Todo.Filters;
using Todo.Interface;
using Todo.Models;
using Todo.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//建立資料庫連線字串
//Scaffold-DbContext "Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\JasonH\DB\TodoList.mdf;Integrated Security=True;Connect Timeout=30" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Force -CoNtext TodoListContext

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TodoListContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("TodoListDatabase")));
//AutoMapper DI注入
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//TodoService DI注入
builder.Services.AddScoped<TodoListService>();

builder.Services.AddScoped<ITodoListService, TodoLinqService>();
builder.Services.AddScoped<ITodoListService, TodoAutoMapperService>();

//cooclie驗證
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
//{
//    //未登入時會自動導到這個網址
//    option.LoginPath = new PathString("/api/Login/NoLogin");
//    //沒有權限會導到這個網址
//    option.AccessDeniedPath = new PathString("/api/Login/NoAccess");
//    //權限通過驗證時間
//    //option.ExpireTimeSpan = TimeSpan.FromSeconds(2);
//});

//JWT驗證環境設定
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,     //以上其實可以不用設定
        ClockSkew = TimeSpan.Zero,  //讓憑證準確時間失效
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:KEY"]))
    };
});

//全部API都需要經過驗證
builder.Services.AddMvc(option =>
{
    option.Filters.Add(new AuthorizeFilter());
    //option.Filters.Add(new TodoAuthorizationFilter());

    option.Filters.Add(typeof(TodoActionFilter));

   // option.Filters.Add(new TodoResultFilter());
});

//注入後可以在其他地方使用HttpContext取用驗證資料
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseRouting();

app.UseStaticFiles();  //開啟靜態檔案目錄

//Cookie驗證設定
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();


app.Run();
