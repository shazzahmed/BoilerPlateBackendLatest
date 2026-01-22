
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class FeeGroupModel : BaseClass
    {
        public int Id { get; set; }
        [MaxLength(200)]
        public required string Name { get; set; }
        public string? Description { get; set; }
        public bool IsSystem { get; set; } = false;

        [JsonIgnore]
        public List<FeeGroupFeeTypeModel>? FeeGroupFeeTypes { get; set; }
        [JsonIgnore]
        public List<FeeTypeModel>? FeeTypes { get; set; }
    }
}
