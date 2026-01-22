
namespace Common.DTO.Request
{
    public class Filter
    {
        public string? PropertyName { get; set; }
        public object? Value { get; set; }
        public string Operator { get; set; } = "Equals"; // Default operator
    }

}
