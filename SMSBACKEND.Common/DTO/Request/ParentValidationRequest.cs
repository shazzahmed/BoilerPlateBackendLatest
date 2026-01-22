using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class ParentValidationRequest
    {
        [MaxLength(20)]
        public string CNIC { get; set; } = string.Empty;

        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        // Optional: For updating existing parent
        public string? ExistingParentId { get; set; }
    }
}
