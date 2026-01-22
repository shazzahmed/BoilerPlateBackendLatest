
namespace Common.DTO.Request
{
    public class StaffRequest
    {
        //public DateTime? LeadRecieveDate { get; set; }
        public string DepartmentId { get; set; }
        public string RoleId { get; set; }
        public string Name { get; set; }
        public PaginationParams PaginationParam { get; set; } = new PaginationParams();
    }
}
