namespace Common.DTO.Response
{
    public class MarksheetTemplateModel
    {
        public int Id { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public string TemplateType { get; set; } = "PDF";
        public string HeaderLogo { get; set; } = string.Empty;
        public string SchoolInfo { get; set; } = string.Empty;
        public string Layout { get; set; } = "{}";
        public bool IsActive { get; set; } = true;
        public bool IsDefault { get; set; } = false;
        public string Description { get; set; } = string.Empty;

        // BaseEntity properties
        public int TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }
}


