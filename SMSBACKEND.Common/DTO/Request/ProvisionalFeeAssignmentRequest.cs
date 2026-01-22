using Common.DTO.Response;

namespace Common.DTO.Request
{
    /// <summary>
    /// Request DTO for assigning provisional fees to applications (pre-admission)
    /// </summary>
    public class ProvisionalFeeAssignmentRequest
    {
        public int ApplicationId { get; set; }
        public List<int> FeeGroupFeeTypeIds { get; set; } = new List<int>();
        public int Month { get; set; }
        public int Year { get; set; }
    }

    /// <summary>
    /// Request DTO for migrating provisional fees when application is converted to admission
    /// </summary>
    public class MigrateProvisionalFeesRequest
    {
        public int ApplicationId { get; set; }
        public int StudentId { get; set; }
    }

    /// <summary>
    /// Request DTO for getting fee preview for an application
    /// </summary>
    public class FeePreviewRequest
    {
        public int? FeeGroupFeeTypeId { get; set; }
        public int ClassId { get; set; }
    }
}


