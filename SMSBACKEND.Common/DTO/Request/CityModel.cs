using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class CityModel
    {
        public int Id { get; set; }

        [MaxLength(256)]
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
