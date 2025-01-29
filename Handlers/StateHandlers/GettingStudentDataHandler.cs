using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotEFCore.DataBase.Repositories;
using TelegramBotEFCore.Extensions;
using TelegramBotEFCore.Handlers.Interfaces;
using TelegramBotEFCore.Models;

namespace TelegramBotEFCore.Handlers.StateHandlers
{
    public class GettingStudentDataHandler:IStateHandler
    {
        private StudentsRepository _studentsRepository;
        private UsersRepository _usersRepository;
        private ITelegramBotClient _botClient;
        public GettingStudentDataHandler(ITelegramBotClient botClient,UsersRepository usersRepository ,StudentsRepository studentsRepository)
        {
           
            _usersRepository = usersRepository;
            _botClient = botClient;
            _studentsRepository = studentsRepository;
        }

        public async Task HandleAsync(Message message, Dictionary<long, UserState> userStates)
        {
            if (message == null || message.Text == null) return;

            var chatId = message.Chat.Id;
            string name = message.Text;
            if (!name.IsValidName()) 
            {
                await _botClient.SendMessage(chatId, $"Неправельный формат имени попробуйте ещё раз");
                return;
            }

            var user = await _usersRepository.GetByTelegramId(chatId);
            var guid = Guid.NewGuid();

            await _studentsRepository.Add(user.Id, guid, name);
            userStates[chatId] = UserState.Student;

            await _botClient.SendMessage(chatId, $"Поздравляю 👏 {name} вы вошли как студент \n");
        }

    }
}
