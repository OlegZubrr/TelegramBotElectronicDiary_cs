using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotEFCore.DataBase.Models
{
    public class GroupEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<StudentEntity> Students { get; set; } = [];
        public List<TeacherEntity> Teachers { get; set; } = [];
        public List<SubjectEntity> Subjects { get; set; } = [];

    }
}
