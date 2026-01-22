public class ChatConnection
{
    public int Id { get; set; }
    public int ChatUserOne { get; set; }
    public int ChatUserTwo { get; set; }
    public string Ip { get; set; }
    public int? Time { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
