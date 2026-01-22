
using Common.DTO.Response;

namespace Common.DTO.Request
{
    public class FeeAssignmentRequest
    {
        public List<int> FeeGroupIds { get; set; } = new List<int>();
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public int StudentId { get; set; }
        public int Month { get; set; } // e.g. 7
        public int Year { get; set; }  // e.g. 2025
        public List<FeeAssignmentModel> Students { get; set; } = new List<FeeAssignmentModel>();
        public List<FeeTypeDetailModel> FeeTypes { get; set; } = new List<FeeTypeDetailModel>(); // Ensures a valid list
        public List<int> FeeTypeIds { get; set; } = new List<int>(); // Ensures a valid list
        public List<int?> FeeGroupFeeTypeIds { get; set; } = new List<int?>(); // Ensures a valid list
        public List<int> StudentIds { get; set; } = new List<int>();
        public PaginationParams PaginationParam { get; set; } = new PaginationParams();
    }
}
