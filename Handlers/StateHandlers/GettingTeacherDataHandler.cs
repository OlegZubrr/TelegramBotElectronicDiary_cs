using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotEFCore.DataBase.Repositories;
using TelegramBotEFCore.Extensions;
using TelegramBotEFCore.Handlers.Interfaces;
using TelegramBotEFCore.Models;
using TelegramBotEFCore.Services;

namespace TelegramBotEFCore.Handlers.StateHandlers
{
    public class GettingTeacherDataHandler : IStateHandler
    {
        private readonly TeachersRepository _teachersRepository;
        private readonly UsersRepository _usersRepository;
        private readonly BotMessageService _botMessageService;

        public GettingTeacherDataHandler(TeachersRepository teachersRepository, UsersRepository usersRepository, BotMessageService botMessageService)
        {
            _teachersRepository = teachersRepository;
            _usersRepository = usersRepository;
            _botMessageService = botMessageService;
        }

        public async Task HandleAsync(Message message, Dictionary<long, UserState> userStates)
        {
            if (message == null || message.Text == null) return;
            var chatId = message.Chat.Id;
            string name = message.Text;
            if (!name.IsValidName())
            {
                await _botMessageService.SendAndStoreMessage(chatId, "Неправельный формат имени попробуйте ещё раз");
                return;
            }
            var user = await _usersRepository.GetByTelegramId(chatId);
            var guid = Guid.NewGuid();
            await _teachersRepository.Add(user.Id, guid, name);
            userStates[chatId] = UserState.Teacher;
            await _botMessageService.SendAndStoreMessage(chatId, $"Поздравляю 👏 {name} вы вошли как учитель \n ");
        }
    }
}
