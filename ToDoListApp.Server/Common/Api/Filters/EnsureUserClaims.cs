using Microsoft.EntityFrameworkCore;
using ToDoListApp.Server.Authentication;
using ToDoListApp.Server.Common.Api.Results;
using ToDoListApp.Server.Data;
using ToDoListApp.Server.Data.Types;

namespace ToDoListApp.Server.Common.Api.Filters
{
    public class EnsureUserClaims<TRequest>(Func<TRequest, Guid> idSelector) : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var request = context.Arguments.OfType<TRequest>().Single();
            var id = idSelector(request);

            var userId = context.HttpContext.User.GetUserReferenceId();

            return userId switch
            {
                _ when id != userId => TypedResults.Forbid(),
                _ => await next(context)
            };
        }

        private record Entity(int Id, int UserId);
    }
}