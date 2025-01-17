using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace TelegramBotEFCore.DataBase
{
    public class ApplicationDbContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ToString();
        }
    }
}
