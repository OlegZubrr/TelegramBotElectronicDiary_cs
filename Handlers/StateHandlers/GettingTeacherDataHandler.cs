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

namespace TelegramBotEFCore.Handlers
{
    public class GettingTeacherDataHandler : IStateHandler
    {
        private TeachersRepository _teachersRepository;
        private UsersRepository _usersRepository;
        private ITelegramBotClient _botClient;
        public GettingTeacherDataHandler(TeachersRepository teachersRepository, UsersRepository usersRepository,ITelegramBotClient botClient)
        {
            _teachersRepository = teachersRepository;
            _usersRepository = usersRepository;
            _botClient = botClient;
        }

        public async Task HandleAsync(Message message, Dictionary<long, UserState> userStates)
        {
            if(message == null || message.Text == null) return;

            string name = message.Text;
            var chatId = message.Chat.Id;
            var user = await _usersRepository.GetByTelegramId(chatId);
            var guid = Guid.NewGuid();

            await _teachersRepository.Add(user.Id,guid,name);
            userStates[chatId] = UserState.Teacher;

            await _botClient.SendMessage(chatId, $"Поздравляю 👏 {name} вы вошли как учитель");
        }
    }
}
