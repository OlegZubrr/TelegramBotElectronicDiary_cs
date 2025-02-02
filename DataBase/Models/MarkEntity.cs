

namespace TelegramBotEFCore.DataBase.Models
{
    public class MarkEntity
    {
        public Guid Id { get; set; }
        public int Value { get; set; }  
        public StudentEntity Student { get; set; }
        public Guid StudentId { get; set; }
        public SubjectEntity Subject { get; set; }
        public Guid SubjectId { get; set; }
    }
}
