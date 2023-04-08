using System.Reflection;
using Consumer;
using MediatR;
using NATS.Client;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<Worker>();
        services.AddMediatR(cfg=>cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
        var connectionFactory = new ConnectionFactory();
        // Try/catch this so that tools like `dotnet swagger` don't crash the build
        try
        {
            var connection = connectionFactory.CreateConnection(context.Configuration.GetConnectionString("NATS"),
                context.Configuration.GetSection("NatsConfig").GetValue<string>("CredentialsPath"));
            services.AddSingleton(connection);
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"NATS {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    })
    .Build();

await host.RunAsync();