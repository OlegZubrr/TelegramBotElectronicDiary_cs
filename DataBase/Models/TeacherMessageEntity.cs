using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotEFCore.Models;

namespace TelegramBotEFCore.DataBase.Models
{
    public class TeacherMessageEntity:MessageEntity
    {
        public Guid Id { get; set; }
        public Guid TeacherId { get; set; }
        public TeacherEntity Teacher { get; set; }
        public MessageTypeEnum MessageType { get; set; }
    }
}
