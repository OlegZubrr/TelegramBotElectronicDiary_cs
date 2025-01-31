using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotEFCore.Models;

namespace TelegramBotEFCore.DataBase.Models
{
    public class StudentMessageEntity:MessageEntity
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public StudentEntity Student { get; set; }

        public MessageTypeEnum MessageType { get; set; }
    }
}
