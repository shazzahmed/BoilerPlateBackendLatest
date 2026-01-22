
// FrontCmsPages Model
public class FrontCmsPages
    {
        public int Id { get; set; }
        public string PageType { get; set; } = "manual";
        public int IsHomepage { get; set; } = 0;
        public string Title { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
        public string Slug { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeyword { get; set; }
        public string FeatureImage { get; set; }
        public string Description { get; set; }
        public DateTime PublishDate { get; set; }
        public int Publish { get; set; } = 0;
        public int Sidebar { get; set; } = 0;
        public string IsActive { get; set; } = "no";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
