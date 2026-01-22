
// FrontCmsMenus Model
public class FrontCmsMenus
    {
        public int Id { get; set; }
        public string Menu { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public int OpenNewTab { get; set; } = 0;
        public string ExtUrl { get; set; }
        public string ExtUrlLink { get; set; }
        public int Publish { get; set; } = 0;
        public string ContentType { get; set; } = "manual";
        public string IsActive { get; set; } = "no";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
