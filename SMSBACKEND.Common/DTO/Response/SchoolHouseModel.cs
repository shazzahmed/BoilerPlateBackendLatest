
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class SchoolHouseModel : BaseClass
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
