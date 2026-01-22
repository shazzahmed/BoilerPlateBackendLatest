using System.ComponentModel.DataAnnotations.Schema;
using static Common.Utilities.Enums;

namespace Domain.Entities
{
    public class Notification : BaseEntity
    {
        public int Id { get; set; }
        public NotificationTypes NotificationTypeId { get; set; }
        public string Recipient { get; set; } = string.Empty;
        public string CC { get; set; } = string.Empty;
        public string BCC { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Attachment { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public int Attempts { get; set; }       
        public DateTime ProcessedAt { get; set; }
        public string Result { get; set; } = string.Empty;


        [ForeignKey("NotificationTypeId")]
        public virtual NotificationType? NotificationType { get; set; }

        [ForeignKey("StatusId")]
        public virtual Status? Status { get; set; }
    }
}
