
using System.Reflection;
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class PermissionModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ModuleId { get; set; }
        public Module Module { get; set; }
        //public int PermissionActionId { get; set; }
        //public PermissionActionModel PermissionAction { get; set; }
    }
}
