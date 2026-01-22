
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class SectionModel : BaseClass
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}
