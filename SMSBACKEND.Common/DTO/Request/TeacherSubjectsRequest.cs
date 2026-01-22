
namespace Common.DTO.Request
{
    public class TeacherSubjectsRequest
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public List<int> SubjectIds { get; set; } = new List<int>(); // Ensures a valid list

        public PaginationParams PaginationParam { get; set; } = new PaginationParams();
    }
}
