using Microsoft.EntityFrameworkCore;
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ToString();
            optionsBuilder.UseMySql(connectionString,ServerVersion.AutoDetect(connectionString));
        }
    }
}
