
namespace Common.DTO.Request
{
    public class ClassTeacherRequest
    {
        public int Id { get; set; }
        public int ClassSectionId { get; set; }
        public List<int> StaffIds { get; set; } = new List<int>(); // Ensures a valid list

        public PaginationParams PaginationParam { get; set; } = new PaginationParams();
    }
}
