using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class BulkEnquiryImportRequest
    {
        [Required]
        public List<EnquiryImportRow> Enquiries { get; set; } = new List<EnquiryImportRow>();

        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        public bool ValidateOnly { get; set; } = false;
        public bool OverwriteExisting { get; set; } = false;
    }

    public class EnquiryImportRow
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime? Dob { get; set; }
        public int ApplyingClass { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Source { get; set; } = "Import";
        public string? Notes { get; set; }
        public string Status { get; set; } = "New";
        
        // Row validation
        public int RowNumber { get; set; }
        public List<string> ValidationErrors { get; set; } = new List<string>();
        public bool IsValid { get; set; } = true;
    }
}

