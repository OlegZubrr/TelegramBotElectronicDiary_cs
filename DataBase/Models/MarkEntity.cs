using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotEFCore.DataBase.Models
{
    public class MarkEntity
    {
        public Guid Id { get; set; }
        public int Value { get; set; }  
        public StudentEntity Student { get; set; }
        public SubjectEntity Subject { get; set; }
    }
}
