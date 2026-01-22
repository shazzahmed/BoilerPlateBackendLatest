using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class MarksheetTemplate : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string TemplateName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string TemplateType { get; set; } = "PDF"; // PDF, HTML

        [MaxLength(500)]
        public string HeaderLogo { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string SchoolInfo { get; set; } = string.Empty;

        public string Layout { get; set; } = "{}"; // JSON configuration

        public bool IsActive { get; set; } = true;

        public bool IsDefault { get; set; } = false;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}


