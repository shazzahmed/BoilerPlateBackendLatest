
// FrontCmsPrograms Model
public class FrontCmsPrograms
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Slug { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? EventStart { get; set; }
        public DateTime? EventEnd { get; set; }
        public string EventVenue { get; set; }
        public string Description { get; set; }
        public string IsActive { get; set; } = "no";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeyword { get; set; }
        public string FeatureImage { get; set; }
        public DateTime PublishDate { get; set; }
        public string Publish { get; set; } = "0";
        public int Sidebar { get; set; } = 0;
    }
