
namespace Common.DTO.Request
{
    public class AttendanceRequest
    {
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public bool IsView { get; set; }
        public PaginationParams PaginationParam { get; set; } = new PaginationParams();
    }
}
