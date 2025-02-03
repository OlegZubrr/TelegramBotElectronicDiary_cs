using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotEFCore.Handlers.Interfaces;
using TelegramBotEFCore.Models;
using TelegramBotEFCore.Services;

namespace TelegramBotEFCore.Handlers.CommandHandlers
{
    public class AddGroupHandler : IMessageHandler
    {
        private readonly BotMessageService _botMessageService;

        public AddGroupHandler(BotMessageService botMessageService)
        {
            _botMessageService = botMessageService;
        }

        public async Task HandleMessageAsync(Message message, Dictionary<long, UserState> userStates)
        {
            var chatId = message.Chat.Id;
            if (userStates.TryGetValue(chatId, out var state) && state == UserState.Teacher)
            {
                await _botMessageService.SendAndStoreMessage(chatId, "Введите название группы");
                userStates[chatId] = UserState.GettingGroupData;
            }
            else
            {
                await _botMessageService.SendAndStoreMessage(chatId, "У вас недостаточно прав чтобы выполнить это действие");
            }
        }
    }
}
