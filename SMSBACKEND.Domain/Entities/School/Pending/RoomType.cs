using Domain.Entities;

public class RoomType : BaseEntity
{
    public int Id { get; set; }
    public string RoomTypeName { get; set; }
    public string Description { get; set; }
}
