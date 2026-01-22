using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    /// <summary>
    /// Represents reasons for disabling/deactivating students or staff
    /// </summary>
    public class DisableReason : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Reason { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}
