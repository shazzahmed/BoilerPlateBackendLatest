public class ConferencesHistory
{
    public int Id { get; set; }
    public int ConferenceId { get; set; }
    public int? StaffId { get; set; }
    public int? StudentId { get; set; }
    public int TotalHit { get; set; } = 1;
    public DateTime CreatedAt { get; set; }
}

