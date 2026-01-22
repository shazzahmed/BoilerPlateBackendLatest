public class ChatMessage
{
    public int Id { get; set; }
    public string Message { get; set; }
    public int ChatUserId { get; set; }
    public string Ip { get; set; }
    public int Time { get; set; }
    public bool IsFirst { get; set; }
    public bool IsRead { get; set; }
    public int ChatConnectionId { get; set; }
    public DateTime? CreatedAt { get; set; }
}
