using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    /// <summary>
    /// Represents a specific permission within a module
    /// Used for granular access control (View, Create, Edit, Delete)
    /// </summary>
    public class Permission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public int ModuleId { get; set; }

        public virtual Module Module { get; set; } = null!;
    }
}