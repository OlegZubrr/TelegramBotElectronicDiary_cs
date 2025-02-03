using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotEFCore.DataBase.Repositories;
using TelegramBotEFCore.Handlers.Interfaces;
using TelegramBotEFCore.Models;
using TelegramBotEFCore.Services;

namespace TelegramBotEFCore.Handlers.StateHandlers
{
    public class GettingGroupDataHandler : IStateHandler
    {
        private readonly BotMessageService _botMessageService;
        private readonly GroupsRepository _groupsRepository;
        private readonly TeachersRepository _teachersRepository;
        private readonly UsersRepository _usersRepository;

        public GettingGroupDataHandler(
            BotMessageService botMessageService,
            GroupsRepository groupsRepository,
            TeachersRepository teachersRepository,
            UsersRepository usersRepository)
        {
            _botMessageService = botMessageService;
            _groupsRepository = groupsRepository;
            _teachersRepository = teachersRepository;
            _usersRepository = usersRepository;
        }

        public async Task HandleAsync(Message message, Dictionary<long, UserState> userStates)
        {
            if (message == null || message.Text == null) return;

            var chatId = message.Chat.Id;
            string groupName = message.Text;

            var user = await _usersRepository.GetByTelegramId(chatId);
            var teacher = await _teachersRepository.GetByUserId(user.Id);
            var guid = Guid.NewGuid();

            await _groupsRepository.Add(teacher.Id, guid, groupName);
            await _teachersRepository.AddGroup(teacher, guid);

            userStates[chatId] = UserState.Teacher;

            await _botMessageService.SendAndStoreMessage(chatId, $"группа {groupName} была добавлена ");
        }
    }
}
