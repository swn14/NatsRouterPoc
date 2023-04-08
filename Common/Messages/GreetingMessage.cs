namespace Common.Messages;

public class GreetingMessage : INatsMessage
{
    public string Message { get; set; }
    public string ReplyTo { get; set; }
}