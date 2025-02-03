using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotEFCore.Handlers.Interfaces;
using TelegramBotEFCore.Models;
using TelegramBotEFCore.Services;

namespace TelegramBotEFCore.Handlers.CommandHandlers
{
    public class AddSubjectHandler : IMessageHandler
    {
        private readonly BotMessageService _botMessageService;
        public AddSubjectHandler(BotMessageService botMessageService)
        {
            _botMessageService = botMessageService;
        }
        public async Task HandleMessageAsync(Message message, Dictionary<long, UserState> userStates)
        {
            var chatId = message.Chat.Id;
            if (userStates.TryGetValue(chatId, out var state) && state == UserState.Teacher)
            {
                await _botMessageService.SendAndStoreMessage(chatId, "Введите название предмета");
                userStates[chatId] = UserState.GettingSubjectData;
            }
            else
            {
                await _botMessageService.SendAndStoreMessage(chatId, "У вас недостаточно прав чтобы выполнить это действие");
            }
        }
    }
}
