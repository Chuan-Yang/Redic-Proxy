using RedisProxy.Service.Services.Cache;
using RedisProxy.Service.Services.Cache.LRU;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var redisConfig = new ConfigurationOptions
{
    EndPoints =
    {
        { builder.Configuration["Redis:Address"], int.Parse(builder.Configuration["Redis:Port"]) },
    },
    Password = builder.Configuration["Redis:Password"]
};

var server = ConnectionMultiplexer.Connect(redisConfig);
builder.Services.AddSingleton<IConnectionMultiplexer>(server);

builder.Services.AddSingleton<ICache, LRUCache>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
