using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotEFCore.DataBase.Repositories;
using TelegramBotEFCore.Handlers.Interfaces;
using TelegramBotEFCore.Models;

namespace TelegramBotEFCore.Handlers.CommandHandlers
{
    public class LeaveGroupMessageHandler:IMessageHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly UsersRepository _usersRepository;
        private readonly StudentsRepository _studentsRepository;
        private readonly GroupsRepository _groupsRepository;

        public LeaveGroupMessageHandler(ITelegramBotClient botClient, UsersRepository usersRepository, StudentsRepository studentsRepository, GroupsRepository groupsRepository)
        {
            _botClient = botClient;
            _usersRepository = usersRepository;
            _studentsRepository = studentsRepository;
            _groupsRepository = groupsRepository;
        }

        public async Task HandleMessageAsync(Message message, Dictionary<long, UserState> userStates)
        {
            if (message == null || message.Text == null) return;

            var chatId = message.Chat.Id;
            var user = await _usersRepository.GetByTelegramId(chatId);
            var student = await _studentsRepository.GetByUserId(user.Id);
            if(student == null) 
            {
                await _botClient.SendMessage(chatId, "Студент не найден");
                return;
            }
            if (student.GroupId == null) 
            {
                await _botClient.SendMessage(chatId, "Вы не состоите в группе");
                return;
            }
            var groupId = (Guid)student.GroupId;
            var group = await _groupsRepository.GetById(groupId);
            if (group == null) 
            {
                await _botClient.SendMessage(chatId, "Группа не найдена");
                return;
            }
            group.StudentIds.Remove(student.Id);
            await _studentsRepository.Update(student.Id,student.UserId,null,student.Name);
            await _groupsRepository.Update(group);
            await _botClient.SendMessage(chatId,$"Вы успешно покинули группу {group.Name}");
        }
    }
}
