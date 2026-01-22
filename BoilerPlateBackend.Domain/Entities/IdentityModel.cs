using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Validation;
using Microsoft.AspNetCore.Identity;
using static Common.Utilities.Enums;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class ApplicationUser : IdentityUser
   {
        /// <summary>
        /// Multi-Tenancy: Links this user to a specific organization/tenant
        /// Users can only access data from their assigned tenant
        /// </summary>
        public int TenantId { get; set; }
        
        [ForeignKey("TenantId")]
        [JsonIgnore] // Prevent reference loops in caching
        public virtual Tenant? Tenant { get; set; }

        [MaxLength(60)]
        public string FirstName { get; set; } = string.Empty;
        [MaxLength(60)]
        public string LastName { get; set; } = string.Empty;
        public string? CNIC { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        [Column(TypeName = "DateTime2")]
        public DateTime BirthDate { get; set; }
        [Phone]
        [RequireOneOfTwoFields(nameof(Email))]
        [Display(Name = "Phone Number")]
        public override string PhoneNumber { get; set; } = string.Empty;
        [EmailAddress]
        [RequireOneOfTwoFields(nameof(PhoneNumber))]
        public override string Email { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        [MaxLength(500)]
        public string? Picture { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DisabledDate { get; set; }

        public int? StatusId { get; set; }

        public bool ForceChangePassword { get; set; }
        public bool IsActive { get; set; } = true;

        public string? ParentId { get; set; } // Self-referencing FK

        [ForeignKey("ParentId")]
        [JsonIgnore] // Prevent reference loops in caching
        public virtual ApplicationUser? Parent { get; set; } // Navigation property
        
        [JsonIgnore] // Prevent reference loops in caching
        public virtual ICollection<ApplicationUser> Children { get; set; } = new List<ApplicationUser>(); // For hierarchical user relationships

        [JsonIgnore] // Prevent reference loops in caching
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
    }

    public class ApplicationUserRole : IdentityUserRole<string>
    {
        [JsonIgnore] // Prevent reference loops in caching
        public virtual ApplicationUser User { get; set; } = null!;
        
        [JsonIgnore] // Prevent reference loops in caching
        public virtual ApplicationRole Role { get; set; } = null!;
    }
    public class ApplicationRole : IdentityRole
    {
        public bool IsSystem { get; set; }
        
        [JsonIgnore] // Prevent reference loops in caching
        public ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
        
        [JsonIgnore] // Prevent reference loops in caching
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
