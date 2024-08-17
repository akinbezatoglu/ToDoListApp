using System.Security.Claims;
using ToDoListApp.Server.Data.Types;

namespace ToDoListApp.Server.Authentication
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserReferenceId(this ClaimsPrincipal claimsPrincipal)
        {
            if (!Guid.TryParse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier), out var id))
            {
                // throw new InvalidOperationException("Invalid UserId");
                return Guid.Empty;
            }

            return id;
        }
    }
}