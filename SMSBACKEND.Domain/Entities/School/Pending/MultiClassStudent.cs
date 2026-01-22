using Domain.Entities;

namespace SMSBACKEND.Domain.Entities.School.Pending;

public class MultiClassStudent : BaseEntity
{
    public int Id { get; set; }
    public int? StudentId { get; set; }
    public int? StudentSessionId { get; set; }
}
