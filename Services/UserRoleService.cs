using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotEFCore.DataBase.Models;
using TelegramBotEFCore.DataBase.Repositories;
using TelegramBotEFCore.Models;

namespace TelegramBotEFCore.Services
{
    public class UserRoleService
    {
        private UsersRepository _usersRepository;
        private TeachersRepository _teachersRepository;
        private StudentsRepository _studentsRepository;

        public UserRoleService(UsersRepository usersRepository, TeachersRepository teachersRepository, StudentsRepository studentsRepository)
        {
            _usersRepository = usersRepository;
            _teachersRepository = teachersRepository;
            _studentsRepository = studentsRepository;
        }

        private async Task<bool> IsTeacher(Guid userId) 
        {
            var teacher = await _teachersRepository.GetByUserId(userId);
            return teacher != null;
        }
        private async Task<bool> IsStudent(Guid userId)
        {
            var student = await _studentsRepository.GetByUserId(userId);
            return student != null;
        }
        public async Task <UserState> GetState(long chatId) 
        {
            var user = await _usersRepository.GetByTelegramId(chatId);
            var userId = user.Id;
            if (await IsStudent(userId))
            {
                return UserState.Student;
            }
            if (await IsTeacher(userId)) 
            {
                return UserState.Teacher;
            }
            return UserState.None;
        }
    }
}
