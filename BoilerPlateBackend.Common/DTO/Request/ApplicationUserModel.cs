using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Common.Utilities;
using Common.DTO.Response;
using Common.Validation;

namespace Common.DTO.Request
{
    public class ApplicationUserModel
    {
        public string? Id { get; set; }
        [MaxLength(60)]
        [Required]
        public string? FirstName { get; set; }
        [MaxLength(60)]
        public string? LastName { get; set; }
        public string CNIC { get; set; }
        public string UserName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        [Column(TypeName = "DateTime2")]
        public DateTime BirthDate { get; set; }
        [Phone]
        [RequireOneOfTwoFields(nameof(Email))]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;
        [EmailAddress]
        [RequireOneOfTwoFields(nameof(PhoneNumber))]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        public Enums.Gender Gender { get; set; }
        [MaxLength(500)]
        public string Picture { get; set; }

        [MaxLength(100)]
        public string? Address { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DisabledDate { get; set; }

        public int? StatusId { get; set; }

        public DateTime LastLoginAt { get; set; }
        public bool ForceChangePassword { get; set; }
        public bool IsActive { get; set; } = true;

        public StatusModel? Status { get; set; }


        public virtual ICollection<ApplicationUserRoleModel>? UserRoles { get; set; }
    }
}
