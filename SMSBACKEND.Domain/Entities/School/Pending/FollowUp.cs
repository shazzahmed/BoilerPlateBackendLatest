
// FollowUp Model
public class FollowUp
    {
        public int Id { get; set; }
        public int EnquiryId { get; set; }
        public DateTime Date { get; set; }
        public DateTime NextDate { get; set; }
        public string Response { get; set; }
        public string Note { get; set; }
        public string FollowUpBy { get; set; }
    }
