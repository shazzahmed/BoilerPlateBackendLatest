using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Response
{
    public class StudentInfoModel : BaseClass
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string AdmissionNo { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        public DateTime? Dob { get; set; }

        [MaxLength(10)]
        public string Gender { get; set; }

        [MaxLength(50)]
        public string Religion { get; set; }

        [MaxLength(50)]
        public string Cast { get; set; }

        [MaxLength(20)]
        public string PhoneNo { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(20)]
        public string MobileNo { get; set; }

        [MaxLength(20)]
        public string BloodGroup { get; set; }

        [MaxLength(10)]
        public string Height { get; set; }

        [MaxLength(10)]
        public string Weight { get; set; }

        public DateTime? MeasurementDate { get; set; }

        [MaxLength(500)]
        public string Image { get; set; }

        public DateTime? AdmissionDate { get; set; }

        [MaxLength(20)]
        public string RollNo { get; set; }

        public int? SchoolHouseId { get; set; }

        public int? CategoryId { get; set; }

        public string ParentId { get; set; }

        public string StudentUserId { get; set; }

        // Navigation properties
        public SchoolHouseModel SchoolHouse { get; set; }
        public StudentCategoryModel StudentCategory { get; set; }
    }
}
