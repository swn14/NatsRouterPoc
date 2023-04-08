using System.Text.Json;
using Api.Responses;
using Common.Messages;
using MediatR;
using NATS.Client;

namespace Consumer.Handlers;

public class GreetingHandler : IRequestHandler<GreetingMessage>
{
    private readonly ILogger<GreetingHandler> _logger;
    private readonly IConnection _natsConnection;

    public GreetingHandler(ILogger<GreetingHandler> logger, IConnection natsConnection)
    {
        _logger = logger;
        _natsConnection = natsConnection;
    }

    public Task Handle(GreetingMessage request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Here is the greeting: {Message}", request.Message);
        var response = new GreetingResponse()
        {
            Message = request.Message,
            TransactionId = Guid.NewGuid().ToString()
        };
        var replyMsg = new Msg(request.ReplyTo, JsonSerializer.SerializeToUtf8Bytes(response));
        _natsConnection.Publish(replyMsg);
        return Unit.Task;
    }
}