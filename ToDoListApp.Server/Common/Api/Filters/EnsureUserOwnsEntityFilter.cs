using Microsoft.EntityFrameworkCore;
using ToDoListApp.Server.Authentication;
using ToDoListApp.Server.Common.Api.Results;
using ToDoListApp.Server.Data;
using ToDoListApp.Server.Data.Types;

namespace ToDoListApp.Server.Common.Api.Filters
{
    public class EnsureUserOwnsEntityFilter<TRequest, TEntity>(ApplicationDbContext database, Func<TRequest, Guid> idSelector) : IEndpointFilter
        where TEntity : class, IEntity, IOwnedEntity
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var request = context.Arguments.OfType<TRequest>().Single();
            var cancellationToken = context.HttpContext.RequestAborted;
            var userReferenceId = context.HttpContext.User.GetUserReferenceId();
            var entityReferenceId = idSelector(request);

            var entity = await database
                .Set<TEntity>()
                .Where(x => x.ReferenceId == entityReferenceId)
                .Select(x => new Entity(x.Id, x.UserId))
                .SingleOrDefaultAsync(cancellationToken);

            var user = await database.Users
                .Where(x => x.ReferenceId == userReferenceId)
                .SingleOrDefaultAsync(cancellationToken);

            return (entity, user) switch
            {
                (null, _) => new NotFoundProblem($"{typeof(TEntity).Name} with id {entityReferenceId} was not found."),
                (_, null) => TypedResults.Unauthorized(),
                (_, _) when entity.UserId != user.Id => TypedResults.Forbid(),
                _ => await next(context)
            };
        }

        private record Entity(int Id, int UserId);
    }
}