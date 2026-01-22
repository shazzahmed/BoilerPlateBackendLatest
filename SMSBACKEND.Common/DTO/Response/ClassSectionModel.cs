using Common.DTO.Response;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Common.DTO.Response;

public class ClassSectionModel
{
    public int Id { get; set; }
    public int ClassId { get; set; }
    public int SectionId { get; set; }
    
    /// <summary>
    /// Computed property - only used for responses, ignored during deserialization
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ClassName => Class?.Name;
    
    /// <summary>
    /// Computed property - only used for responses, ignored during deserialization
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SectionName => Section?.Name;
    
    // Navigation properties - ignored during serialization and deserialization
    [JsonIgnore]
    public virtual ClassModel? Class { get; set; }

    //[JsonIgnore]
    public virtual SectionModel? Section { get; set; }
}
