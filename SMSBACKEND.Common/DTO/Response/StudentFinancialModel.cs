using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Response
{
    public class StudentFinancialModel : BaseClass
    {
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        public float? AdmissionFees { get; set; }

        public float? SecurityDeposit { get; set; }

        [Required]
        public decimal MonthlyFees { get; set; }

        [Required]
        public decimal Arrears { get; set; }
    }
}
