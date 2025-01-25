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

namespace TelegramBotEFCore.Handlers.CallbackHandlers
{
    public class GettingRoleCallbackHandler : ICallbackHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly UsersRepository _usersRepository;
        public GettingRoleCallbackHandler(ITelegramBotClient botClient, UsersRepository usersRepository)
        {
            _botClient = botClient;
            _usersRepository = usersRepository;
        }

        public async Task HandleCallbackAsync(CallbackQuery callbackQuery, Dictionary<long, UserState> userStates)
        {
            var splitedCallback = callbackQuery.Data.Split('_');
            string asnwer = splitedCallback[0];
            Guid id = Guid.Parse(splitedCallback[1]);
            var user = await _usersRepository.GetById(id);
            var chatId = user.TelegramId;
            if (user == null) 
            {
                return;
            }
            if (asnwer == "cancel")
            {
                await _botClient.SendMessage(chatId, "Администратор отклонил ваш запрос");
                userStates[chatId] = UserState.None;
            }
            else 
            {
                if (userStates.TryGetValue(chatId, out var state) && state == UserState.BecomingTeacher)
                {
                    await _botClient.SendMessage(chatId, "Администратор принял ваш запрос \nВведите ваше имя");
                    userStates[chatId] = UserState.GettingTeacherData;
                }
                else if(state == UserState.BecomingStudent)
                {
                    await _botClient.SendMessage(chatId, "Администратор принял ваш запрос  \nВведите ваше имя");
                    userStates[chatId] = UserState.GettingStudentData;
                }
            }

        }
    }
}
