using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Response
{
    public class StudentAddressModel : BaseClass
    {
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [MaxLength(500)]
        public string CurrentAddress { get; set; }

        [MaxLength(500)]
        public string AreaAddress { get; set; }

        [MaxLength(500)]
        public string PermanentAddress { get; set; }

        [MaxLength(20)]
        public string Zipcode { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(100)]
        public string State { get; set; }
    }
}
