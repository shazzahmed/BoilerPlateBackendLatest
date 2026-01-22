
using System.ComponentModel.DataAnnotations;
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class FeeGroupWithTypesModel
    {
        public List<int> FeeGroupIds { get; set; } = new List<int>();
        public string? FeeGroupName { get; set; }
        public string? Description { get; set; }
        public bool IsSystem { get; set; } = false;
        public List<FeeTypeDetailModel>? FeeTypes { get; set; }
        public List<int> FeeTypeIds { get; set; } = new List<int>(); // Ensures a valid list
        public List<int> FeeGroupFeeTypeIds { get; set; } = new List<int>(); // Ensures a valid list
    }

    public class FeeTypeDetailModel
    {
        public int FeeTypeId { get; set; }
        public int? FeeDiscountId { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public FeeFrequency FeeFrequency { get; set; }
        public decimal Amount { get; set; }
        public DateTime? DueDate { get; set; }
        public string? FineType { get; set; }
        public decimal FineAmount { get; set; }
        public decimal FinePercentage { get; set; }
        public bool IsSystem { get; set; } = false;
    }

}
