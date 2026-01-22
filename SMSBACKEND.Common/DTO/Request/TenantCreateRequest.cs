using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    /// <summary>
    /// Request model for creating a new tenant/school
    /// Used by super admin to onboard new schools
    /// </summary>
    public class TenantCreateRequest
    {
        [Required(ErrorMessage = "Tenant Code is required")]
        [MaxLength(50, ErrorMessage = "Tenant Code cannot exceed 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Tenant Code can only contain letters, numbers, and underscores")]
        public string TenantCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "School Name is required")]
        [MaxLength(200, ErrorMessage = "School Name cannot exceed 200 characters")]
        public string SchoolName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string ShortName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Domain { get; set; }

        // Contact Information
        [Required(ErrorMessage = "Address is required")]
        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;

        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [MaxLength(100)]
        public string State { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        [MaxLength(20)]
        public string PostalCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(100)]
        public string ContactEmail { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number")]
        [MaxLength(20)]
        public string ContactPhone { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Website { get; set; } = string.Empty;

        // Subscription
        [Required(ErrorMessage = "Subscription Tier is required")]
        public string SubscriptionTier { get; set; } = "Basic"; // Basic, Standard, Premium, Enterprise

        public DateTime SubscriptionStartDate { get; set; } = DateTime.UtcNow;

        public DateTime? SubscriptionEndDate { get; set; }

        // Resource Limits
        [Range(1, 100000, ErrorMessage = "Max Students must be between 1 and 100,000")]
        public int MaxStudents { get; set; } = 500;

        [Range(1, 10000, ErrorMessage = "Max Staff must be between 1 and 10,000")]
        public int MaxStaff { get; set; } = 50;

        [Range(1, 1000, ErrorMessage = "Storage Limit must be between 1 and 1,000 GB")]
        public decimal StorageLimitGB { get; set; } = 10;

        // Regional Settings
        [MaxLength(100)]
        public string TimeZone { get; set; } = "UTC";

        [MaxLength(10)]
        public string Currency { get; set; } = "USD";

        [MaxLength(10)]
        public string CurrencySymbol { get; set; } = "$";

        [MaxLength(10)]
        public string Language { get; set; } = "en";

        [MaxLength(20)]
        public string DateFormat { get; set; } = "MM/DD/YYYY";

        // Academic Configuration
        [MaxLength(50)]
        public string InstitutionType { get; set; } = "School"; // School, College, University, Academy

        [MaxLength(50)]
        public string EducationLevel { get; set; } = "All"; // Primary, Secondary, HigherSecondary, All

        [Range(1, 12)]
        public int AcademicYearStartMonth { get; set; } = 9; // September

        [Range(1, 12)]
        public int AcademicYearEndMonth { get; set; } = 6; // June

        // Features
        public string EnabledFeatures { get; set; } = "Attendance,Fees,Students,Staff,Classes"; // Comma-separated

        // Admin
        [MaxLength(100)]
        public string PrimaryContactName { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Notes { get; set; } = string.Empty;
    }
}


