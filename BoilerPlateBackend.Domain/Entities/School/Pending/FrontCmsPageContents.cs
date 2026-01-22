
// FrontCmsPageContents Model
public class FrontCmsPageContents
    {
        public int Id { get; set; }
        public int? PageId { get; set; }
        public string ContentType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
