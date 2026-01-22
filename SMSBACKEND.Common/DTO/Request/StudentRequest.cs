
namespace Common.DTO.Request
{
    public class StudentRequest
    {
        //public DateTime? LeadRecieveDate { get; set; }
        public string Gender { get; set; }
        public string SectionId { get; set; }
        public string ClassId { get; set; }
        public string Name { get; set; }
        
        // Sync-related fields for offline-first functionality
        public DateTime? LastSyncTime { get; set; } // Frontend can send last sync time to get only changes
        public bool IncludeDeleted { get; set; } = true; // Include soft-deleted students for frontend visibility control
        public int MaxRecords { get; set; } = 1000; // Maximum records to return (safety limit)
        
        // Removed pagination - frontend will handle pagination locally
        // public PaginationParams PaginationParam { get; set; } = new PaginationParams();
    }
}
