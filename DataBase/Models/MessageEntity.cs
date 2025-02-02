

namespace TelegramBotEFCore.DataBase.Models
{
    public abstract class MessageEntity
    {
        public Guid Id { get; set; } 
        public long ChatId { get; set; } 
        public long MessageId { get; set; } 
    }
}
