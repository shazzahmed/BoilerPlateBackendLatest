
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class SubjectModel : BaseClass
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Code { get; set; }
        public SubjectTypes Type { get; set; }
    }
}
