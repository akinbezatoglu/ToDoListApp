using Microsoft.EntityFrameworkCore;
using ToDoListApp.Server.Data.Types;

namespace ToDoListApp.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<TodoList> Todolists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureUsersTable(modelBuilder);
            ConfigureTodolistsTable(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private static void ConfigureUsersTable(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<User>();

            builder.HasIndex(x => x.ReferenceId)
                .IsUnique();

            builder.HasIndex(x => x.Email)
                .IsUnique();

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.Fullname)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.Password)
                .IsRequired()
                .HasMaxLength(128);

            builder.HasMany(x => x.Todolists)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }

        private static void ConfigureTodolistsTable(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<TodoList>();

            builder.HasIndex(x => x.ReferenceId)
                .IsUnique();

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.Content)
                .HasMaxLength(750);
        }
    }
}
