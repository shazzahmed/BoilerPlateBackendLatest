
using System.ComponentModel.DataAnnotations;
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class FeeTypeModel : BaseClass
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public required string Name { get; set; }
        [Required]
        [MaxLength(100)]
        public required string Code { get; set; }
        public string? Description { get; set; }
        public FeeFrequency FeeFrequency { get; set; }
        public bool IsSystem { get; set; } = false;
    }
}
