namespace Common.DTO.Response
{
    /// <summary>
    /// General-purpose DTO for counting items by status/role
    /// </summary>
    public class StatusCount
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
