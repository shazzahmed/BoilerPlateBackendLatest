using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Common.Utilities.Enums;

namespace Domain.Entities
{
    public class NotificationType
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public NotificationTypes Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
