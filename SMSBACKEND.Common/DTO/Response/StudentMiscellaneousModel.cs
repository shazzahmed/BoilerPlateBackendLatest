using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Response
{
    public class StudentMiscellaneousModel : BaseClass
    {
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [MaxLength(1000)]
        public string PreviousSchool { get; set; }

        [MaxLength(1000)]
        public string MedicalHistory { get; set; }

        [MaxLength(200)]
        public string PickupPerson { get; set; }

        [MaxLength(1000)]
        public string Note { get; set; }

        public int DisReason { get; set; }

        [MaxLength(1000)]
        public string DisNote { get; set; }

        public DateTime? DisableAt { get; set; }
    }
}
