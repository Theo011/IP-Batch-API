using IP_Batch_API.DbContexts;
using IP_Batch_API.Services;
using IPInfoProvider;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddMemoryCache();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IIPBatchRepository, IPBatchRepository>();
// set ipstackapikey in appsettings.json or environment variables
builder.Services.AddSingleton<IIPInfoProvider>(p => new IPInfoProvider.IPInfoProvider(configuration.GetValue<string>("ipstackapikey")));
// set dbconnectionstring in appsettings.json or environment variables and also create the database in SQL Server
builder.Services.AddDbContext<IPBatchAPIDbContext>(DbContextOptions => DbContextOptions.UseSqlServer(configuration.GetValue<string>("dbconnectionstring")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();