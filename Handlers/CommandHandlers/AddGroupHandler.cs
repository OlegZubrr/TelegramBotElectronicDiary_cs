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

namespace TelegramBotEFCore.Handlers.CommandHandlers
{
    public class AddGroupHandler : IMessageHandler
    {
        private readonly ITelegramBotClient _botClient;
        public AddGroupHandler(
            ITelegramBotClient botClient
            )
        {
            _botClient = botClient;
        }

        public async Task HandleMessageAsync(Message message, Dictionary<long, UserState> userStates)
        {
            var chatId = message.Chat.Id;
            if (userStates.TryGetValue(chatId, out var state) && state == UserState.Teacher)
            {
                await _botClient.SendMessage(chatId, "Введите название группы");
                userStates[chatId] = UserState.GettingGroupData;

            }
            else
            {
                await _botClient.SendMessage(chatId, "У вас недостаточно прав чтобы выполнить это действие");
            }
        }
    }
}
