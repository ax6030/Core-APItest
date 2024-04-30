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
//�إ߸�Ʈw�s�u�r��
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
//AutoMapper DI�`�J
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//TodoService DI�`�J
builder.Services.AddScoped<TodoListService>();

builder.Services.AddScoped<ITodoListService, TodoLinqService>();
builder.Services.AddScoped<ITodoListService, TodoAutoMapperService>();

//cooclie����
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
//{
//    //���n�J�ɷ|�۰ʾɨ�o�Ӻ��}
//    option.LoginPath = new PathString("/api/Login/NoLogin");
//    //�S���v���|�ɨ�o�Ӻ��}
//    option.AccessDeniedPath = new PathString("/api/Login/NoAccess");
//    //�v���q�L���Үɶ�
//    //option.ExpireTimeSpan = TimeSpan.FromSeconds(2);
//});

//JWT�������ҳ]�w
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,     //�H�W���i�H���γ]�w
        ClockSkew = TimeSpan.Zero,  //�����ҷǽT�ɶ�����
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:KEY"]))
    };
});

//����API���ݭn�g�L����
builder.Services.AddMvc(option =>
{
    option.Filters.Add(new AuthorizeFilter());
    //option.Filters.Add(new TodoAuthorizationFilter());

    option.Filters.Add(typeof(TodoActionFilter));

   // option.Filters.Add(new TodoResultFilter());
});

//�`�J��i�H�b��L�a��ϥ�HttpContext�������Ҹ��
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

app.UseStaticFiles();  //�}���R�A�ɮץؿ�

//Cookie���ҳ]�w
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();


app.Run();
