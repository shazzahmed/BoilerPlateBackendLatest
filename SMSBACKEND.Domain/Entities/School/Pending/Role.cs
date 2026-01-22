public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public int IsActive { get; set; } = 0;
    public int IsSystem { get; set; } = 0;
    public int IsSuperadmin { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}
