
using System.Threading.Tasks;

namespace TelegramBotEFCore.DataBase.Models
{
    public class UserRoleVerificationEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public UserEntity User { get; set; }
        public Guid RegisterCode { get; set; }
    }
}
