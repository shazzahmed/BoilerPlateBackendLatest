public class NotificationSetting
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string IsMail { get; set; } = "0";
    public string IsSms { get; set; } = "0";
    public int IsNotification { get; set; } = 0;
    public int DisplayNotification { get; set; } = 0;
    public int DisplaySms { get; set; } = 1;
    public string Template { get; set; }
    public string Variables { get; set; }
    public DateTime CreatedAt { get; set; }
}
