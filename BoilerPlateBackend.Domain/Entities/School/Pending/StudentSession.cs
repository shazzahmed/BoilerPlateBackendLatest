public class StudentSession
{
    public int Id { get; set; }
    public int? SessionId { get; set; }
    public int? StudentId { get; set; }
    public int? ClassId { get; set; }
    public int? GradeInAdmitted { get; set; }
    public int? SectionId { get; set; }
    public int RouteId { get; set; }
    public int HostelRoomId { get; set; }
    public int? VehRouteId { get; set; }
    public float TransportFees { get; set; } = 0.00f;
    public float FeesDiscount { get; set; } = 0.00f;
    public string IsActive { get; set; } = "no";
    public int IsAlumni { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
