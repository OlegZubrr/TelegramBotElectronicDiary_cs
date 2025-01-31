using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotEFCore.DataBase.Models
{
    public abstract class MessageEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid(); 
        public long ChatId { get; set; } 
        public long MessageId { get; set; } 
    }
}
