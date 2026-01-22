
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class DepartmentModel : BaseClass
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}
