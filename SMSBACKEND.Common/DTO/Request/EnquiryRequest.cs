using Common.DTO.Response;
using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class EnquiryRequest : BaseClass
    {
        [Required]
        [MaxLength(200)]
        public string FullName { get; set; }

        public DateTime? Dob { get; set; }

        [Required]
        public int ApplyingClass { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(500)]
        public string Address { get; set; }

        [MaxLength(50)]
        public string Source { get; set; }

        [MaxLength(1000)]
        public string Notes { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "New";
    }

    public class EnquiryCreateRequest : EnquiryRequest
    {
        // Inherits all properties from EnquiryRequest
    }

    public class EnquiryUpdateRequest : EnquiryRequest
    {
        [Required]
        public int Id { get; set; }
    }
}
