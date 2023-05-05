using Microsoft.EntityFrameworkCore;
using VCWebPortal.API.Data;

//AZURE URL!!!!!!
//https://azuredevops.optum.com/tfs/UHG


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//INJECT DBCONTEXT
builder.Services.AddDbContext<VCWebPortalTestDbContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("VCWebPortalTestDbConnectionString")));


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment()) //CSG ALWAYS KEEP THESE DOCUMENTATIONS AVAILABLE FOR USERS!
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}


//FORCE HTTP TO HTTPS REQUESTS
app.UseHttpsRedirection();

app.UseAuthorization();

//LOOK FOR CONTROLLERS AND ROUTE PROPER PATHS
app.MapControllers();

app.Run();
