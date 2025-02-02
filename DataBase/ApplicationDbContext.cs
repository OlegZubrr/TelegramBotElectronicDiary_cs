﻿using Microsoft.EntityFrameworkCore;
using System.Configuration;
using TelegramBotEFCore.DataBase.Models;

namespace TelegramBotEFCore.DataBase
{
    public class ApplicationDbContext:DbContext
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<StudentEntity> Students { get; set; }
        public DbSet<TeacherEntity> Teachers { get; set; }
        public DbSet<GroupEntity> Groups { get; set; }
        public DbSet<SubjectEntity> Subjects { get; set; }
        public DbSet<MarkEntity> Marks { get; set; }
        public DbSet<UserRoleVerificationEntity> UserRoleVerification { get; set; }
        public DbSet<TeacherMessageEntity> TeacherMessages { get; set; }
        public DbSet<StudentMessageEntity> StudentMessages { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ToString();
            optionsBuilder.UseMySql(connectionString,ServerVersion.AutoDetect(connectionString));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
             base.OnModelCreating(modelBuilder);

             modelBuilder.Entity<TeacherMessageEntity>()
                .Property(tm => tm.MessageType)
                .HasConversion<string>()
                .IsRequired();

             modelBuilder.Entity<StudentMessageEntity>()
                .Property(sm => sm.MessageType)
                .HasConversion<string>()
                .IsRequired();

            modelBuilder.Entity<TeacherMessageEntity>()
                .HasOne(tm => tm.Teacher)
                .WithMany(t => t.Messages)
                .HasForeignKey(tm => tm.TeacherId);

            modelBuilder.Entity<StudentMessageEntity>()
                .HasOne(sm => sm.Student)
                .WithMany(t => t.Messages)
                .HasForeignKey(sm => sm.StudentId);


        }
    }
}
