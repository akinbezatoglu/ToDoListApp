﻿
using Microsoft.Extensions.Hosting;

namespace ToDoListApp.Server.Data.Types
{
    public class User : IEntity
    {
        public int Id { get; private init; }
        public Guid ReferenceId { get; private init; } = Guid.NewGuid();
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Displayname { get; set; }
        public DateTime CreatedAtUtc { get; private init; } = DateTime.UtcNow;
        public List<TodoList> Todolists { get; init; } = [];
    }
}
