using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Response
{
    public class StudentParentModel : BaseClass
    {
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        // Father Information
        [MaxLength(200)]
        public string FatherName { get; set; }

        [MaxLength(20)]
        public string FatherPhone { get; set; }

        [MaxLength(100)]
        public string FatherOccupation { get; set; }

        [MaxLength(20)]
        public string FatherCNIC { get; set; }

        [MaxLength(500)]
        public string FatherPic { get; set; }

        // Mother Information
        [MaxLength(200)]
        public string MotherName { get; set; }

        [MaxLength(20)]
        public string MotherPhone { get; set; }

        [MaxLength(100)]
        public string MotherOccupation { get; set; }

        [MaxLength(20)]
        public string MotherCNIC { get; set; }

        [MaxLength(500)]
        public string MotherPic { get; set; }

        // Guardian Information
        [MaxLength(50)]
        public string GuardianIs { get; set; }

        [MaxLength(200)]
        public string GuardianName { get; set; }

        [MaxLength(50)]
        public string GuardianRelation { get; set; }

        [MaxLength(100)]
        public string GuardianEmail { get; set; }

        [MaxLength(20)]
        public string GuardianPhone { get; set; }

        [MaxLength(100)]
        public string GuardianOccupation { get; set; }

        [MaxLength(500)]
        public string GuardianAddress { get; set; }

        [MaxLength(500)]
        public string GuardianAreaAddress { get; set; }

        [MaxLength(500)]
        public string GuardianPic { get; set; }
    }
}
