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

namespace TelegramBotEFCore.Handlers.StateHandlers
{
    public class GettingSubjectDataHandler : IStateHandler
    {
        private readonly ITelegramBotClient _botClient;
        private SubjectsRepository _subjectsRepository;
        private UsersRepository _usersRepository;
        private TeachersRepository _teachersRepository;
        private GroupsRepository _groupsRepository;

        public GettingSubjectDataHandler(
            ITelegramBotClient botClient,
            SubjectsRepository subjectsRepository,
            UsersRepository usersRepository,
            TeachersRepository teachersRepository,
            GroupsRepository groupsRepository
            ) 
        {
            _botClient = botClient;
            _subjectsRepository = subjectsRepository;
            _usersRepository = usersRepository;
            _teachersRepository = teachersRepository;
            _groupsRepository = groupsRepository;
            
        }

        public async Task HandleAsync(Message message, Dictionary<long, UserState> userStates)
        {
            if (message == null || message.Text == null) 
            {
                return;
            }
            string name = message.Text;
            var chatId = message.Chat.Id;
            var user = await _usersRepository.GetByTelegramId( chatId );
            var teacher = await _teachersRepository.GetByUserId(user.Id);
            userStates[chatId] = UserState.Teacher;
            if (teacher.CurrentGroupId == null)
            {
                await _botClient.SendMessage(chatId,"Вы не выбрали группу \nвведите /getGroups чтобы сделать это");

                return;
            }
            Guid guid = Guid.NewGuid();
            Guid currentGroupId = (Guid)teacher.CurrentGroupId;
            var group = await _groupsRepository.GetById(currentGroupId);
            if (group == null) 
            {
                await _botClient.SendMessage(chatId, $"текущей группы не существует");
                return;
            }
            await _subjectsRepository.Add(currentGroupId,group,guid,name);
            await _botClient.SendMessage(chatId, $"предмет {name} был успешно добавлен");


        }
    }
}
