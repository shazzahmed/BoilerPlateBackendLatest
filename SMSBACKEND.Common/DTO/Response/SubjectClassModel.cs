
using System.Text.Json.Serialization;
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class SubjectClassModel
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public int ClassId { get; set; }
        
        /// <summary>
        /// Computed property - only used for responses, ignored during deserialization
        /// </summary>
        public string? SubjectName => Subject?.Name;
        
        /// <summary>
        /// Navigation property - only used internally, ignored during serialization/deserialization
        /// </summary>
        public SubjectModel? Subject { get; set; }
        
        /// <summary>
        /// Navigation property - only used internally, ignored during serialization/deserialization
        /// </summary>
        [JsonIgnore]
        public ClassModel? Class { get; set; }
    }
}
