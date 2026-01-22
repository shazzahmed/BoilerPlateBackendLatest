using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Database
{
    public static class DbMigrator
    {
        public static Task Migrate(this IApplicationBuilder app)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            using (IServiceScope? scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<SqlServerDbContext>().Database.Migrate();
                return Task.FromResult(0);
            }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
    }
}
