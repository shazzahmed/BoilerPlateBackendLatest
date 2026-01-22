
// FrontCmsSettings Model
public class FrontCmsSettings
    {
        public int Id { get; set; }
        public string Theme { get; set; }
        public int IsActiveRtl { get; set; } = 0;
        public int IsActiveFrontCms { get; set; } = 0;
        public int IsActiveSidebar { get; set; } = 0;
        public string Logo { get; set; }
        public string ContactUsEmail { get; set; }
        public string ComplainFormEmail { get; set; }
        public string SidebarOptions { get; set; }
        public string WhatsappUrl { get; set; }
        public string FbUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string YoutubeUrl { get; set; }
        public string GooglePlus { get; set; }
        public string InstagramUrl { get; set; }
        public string PinterestUrl { get; set; }
        public string LinkedinUrl { get; set; }
        public string GoogleAnalytics { get; set; }
        public string FooterText { get; set; }
        public string FavIcon { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }