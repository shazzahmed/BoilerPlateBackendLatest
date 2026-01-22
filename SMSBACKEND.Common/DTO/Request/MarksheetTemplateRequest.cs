using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class MarksheetTemplateRequest
    {
        public PaginationParams PaginationParam { get; set; } = new PaginationParams();
        public bool? IsActive { get; set; }
        public string TemplateType { get; set; } = string.Empty;
    }

    public class MarksheetTemplateCreateRequest
    {
        [Required]
        [MaxLength(200)]
        public string TemplateName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string TemplateType { get; set; } = "PDF";

        [MaxLength(500)]
        public string HeaderLogo { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string SchoolInfo { get; set; } = string.Empty;

        public string Layout { get; set; } = "{}";

        public bool IsActive { get; set; } = true;

        public bool IsDefault { get; set; } = false;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
    }

    public class MarksheetTemplateUpdateRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string TemplateName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string TemplateType { get; set; }

        [MaxLength(500)]
        public string HeaderLogo { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string SchoolInfo { get; set; } = string.Empty;

        public string Layout { get; set; } = "{}";

        public bool IsActive { get; set; }

        public bool IsDefault { get; set; }

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}


