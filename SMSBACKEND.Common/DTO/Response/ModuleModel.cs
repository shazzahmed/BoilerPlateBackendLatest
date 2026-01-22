
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class ModuleModel
    {
        public int Id { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public bool IsOpen { get; set; }
        public bool IsDashboard { get; set; }
        public bool IsActive { get; set; }
        public int? ParentId { get; set; }
        public int OrderById { get; set; }
        //public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        // Navigation property for self-referencing relationship
        public virtual ICollection<ModuleModel> Children { get; set; } = new List<ModuleModel>();

        // Optional navigation property to access the parent menu
        public virtual ModuleModel Parent { get; set; }
        public MenuType Type { get; set; }
        //public List<string> Permissions { get; set; } = new();
        public ICollection<PermissionModel> Permissions { get; set; } = new List<PermissionModel>();
    }
}
