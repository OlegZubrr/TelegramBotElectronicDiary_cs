
namespace TelegramBotEFCore.DataBase.Models
{
    public class TeacherEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public UserEntity User { get; set; }
        public List<GroupEntity> Groups { get; set; } = [];
        public List<Guid> GroupsIds { get; set; } = [];
        public Guid? CurrentGroupId { get; set; }
        public Guid? CurrentSubjectId { get; set; }
        public Guid? CurrentStudentId { get; set; }

    }
}
