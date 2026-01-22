using Common.DTO.Response;
using System.ComponentModel.DataAnnotations.Schema;

public class TeacherSubjectsModel
{
    public int Id { get; set; }
    public int TeacherId { get; set; }
    public int SubjectId { get; set; }
    public string TeacherName { get; set; }
    public string SubjectName { get; set; }
    public List<int> SubjectIds { get; set; }


    [ForeignKey("SubjectId")]
    public virtual SubjectModel Subject { get; set; }

    [ForeignKey("TeacherId")]
    public virtual StaffModel Teacher { get; set; }
}
