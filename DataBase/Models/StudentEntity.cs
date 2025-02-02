

namespace TelegramBotEFCore.DataBase.Models
{
    public class StudentEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public UserEntity? User { get; set; }
        public GroupEntity? Group { get; set; }
        public Guid? GroupId { get; set; }
        public List<MarkEntity> Marks { get; set; } = new();
        public List<Guid> MessagesIds { get; set; } = new();
        public List<StudentMessageEntity> Messages { get; set; } = new();
    }
}
