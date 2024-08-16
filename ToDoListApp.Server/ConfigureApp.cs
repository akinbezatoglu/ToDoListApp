using Microsoft.EntityFrameworkCore;
using Serilog;
using ToDoListApp.Server.Data;

namespace ToDoListApp.Server
{
    public static class ConfigureApp
    {
        public static async Task Configure(this WebApplication app)
        {
            app.UseSerilogRequestLogging();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
            app.MapEndpoints();
            await app.EnsureDatabaseCreated();
        }

        private static async Task EnsureDatabaseCreated(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await db.Database.MigrateAsync();
        }
    }
}