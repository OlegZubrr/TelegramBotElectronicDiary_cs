using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotEFCore.DataBase.Repositories;
using TelegramBotEFCore.Handlers.Interfaces;
using TelegramBotEFCore.Models;
using TelegramBotEFCore.Services;

namespace TelegramBotEFCore.Handlers.CallbackHandlers
{
    public class GettingRoleCallbackHandler : ICallbackHandler
    {
        private readonly BotMessageService _botMessageService;
        private readonly UsersRepository _usersRepository;

        public GettingRoleCallbackHandler(BotMessageService botMessageService, UsersRepository usersRepository)
        {
            _botMessageService = botMessageService;
            _usersRepository = usersRepository;
        }

        public async Task HandleCallbackAsync(CallbackQuery callbackQuery, Dictionary<long, UserState> userStates)
        {
            var splitedCallback = callbackQuery.Data.Split('_');
            string answer = splitedCallback[0];
            Guid id = Guid.Parse(splitedCallback[1]);
            var user = await _usersRepository.GetById(id);
            if (user == null)
                return;
            var chatId = user.TelegramId;
            if (answer == "cancel")
            {
                await _botMessageService.SendAndStoreMessage(chatId, "Администратор отклонил ваш запрос");
                userStates[chatId] = UserState.None;
            }
            else
            {
                if (userStates.TryGetValue(chatId, out var state) && state == UserState.BecomingTeacher)
                {
                    await _botMessageService.SendAndStoreMessage(chatId, "Администратор принял ваш запрос \nВведите ваше имя");
                    userStates[chatId] = UserState.GettingTeacherData;
                }
                else if (state == UserState.BecomingStudent)
                {
                    await _botMessageService.SendAndStoreMessage(chatId, "Администратор принял ваш запрос \nВведите ваше имя");
                    userStates[chatId] = UserState.GettingStudentData;
                }
            }
        }
    }
}
