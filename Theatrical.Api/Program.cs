using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using Theatrical.Data.Context;
using Theatrical.Services;
using Theatrical.Services.PerformersService;
using Theatrical.Services.Repositories;
using Theatrical.Services.Validation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson(o =>
{
    o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//dbconnection
builder.Services.AddDbContext<TheatricalPlaysDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("ConnString")));

//services registering
builder.Services.AddTransient<IPerformerRepository, PerformerRepository>();
builder.Services.AddTransient<IPerformerService, PerformerService>();
builder.Services.AddTransient<IPerformerValidationService, PerformerValidationService>();

builder.Services.AddTransient<IRoleRepository, RoleRepository>();
builder.Services.AddTransient<IRoleService, RoleService>();
builder.Services.AddTransient<IRoleValidationService, RoleValidationService>();

//Serilog Console log styling
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();