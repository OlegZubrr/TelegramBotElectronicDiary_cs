

namespace TelegramBotEFCore.DataBase.Models
{
    public class UserEntity
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public long TelegramId {  get; set; }
    }
}
