
// FrontCmsProgramPhotos Model
public class FrontCmsProgramPhotos
    {
        public int Id { get; set; }
        public int? ProgramId { get; set; }
        public int MediaGalleryId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
