using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Communication
{
    public class SseService
    {
        private readonly List<HttpResponse> _clients = new();

        public void AddClient(HttpResponse response) => _clients.Add(response);

        public async Task BroadcastAsync(string message)
        {
            // Legacy format for backward compatibility
            foreach (var client in _clients.ToList())
            {
                try
                {
                    await client.WriteAsync($"data: {message}\n\n");
                    await client.Body.FlushAsync();
                }
                catch
                {
                    _clients.Remove(client);
                }
            }
        }

        public async Task BroadcastStructuredAsync(string title, string message, string entityType = null, string entityId = null)
        {
            var notification = new
            {
                Title = title,
                Message = message,
                EntityType = entityType,
                EntityId = entityId,
                Timestamp = DateTime.UtcNow.ToString("o"),
                Type = "notification"
            };

            var payload = System.Text.Json.JsonSerializer.Serialize(notification);
            await BroadcastAsync(payload);
        }

        public async Task BroadcastEntityEventAsync(string action, string entityName, string entityType, string entityId, string additionalMessage = null)
        {
            var title = $"{action} {entityType}";
            var message = additionalMessage ?? $"{entityName} - {entityType} has been {action.ToLower()}";
            
            await BroadcastStructuredAsync(title, message, entityType, entityId);
        }

        public async Task BroadcastEntityCreatedAsync(string entityName, string entityType, string entityId)
        {
            await BroadcastEntityEventAsync("Created", entityName, entityType, entityId);
        }

        public async Task BroadcastEntityUpdatedAsync(string entityName, string entityType, string entityId)
        {
            await BroadcastEntityEventAsync("Updated", entityName, entityType, entityId);
        }

        public async Task BroadcastEntityDeletedAsync(string entityName, string entityType, string entityId)
        {
            await BroadcastEntityEventAsync("Deleted", entityName, entityType, entityId);
        }

        public async Task BroadcastEntityRestoredAsync(string entityName, string entityType, string entityId)
        {
            await BroadcastEntityEventAsync("Restored", entityName, entityType, entityId);
        }

        public async Task BroadcastCustomNotificationAsync(string title, string message, string notificationType = "info", string entityType = null, string entityId = null)
        {
            var notification = new
            {
                Title = title,
                Message = message,
                EntityType = entityType,
                EntityId = entityId,
                Timestamp = DateTime.UtcNow.ToString("o"),
                Type = "custom_notification",
                NotificationType = notificationType
            };

            var payload = System.Text.Json.JsonSerializer.Serialize(notification);
            await BroadcastAsync(payload);
        }
    }
}
