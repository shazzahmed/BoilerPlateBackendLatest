public class Message
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string MessageContent { get; set; }
    public string SendMail { get; set; } = "0";
    public string SendSms { get; set; } = "0";
    public string IsGroup { get; set; } = "0";
    public string IsIndividual { get; set; } = "0";
    public int IsClass { get; set; } = 0;
    public string GroupList { get; set; }
    public string UserList { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
