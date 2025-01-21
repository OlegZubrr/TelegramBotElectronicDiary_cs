using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotEFCore.Handlers.Interfaces;
using TelegramBotEFCore.Models;

namespace TelegramBotEFCore.Handlers.CommandHandlers
{
    public class AddSubjectHandler : IMessageHandler
    {
        private readonly ITelegramBotClient _botClient;
        public AddSubjectHandler(ITelegramBotClient botClient) 
        {
            _botClient = botClient;
        }
        public async Task HandleMessageAsync(Message message, Dictionary<long, UserState> userStates)
        {
            var chatId = message.Chat.Id;
            if (userStates.TryGetValue(chatId, out var state) && state == UserState.Teacher)
            {
                await _botClient.SendMessage(chatId, "Введите название предмета");
                userStates[chatId] = UserState.GettingSubjectData;
            }
            else 
            {
                await _botClient.SendMessage(chatId, "У вас недостаточно прав чтобы выполнить это действие");
            }
        }
    }
}
