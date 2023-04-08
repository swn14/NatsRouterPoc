using System.Text.Json;
using Common.Messages;
using MediatR;
using NATS.Client;

namespace Consumer;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConnection _natsConnection;
    private readonly IMediator _mediator;

    public Worker(ILogger<Worker> logger, IConnection natsConnection, IMediator mediator)
    {
        _logger = logger;
        _natsConnection = natsConnection;
        _mediator = mediator;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subjectNamespace = "default";
        var natsRouterSubscriptions = _natsConnection.SubscribeAsync(
            $"{subjectNamespace}.>",
            "NatsRouterQueue",
            async (_, args) =>
            {
                var messageType = args.Message.Header["Message-Type"];
                var type = Type.GetType($"Common.Messages.{messageType}, Common");
                if (type is null)
                {
                    _logger.LogWarning("Unknown Message Type: {MessageType}", messageType);
                }
                else
                {
                    var message = (INatsMessage)JsonSerializer.Deserialize(args.Message.Data, type!)!;
                    message.ReplyTo = args.Message.Reply;
                    await _mediator.Send(message, stoppingToken);
                }
            }
        );
        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}