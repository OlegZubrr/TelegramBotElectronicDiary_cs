using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotEFCore.Handlers.Interfaces;
using TelegramBotEFCore.Models;

namespace TelegramBotEFCore.Handlers
{
    public class BecomeStudentHandler:IMessageHandler
    {
        private readonly ITelegramBotClient _botClient;

        public BecomeStudentHandler(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task HandleMessageAsync(Message message, Dictionary<long, UserState> userStates)
        {
            var chatId = message.Chat.Id;

            if (userStates.TryGetValue(chatId, out var state) && state == UserState.WaitingForRole)
            {
                userStates[chatId] = UserState.BecomingStudent;
                await _botClient.SendMessage(chatId, "Введите код который вы получили от администратора");   
            }
            else
            {
                await _botClient.SendMessage(chatId, "Сначала выполните команду /getRole.");
            }
        }

    }
}
