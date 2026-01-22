using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class BulkStudentImportRequest
    {
        [Required]
        public List<StudentImportRow> students { get; set; } = new List<StudentImportRow>();
        
        public string ImportJobId { get; set; } = string.Empty;
        
        [Required]
        public string CreatedBy { get; set; } = string.Empty;
        
        public int SessionId { get; set; } = 1;
    }

    public class StudentImportRow
    {
        public int RowNumber { get; set; }
        
        [Required]
        public string FullName { get; set; } = string.Empty;
        
        public DateTime? DateOfBirth { get; set; }
        
        public string Gender { get; set; } = string.Empty;
        
        [Required]
        public string Class { get; set; } = string.Empty;
        
        public string Section { get; set; } = string.Empty;
        
        public string PhoneNo { get; set; } = string.Empty;
        
        public string Email { get; set; } = string.Empty;
        
        public string Address { get; set; } = string.Empty;
        
        // Parent/Guardian Information
        public string FatherName { get; set; } = string.Empty;
        
        public string FatherCNIC { get; set; } = string.Empty;
        
        public string FatherPhone { get; set; } = string.Empty;
        
        public string MotherName { get; set; } = string.Empty;
        
        public string MotherCNIC { get; set; } = string.Empty;
        
        public string MotherPhone { get; set; } = string.Empty;
        
        public string GuardianType { get; set; } = "Father";
        
        public string GuardianName { get; set; } = string.Empty;
        
        public string GuardianPhone { get; set; } = string.Empty;
        
        public string GuardianEmail { get; set; } = string.Empty;
        
        // Academic Information
        public string RollNo { get; set; } = string.Empty;
        
        public string AdmissionNo { get; set; } = string.Empty;
        
        public DateTime? AdmissionDate { get; set; }
        
        // Financial Information
        public decimal? MonthlyFees { get; set; }
        
        public decimal? AdmissionFees { get; set; }
        
        public decimal? SecurityDeposit { get; set; }
        
        // Physical Information
        public string Height { get; set; } = string.Empty;
        
        public string Weight { get; set; } = string.Empty;
        
        public DateTime? MeasurementDate { get; set; }
        
        public string BloodGroup { get; set; } = string.Empty;
        
        // Additional Information
        public string Religion { get; set; } = string.Empty;
        
        public string Cast { get; set; } = string.Empty;
        
        public string MedicalHistory { get; set; } = string.Empty;
        
        public string Notes { get; set; } = string.Empty;
        
        // Validation and Status
        public List<string> ValidationErrors { get; set; } = new List<string>();
        
        public bool IsValid { get; set; } = true;
        
        public ImportStatus Status { get; set; } = ImportStatus.Pending;
    }

    public enum ImportStatus
    {
        Pending,
        Processing,
        Completed,
        Failed
    }
}
