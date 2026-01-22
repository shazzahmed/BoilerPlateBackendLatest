using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Utilities;
using static Common.Utilities.Enums;

namespace Domain.Entities
{
    public class NotificationTemplate
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public NotificationTemplates Id { get; set; }

        public NotificationTypes NotificationTypeId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string MessageBody { get; set; } = string.Empty;

        [ForeignKey("NotificationTypeId")]
        public virtual NotificationType NotificationTypes { get; set; }
    }
}
