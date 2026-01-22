
using Common.DTO.Response;
using static Common.Utilities.Enums;

namespace Common.DTO.Request
{
    public class FeeGroupFeeTypeRequest
    {
        public int FeeGroupIds { get; set; }
        public List<FeeTypeDetailModel> FeeTypes { get; set; } = new List<FeeTypeDetailModel>(); // Ensures a valid list
        public List<int> FeeTypeIds { get; set; } = new List<int>(); // Ensures a valid list
        public PaginationParams PaginationParam { get; set; } = new PaginationParams();
    }
}
