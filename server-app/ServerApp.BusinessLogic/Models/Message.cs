namespace ServerApp.BusinessLogic.Models;

public class Message
{
    public string Id { get; set; } = string.Empty;
    public string Sender { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreationTime { get; set; }

}