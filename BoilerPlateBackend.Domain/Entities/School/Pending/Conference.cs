public class Conference
{
    public int Id { get; set; }
    public string Purpose { get; set; } = "class";
    public int? StaffId { get; set; }
    public int CreatedId { get; set; }
    public string? Title { get; set; }
    public DateTime? Date { get; set; }
    public int? Duration { get; set; }
    public string? Password { get; set; }
    public string? Subject { get; set; }
    public int? ClassId { get; set; }
    public int? SectionId { get; set; }
    public int SessionId { get; set; }
    public bool HostVideo { get; set; } = true;
    public bool ClientVideo { get; set; } = true;
    public string? Description { get; set; }
    public string? Timezone { get; set; }
    public string? ReturnResponse { get; set; }
    public string ApiType { get; set; } = string.Empty;
    public bool Status { get; set; } = false;
    public bool IsSession { get; set; } = false;
    public DateTime CreatedAt { get; set; }
}

