﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ToDoListApp.Server.Data;

#nullable disable

namespace ToDoListApp.Server.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240817145005_UpdateUsersTable")]
    partial class UpdateUsersTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ToDoListApp.Server.Data.Types.TodoList", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Content")
                        .HasMaxLength(750)
                        .HasColumnType("nvarchar(750)");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ReferenceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<DateTime?>("UpdatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ReferenceId")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("Todolists");
                });

            modelBuilder.Entity("ToDoListApp.Server.Data.Types.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Fullname")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<Guid>("ReferenceId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("ReferenceId")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ToDoListApp.Server.Data.Types.TodoList", b =>
                {
                    b.HasOne("ToDoListApp.Server.Data.Types.User", "User")
                        .WithMany("Todolists")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ToDoListApp.Server.Data.Types.User", b =>
                {
                    b.Navigation("Todolists");
                });
#pragma warning restore 612, 618
        }
    }
}
