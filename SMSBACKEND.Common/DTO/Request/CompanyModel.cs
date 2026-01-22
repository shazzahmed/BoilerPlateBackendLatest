using System.ComponentModel.DataAnnotations;
using Common.DTO.Response;

namespace Common.DTO.Request
{
    public class CompanyModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        [Required]
        public string Website { get; set; } = string.Empty;


        public virtual ICollection<AddressModel>? Addresses { get; set; }
    }
}
