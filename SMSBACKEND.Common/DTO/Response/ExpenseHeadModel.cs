
using System.ComponentModel.DataAnnotations;
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class ExpenseHeadModel : BaseClass
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public required string Name { get; set; }
        [MaxLength(255)]
        public string? Description { get; set; }
    }
}
