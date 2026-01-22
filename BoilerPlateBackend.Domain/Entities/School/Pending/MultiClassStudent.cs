using Domain.Entities;

namespace BoilerPlateBackend.Domain.Entities.School.Pending;

public class MultiClassStudent : BaseEntity
{
    public int Id { get; set; }
    public int? StudentId { get; set; }
    public int? StudentSessionId { get; set; }
}
