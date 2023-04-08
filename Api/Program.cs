using NATS.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connectionFactory = new ConnectionFactory();
// Try/catch this so that tools like `dotnet swagger` don't crash the build
try
{
    var connection = connectionFactory.CreateConnection(builder.Configuration.GetConnectionString("NATS"),
        builder.Configuration.GetSection("NatsConfig").GetValue<string>("CredentialsPath"));
    builder.Services.AddSingleton(connection);
}
catch (Exception ex)
{
    Console.WriteLine($@"NATS {ex.Message}");
    Console.WriteLine(ex.StackTrace);
}

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