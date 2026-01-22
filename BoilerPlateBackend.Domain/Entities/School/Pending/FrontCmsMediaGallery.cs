
// FrontCmsMediaGallery Model
public class FrontCmsMediaGallery
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public string ThumbPath { get; set; }
        public string DirPath { get; set; }
        public string ImgName { get; set; }
        public string ThumbName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string FileType { get; set; }
        public string FileSize { get; set; }
        public string VidUrl { get; set; }
        public string VidTitle { get; set; }
    }
