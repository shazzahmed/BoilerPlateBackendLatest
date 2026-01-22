
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class SessionModel : BaseClass
    {
        public string Sessions { get; set; } = string.Empty;  // e.g., "2024-2025"

        [Required]
        [Column(TypeName = "DateTime2")]
        public DateTime StartDate { get; set; }

        [Required]
        [Column(TypeName = "DateTime2")]
        public DateTime EndDate { get; set; }
    }
}
