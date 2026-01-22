using Presentation.Filters;
using Presentation.Models;
using static Common.Utilities.Enums;
using Infrastructure.DependencyResolutions;
using Infrastructure.ServicesDependencyResolutions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var env = builder.Environment;

// Store static configuration (if needed)
StaticConfigurationProvider.StaticConfiguration = configuration;

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

// Add services to the container (ConfigureServices equivalent)
builder.Services.Configure(configuration, ApplicationType.CoreApi);
builder.Services.AddServices(configuration);
builder.Services.AddScoped<ValidateModelState>();

var app = builder.Build();

// Configure the HTTP request pipeline (Configure equivalent)
app.RegisterApps(env, ApplicationType.CoreApi);

app.Run();
