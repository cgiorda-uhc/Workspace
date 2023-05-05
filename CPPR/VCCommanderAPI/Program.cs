using Microsoft.EntityFrameworkCore;
using VCCommandAPI.Data.Abstract;
using VCCommandAPI.Data.Concrete;
using VCCommandAPI.Data.Context;
using VCCommandAPI.Data.Mock;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

//DEPENDENCY INJECTION AUTOMAPPER FOR DTO SUPPORT
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//ADD Secrets.json FOR UNSHARED CONNECTIONS
//builder.Configuration
//.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("Secrets.json");

//ADD DBContext
builder.Services.AddDbContext<CommandContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("CommandConnection")));
//builder.Services.AddDbContext<CommandContext>(options =>
//options.UseSqlServer("server=(local)\\CSG_LOCAL_DB;Database=CommandDB;User Id=vc_api_login;Password=Sigmund2010!!;TrustServerCertificate=True"));


//DEPENDENCY INJECTION 
//builder.Services.AddScoped<ICommandRepo, MockCommandRepo>(); 
builder.Services.AddScoped<ICommandRepo, SqlCommandRepo>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.Run();
