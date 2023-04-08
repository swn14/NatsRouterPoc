using MediatR;

namespace Common.Messages;

public interface INatsMessage : IRequest
{
    public string ReplyTo { get; set; }
}