public class TransportRoute
{
    public int Id { get; set; }
    public string RouteTitle { get; set; }
    public int? NoOfVehicle { get; set; }
    public decimal Fare { get; set; }
    public string Note { get; set; }
    public string IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
