using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Telegram.Bot.Types;
using Telegram.Bot;
using TelegramBotEFCore.Handlers.Interfaces;
using TelegramBotEFCore.Models;

namespace TelegramBotEFCore.Handlers
{
    public class BecomeTeacherHandler : IMessageHandler
    {
        private readonly ITelegramBotClient _botClient;

        public BecomeTeacherHandler(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task HandleMessageAsync(Message message, Dictionary<long, UserState> userStates)
        {
            var chatId = message.Chat.Id;

            if (userStates.TryGetValue(chatId, out var state) && state == UserState.WaitingForRole)
            {
                userStates[chatId] = UserState.BecomingTeacher;
                await _botClient.SendMessage(chatId, "Введите код который вы получили от администратора");
                string adminChatId = ConfigurationManager.AppSettings[nameof(adminChatId)];
                var guid = Guid.NewGuid();
                await _botClient.SendMessage(
                    adminChatId,
                    $"Пользователь {message.Chat.Username} запросил стать учителем.\n" +
                    $"Код подтверждения:\n🚨\n<code>{guid}</code> \n🚨",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html
                );
            }
            else
            {
                await _botClient.SendMessage(chatId, "Сначала выполните команду /getRole.");
            }
        }
    }
}
