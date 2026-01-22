using System.ComponentModel.DataAnnotations.Schema;
using static Common.Utilities.Enums;

namespace Domain.Entities
{
    public class Status
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public StatusTypes TypeId { get; set; }

        [ForeignKey("TypeId")]
        public virtual StatusType? StatusType { get; set; }
    }
}
