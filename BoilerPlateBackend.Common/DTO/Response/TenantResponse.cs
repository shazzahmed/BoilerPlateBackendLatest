namespace Common.DTO.Response
{
    /// <summary>
    /// Response model for tenant/school information
    /// Returned to frontend after login or when fetching tenant details
    /// </summary>
    public class TenantResponse
    {
        public int TenantId { get; set; }
        public string TenantCode { get; set; } = string.Empty;
        public string SchoolName { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string? Domain { get; set; }

        // Contact Information
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;

        // Branding
        public string Logo { get; set; } = string.Empty;
        public string PrimaryColor { get; set; } = "#1976d2";
        public string SecondaryColor { get; set; } = "#dc004e";

        // Subscription
        public DateTime SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public string SubscriptionTier { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsSubscriptionValid { get; set; }

        // Resource Usage
        public int MaxStudents { get; set; }
        public int CurrentStudentCount { get; set; }
        public int MaxStaff { get; set; }
        public int CurrentStaffCount { get; set; }
        public decimal StorageLimitGB { get; set; }
        public decimal CurrentStorageUsedGB { get; set; }

        // Regional Settings
        public string TimeZone { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string CurrencySymbol { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string DateFormat { get; set; } = string.Empty;

        // Academic Configuration
        public string InstitutionType { get; set; } = string.Empty;
        public string EducationLevel { get; set; } = string.Empty;
        public int AcademicYearStartMonth { get; set; }
        public int AcademicYearEndMonth { get; set; }

        // Features - Split into array for frontend consumption
        public List<string> EnabledFeatures { get; set; } = new List<string>();

        // Admin
        public string? PrimaryAdminUserId { get; set; }
        public string PrimaryContactName { get; set; } = string.Empty;

        // System Fields
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;

        // Computed Properties
        public bool IsStudentLimitReached => CurrentStudentCount >= MaxStudents;
        public bool IsStaffLimitReached => CurrentStaffCount >= MaxStaff;
        public int StudentsRemaining => Math.Max(0, MaxStudents - CurrentStudentCount);
        public int StaffRemaining => Math.Max(0, MaxStaff - CurrentStaffCount);
        public int DaysUntilExpiration
        {
            get
            {
                if (!SubscriptionEndDate.HasValue) return int.MaxValue;
                return (SubscriptionEndDate.Value - DateTime.UtcNow).Days;
            }
        }
        public bool IsExpiringSoon => DaysUntilExpiration <= 30 && DaysUntilExpiration > 0;
    }
}


