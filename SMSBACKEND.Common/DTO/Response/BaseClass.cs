using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace Common.DTO.Response
{
    public class BaseClass
    {
        /// <summary>
        /// Multi-Tenancy: Links this entity to a specific school/tenant
        /// All queries are automatically filtered by this TenantId
        /// </summary>
        public int TenantId { get; set; }

        [ForeignKey("TenantId")]
        [JsonIgnore]
        public virtual TenantInfo? Tenant { get; set; }
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "DateTime2")]
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string UpdatedBy { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        //public bool IsActive { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        // Sync fields for offline-first functionality
        public string SyncStatus { get; set; } = "synced"; // synced, created, updated, deleted
    }
}
