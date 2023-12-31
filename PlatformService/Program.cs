using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
if(builder.Environment.IsProduction())
{
    Console.WriteLine("---> Using Sql-Server Db");
    var connectionString= builder.Configuration.GetConnectionString("PlatformsConn");
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connectionString));
}
else
{
    Console.WriteLine("---> Using InMem Db");
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
}
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddGrpc();
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.Configure<RabbitMQOptions>(builder.Configuration.GetSection(RabbitMQOptions.RABBIT_MQ));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var sp=builder.Services.BuildServiceProvider();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Configure the HTTP request pipeline.
app.MapGrpcService<GrpcPlatformService>();
app.MapGet("/protos/platform.proto", async context =>
{
    await context.Response.WriteAsync(System.IO.File.ReadAllText("Protos/platforms.proto"));
} );
PrepDb.PrepPopulation(sp, builder.Environment.IsProduction());

app.Run();
