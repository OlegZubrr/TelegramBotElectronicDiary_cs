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
    public class GettingGroupDataHandler:IStateHandler
    {
        private readonly ITelegramBotClient _botClient;
        private GroupsRepository _groupsRepository;
        private TeachersRepository _teachersRepository;
        private UsersRepository _usersRepository;

        public GettingGroupDataHandler(
            ITelegramBotClient botClient,
            GroupsRepository groupsRepository,
            TeachersRepository teachersRepository,
            UsersRepository usersRepository
            ) 
        {
            _botClient = botClient;
            _groupsRepository = groupsRepository;
            _teachersRepository = teachersRepository;
            _usersRepository = usersRepository;
        }

        public async Task HandleAsync(Message message, Dictionary<long, UserState> userStates)
        {
            if(message == null || message.Text == null) return;

            var chatId = message.Chat.Id;
            string groupName = message.Text;

            var user = await _usersRepository.GetByTelegramId( chatId );
            var teacher = await _teachersRepository.GetByUserId(user.Id);
            var guid = Guid.NewGuid();
            
            await _groupsRepository.Add(teacher.Id,guid,groupName);

            userStates[chatId] = UserState.Teacher;

            await _botClient.SendMessage(chatId,$"группа {groupName} была добавлена ");


        }
    }
}
