
// FrontCmsMenuItems Model
public class FrontCmsMenuItems
    {
        public int Id { get; set; }
        public int MenuId { get; set; }
        public string Menu { get; set; }
        public int PageId { get; set; }
        public int ParentId { get; set; }
        public string ExtUrl { get; set; }
        public int OpenNewTab { get; set; } = 0;
        public string ExtUrlLink { get; set; }
        public string Slug { get; set; }
        public int? Weight { get; set; }
        public int Publish { get; set; } = 0;
        public string Description { get; set; }
        public string IsActive { get; set; } = "no";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
