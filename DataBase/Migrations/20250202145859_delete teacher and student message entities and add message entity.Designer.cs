﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TelegramBotEFCore.DataBase;

#nullable disable

namespace TelegramBotEFCore.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250202145859_delete teacher and student message entities and add message entity")]
    partial class deleteteacherandstudentmessageentitiesandaddmessageentity
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("TelegramBotEFCore.DataBase.Models.GroupEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("StudentIds")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("SubjectIds")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid?>("TeacherEntityId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("TeacherEntityId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("TelegramBotEFCore.DataBase.Models.MarkEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("StudentId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("SubjectId")
                        .HasColumnType("char(36)");

                    b.Property<int>("Value")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("StudentId");

                    b.HasIndex("SubjectId");

                    b.ToTable("Marks");
                });

            modelBuilder.Entity("TelegramBotEFCore.DataBase.Models.StudentEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("GroupId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.HasIndex("UserId");

                    b.ToTable("Students");
                });

            modelBuilder.Entity("TelegramBotEFCore.DataBase.Models.SubjectEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.ToTable("Subjects");
                });

            modelBuilder.Entity("TelegramBotEFCore.DataBase.Models.TeacherEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("CurrentGroupId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("CurrentStudentId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("CurrentSubjectId")
                        .HasColumnType("char(36)");

                    b.Property<string>("GroupsIds")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Teachers");
                });

            modelBuilder.Entity("TelegramBotEFCore.DataBase.Models.UserEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<long>("TelegramId")
                        .HasColumnType("bigint");

                    b.Property<string>("UserName")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TelegramBotEFCore.DataBase.Models.UserRoleVerificationEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("RegisterCode")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserRoleVerification");
                });

            modelBuilder.Entity("TelegramBotEFCore.DataBase.Models.GroupEntity", b =>
                {
                    b.HasOne("TelegramBotEFCore.DataBase.Models.TeacherEntity", null)
                        .WithMany("Groups")
                        .HasForeignKey("TeacherEntityId");
                });

            modelBuilder.Entity("TelegramBotEFCore.DataBase.Models.MarkEntity", b =>
                {
                    b.HasOne("TelegramBotEFCore.DataBase.Models.StudentEntity", "Student")
                        .WithMany("Marks")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TelegramBotEFCore.DataBase.Models.SubjectEntity", "Subject")
                        .WithMany("Marks")
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Student");

                    b.Navigation("Subject");
                });

            modelBuilder.Entity("TelegramBotEFCore.DataBase.Models.StudentEntity", b =>
                {
                    b.HasOne("TelegramBotEFCore.DataBase.Models.GroupEntity", "Group")
                        .WithMany("Students")
                        .HasForeignKey("GroupId");

                    b.HasOne("TelegramBotEFCore.DataBase.Models.UserEntity", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TelegramBotEFCore.DataBase.Models.SubjectEntity", b =>
                {
                    b.HasOne("TelegramBotEFCore.DataBase.Models.GroupEntity", "Group")
                        .WithMany("Subjects")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");
                });

            modelBuilder.Entity("TelegramBotEFCore.DataBase.Models.TeacherEntity", b =>
                {
                    b.HasOne("TelegramBotEFCore.DataBase.Models.UserEntity", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("TelegramBotEFCore.DataBase.Models.UserRoleVerificationEntity", b =>
                {
                    b.HasOne("TelegramBotEFCore.DataBase.Models.UserEntity", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("TelegramBotEFCore.DataBase.Models.GroupEntity", b =>
                {
                    b.Navigation("Students");

                    b.Navigation("Subjects");
                });

            modelBuilder.Entity("TelegramBotEFCore.DataBase.Models.StudentEntity", b =>
                {
                    b.Navigation("Marks");
                });

            modelBuilder.Entity("TelegramBotEFCore.DataBase.Models.SubjectEntity", b =>
                {
                    b.Navigation("Marks");
                });

            modelBuilder.Entity("TelegramBotEFCore.DataBase.Models.TeacherEntity", b =>
                {
                    b.Navigation("Groups");
                });
#pragma warning restore 612, 618
        }
    }
}
