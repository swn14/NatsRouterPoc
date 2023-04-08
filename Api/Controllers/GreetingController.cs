using System.Text;
using System.Text.Json;
using Api.Requests;
using Api.Responses;
using Common.Messages;
using Microsoft.AspNetCore.Mvc;
using NATS.Client;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class GreetingController : ControllerBase
{
    private readonly IConnection _natsConnection;

    public GreetingController(IConnection natsConnection)
    {
        _natsConnection = natsConnection;
    }

    [HttpPost]
    public async Task<GreetingResponse?> SendGreeting([FromBody] GreetingRequest request)
    {

        var dataBytes = JsonSerializer.SerializeToUtf8Bytes(request);
        var msg = new Msg("default.greeting", dataBytes)
        {
            Header =
            {
                ["Message-Type"] = nameof(GreetingMessage)
            }
        };
        var responseMsg = await _natsConnection.RequestAsync(msg);
        var response = JsonSerializer.Deserialize<GreetingResponse>(responseMsg.Data);
        return response;
    }
}