
namespace TelegramBotEFCore.DataBase.Models
{
    public class GroupEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<StudentEntity> Students { get; set; } = [];
        public List<Guid> StudentIds { get; set; } = [];
        public List<SubjectEntity> Subjects { get; set; } = [];
        public List<Guid> SubjectIds { get;set; } = [];
    }
}
