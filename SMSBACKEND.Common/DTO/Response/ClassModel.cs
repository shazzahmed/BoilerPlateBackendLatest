

namespace Common.DTO.Response
{
    public class ClassModel : BaseClass
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<int> SelectedSectionIds { get; set; } = new List<int>();
        public List<int> SelectedSubjectIds { get; set; } = new List<int>();
        public virtual List<ClassSectionModel>? ClassSections { get; set; } = new List<ClassSectionModel>();
        public virtual List<SubjectClassModel>? SubjectClasses { get; set; } = new List<SubjectClassModel>();
        public virtual List<ClassTeacherModel>? ClassTeachers { get; set; } = new List<ClassTeacherModel>();
        public ICollection<StudentClassAssignmentModel>? StudentAssignments { get; set; }
    }
}
