using static Common.Utilities.Enums;

namespace Domain.Entities
{
    public class Module
    {
        public int Id { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public bool IsOpen { get; set; }
        public bool IsDashboard { get; set; }
        public int? ParentId { get; set; }
        //public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        // Navigation property for self-referencing relationship
        public virtual ICollection<Module> Children { get; set; } = new List<Module>();

        // Optional navigation property to access the parent menu
        public virtual Module? Parent { get; set; }
        public MenuType Type { get; set; }
        public bool IsActive { get; set; }
        public int OrderById { get; set; }
        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    }
}
