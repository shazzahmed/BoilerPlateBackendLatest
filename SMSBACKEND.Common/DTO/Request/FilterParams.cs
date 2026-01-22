
namespace Common.DTO.Request
{
    public class FilterParams
    {
        //public Dictionary<string, object> Filters { get; set; } = new Dictionary<string, object>();
        public List<Filter> Filters { get; set; } = new List<Filter>();

        public PaginationParams PaginationParam { get; set; } = new PaginationParams();
    }
}
