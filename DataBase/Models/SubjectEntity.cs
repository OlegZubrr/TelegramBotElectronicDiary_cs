
namespace TelegramBotEFCore.DataBase.Models
{
    public class SubjectEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public GroupEntity Group { get; set; }
        public Guid GroupId { get; set; }
        public List<MarkEntity> Marks { get; set; } = [];

    }

}
