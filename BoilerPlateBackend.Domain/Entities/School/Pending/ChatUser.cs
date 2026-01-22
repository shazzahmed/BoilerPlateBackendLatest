public class ChatUser
{
    public int Id { get; set; }
    public string UserType { get; set; }
    public int? StaffId { get; set; }
    public int? StudentId { get; set; }
    public int? CreateStaffId { get; set; }
    public int? CreateStudentId { get; set; }
    public int IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
