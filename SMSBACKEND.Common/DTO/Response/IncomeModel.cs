
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class IncomeModel : BaseClass
    {
        public int Id { get; set; }
        public int IncomeHeadId { get; set; }
        [Required]
        [MaxLength(200)]
        public required string Name { get; set; }
        [MaxLength(50)]
        public string? InvoiceNo { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string? AttachDocument { get; set; }
        public string? Description { get; set; }
        [ForeignKey("IncomeHeadId")]
        public virtual IncomeHeadModel? IncomeHead { get; set; }
    }
}
