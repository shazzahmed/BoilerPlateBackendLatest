public class PermissionCategory
{
    public int Id { get; set; }
    public int? PermGroupId { get; set; }
    public string Name { get; set; }
    public string ShortCode { get; set; }
    public int EnableView { get; set; }
    public int EnableAdd { get; set; }
    public int EnableEdit { get; set; }
    public int EnableDelete { get; set; }
    public DateTime CreatedAt { get; set; }
}
