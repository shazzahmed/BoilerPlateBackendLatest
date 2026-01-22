
using System.ComponentModel.DataAnnotations.Schema;
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class ClassTeacherModel: BaseClass
    {
        public int Id { get; set; }
        public int ClassSectionId { get; set; }
        public int ClassId { get; set; }
        public int StaffId { get; set; }
        public int SectionId { get; set; }
        public string? ClassName { get; set; }
        public string? SectionName { get; set; }
        public List<string>? StaffNameList { get; set; }
        public List<int>? StaffIds { get; set; }
        public int SessionId { get; set; }
        // Navigation properties
        [ForeignKey("ClassSectionId")]
        public virtual ClassSectionModel? ClassSection { get; set; }
        [ForeignKey("StaffId")]
        public virtual StaffModel? Staff { get; set; }
    }
}
