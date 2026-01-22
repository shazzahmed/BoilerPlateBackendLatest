using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    /// <summary>
    /// Represents a school/organization tenant in the multi-tenant SMS system
    /// Each tenant is an independent school with its own data, users, and configuration
    /// </summary>
    public class Tenant
    {
        [Key]
        public int TenantId { get; set; }

        /// <summary>
        /// Unique identifier for the tenant (e.g., "SCHOOL001", "GREENWOOD_HIGH")
        /// Used for subdomain-based routing: school001.sms.com
        /// </summary>
        [Required]
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string TenantCode { get; set; } = string.Empty;

        /// <summary>
        /// Official name of the school/institution
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string SchoolName { get; set; } = string.Empty;

        /// <summary>
        /// Short name or abbreviation (e.g., "GHS" for Greenwood High School)
        /// </summary>
        [MaxLength(50)]
        public string ShortName { get; set; } = string.Empty;

        /// <summary>
        /// Optional custom domain (e.g., school001.sms.com or school.edu)
        /// </summary>
        [MaxLength(100)]
        public string? Domain { get; set; }

        // Contact Information
        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string ContactEmail { get; set; } = string.Empty;

        [MaxLength(20)]
        [Phone]
        public string? ContactPhone { get; set; }

        [MaxLength(100)]
        public string? Website { get; set; }

        // Branding
        /// <summary>
        /// Path or URL to school logo
        /// </summary>
        [MaxLength(500)]
        public string? Logo { get; set; }

        /// <summary>
        /// Primary brand color (hex code)
        /// </summary>
        [MaxLength(10)]
        public string? PrimaryColor { get; set; }

        /// <summary>
        /// Secondary brand color (hex code)
        /// </summary>
        [MaxLength(10)]
        public string? SecondaryColor { get; set; }

        // Subscription & Licensing
        /// <summary>
        /// When the school's subscription started
        /// </summary>
        [Required]
        [Column(TypeName = "DateTime2")]
        public DateTime SubscriptionStartDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// When the subscription expires (null = no expiration)
        /// </summary>
        [Column(TypeName = "DateTime2")]
        public DateTime? SubscriptionEndDate { get; set; }

        /// <summary>
        /// Subscription tier: Basic, Standard, Premium, Enterprise
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string SubscriptionTier { get; set; } = "Basic";

        /// <summary>
        /// Whether the tenant is currently active and can access the system
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Whether the subscription is currently valid (not expired)
        /// </summary>
        public bool IsSubscriptionValid { get; set; } = true;

        // Resource Limits
        /// <summary>
        /// Maximum number of students allowed based on subscription
        /// </summary>
        public int MaxStudents { get; set; } = 500;

        /// <summary>
        /// Current number of students (cached for quick checks)
        /// </summary>
        public int CurrentStudentCount { get; set; } = 0;

        /// <summary>
        /// Maximum number of staff members allowed
        /// </summary>
        public int MaxStaff { get; set; } = 50;

        /// <summary>
        /// Current number of staff members (cached for quick checks)
        /// </summary>
        public int CurrentStaffCount { get; set; } = 0;

        /// <summary>
        /// Storage limit in GB
        /// </summary>
        public decimal StorageLimitGB { get; set; } = 10;

        /// <summary>
        /// Current storage used in GB
        /// </summary>
        public decimal CurrentStorageUsedGB { get; set; } = 0;

        // Regional Settings
        /// <summary>
        /// Timezone for the school (e.g., "Asia/Karachi", "America/New_York")
        /// </summary>
        [MaxLength(100)]
        public string? TimeZone { get; set; }

        /// <summary>
        /// Currency code (e.g., "USD", "PKR", "GBP")
        /// </summary>
        [MaxLength(10)]
        public string? Currency { get; set; }

        /// <summary>
        /// Currency symbol (e.g., "$", "Rs", "£")
        /// </summary>
        [MaxLength(10)]
        public string? CurrencySymbol { get; set; }

        /// <summary>
        /// Default language (e.g., "en", "ur", "ar")
        /// </summary>
        [MaxLength(10)]
        public string? Language { get; set; }

        /// <summary>
        /// Date format preference (e.g., "MM/DD/YYYY", "DD/MM/YYYY")
        /// </summary>
        [MaxLength(20)]
        public string? DateFormat { get; set; }

        // Academic Configuration
        /// <summary>
        /// Type of institution: School, College, University, Academy
        /// </summary>
        [MaxLength(50)]
        public string? InstitutionType { get; set; }

        /// <summary>
        /// Education level: Primary, Secondary, HigherSecondary, All
        /// </summary>
        [MaxLength(50)]
        public string? EducationLevel { get; set; }

        /// <summary>
        /// Academic year start month (1-12)
        /// </summary>
        public int AcademicYearStartMonth { get; set; } = 9; // September

        /// <summary>
        /// Academic year end month (1-12)
        /// </summary>
        public int AcademicYearEndMonth { get; set; } = 6; // June

        // Feature Flags (JSON or comma-separated)
        /// <summary>
        /// Enabled features based on subscription tier
        /// JSON array: ["Attendance", "Fees", "Exams", "Library", "Transport"]
        /// </summary>
        [MaxLength(1000)]
        public string? EnabledFeatures { get; set; }

        // Admission Number Configuration
        /// <summary>
        /// Last used student sequence number (auto-increment counter)
        /// Used for generating unique admission numbers
        /// </summary>
        public int LastStudentSequence { get; set; } = 0;

        /// <summary>
        /// Admission number format template
        /// Placeholders: {SEQ} = sequence, {SEQ:000} = padded sequence, {YEAR} = current year
        /// Examples: "{SEQ}" -> "1", "ADM{YEAR}{SEQ:000}" -> "ADM2025001", "{YEAR}/{SEQ:000}" -> "2025/001"
        /// </summary>
        [MaxLength(100)]
        public string AdmissionNoFormat { get; set; } = "{SEQ}"; // Default: simple number

        // Parent Identification Configuration
        /// <summary>
        /// Determines if parent CNIC fields accept numeric ParentId OR require strict CNIC format
        /// true = Accept BOTH CNIC (12345-1234567-1) AND numeric ParentId (151)
        /// false = Require strict CNIC format only (12345-1234567-1)
        /// 
        /// Use Cases:
        /// - Schools without parent CNICs → Set to true (allows ParentId)
        /// - Schools with mandatory CNICs → Set to false (strict validation)
        /// 
        /// Frontend: Controls which validator is applied to FatherCNIC/MotherCNIC fields
        /// </summary>
        public bool AllowParentIdInCnicField { get; set; } = true; // Default: allow ParentId for backward compatibility

        // Admin Information
        /// <summary>
        /// User ID of the primary admin/owner for this tenant
        /// </summary>
        [MaxLength(450)] // ASP.NET Identity user ID length
        public string? PrimaryAdminUserId { get; set; }

        /// <summary>
        /// Name of the primary contact person
        /// </summary>
        [MaxLength(100)]
        public string? PrimaryContactName { get; set; }

        // System Fields
        /// <summary>
        /// When this tenant was created
        /// </summary>
        [Required]
        [Column(TypeName = "DateTime2")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// When tenant settings were last updated
        /// </summary>
        [Column(TypeName = "DateTime2")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Who created this tenant (super admin)
        /// </summary>
        [MaxLength(100)]
        public string CreatedBy { get; set; } = "System";

        /// <summary>
        /// Who last updated tenant settings
        /// </summary>
        [MaxLength(100)]
        public string UpdatedBy { get; set; } = "System";

        /// <summary>
        /// Soft delete flag
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// When the tenant was soft deleted
        /// </summary>
        [Column(TypeName = "DateTime2")]
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Additional notes or comments about the tenant
        /// </summary>
        [MaxLength(1000)]
        public string? Notes { get; set; }

        // Navigation Properties
        // Uncomment if you want to navigate from Tenant to Users
        // public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}


