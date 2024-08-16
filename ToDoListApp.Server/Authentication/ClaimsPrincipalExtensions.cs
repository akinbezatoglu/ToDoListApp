using System.Security.Claims;
using ToDoListApp.Server.Data.Types;

namespace ToDoListApp.Server.Authentication
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            if (!int.TryParse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier), out var id))
            {
                // throw new InvalidOperationException("Invalid UserId");
                return 0;
            }

            return id;
        }
    }
}