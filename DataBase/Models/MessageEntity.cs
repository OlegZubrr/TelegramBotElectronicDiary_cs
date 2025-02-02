

namespace TelegramBotEFCore.DataBase.Models
{
    public class MessageEntity
    {
        public Guid Id { get; set; } 
        public Guid UserId { get; set; }
        public UserEntity User { get; set; }
        public long ChatId { get; set; } 
        public long MessageId { get; set; } 
    }
}
