using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotEFCore.Handlers.Interfaces;
using TelegramBotEFCore.Models;

namespace TelegramBotEFCore.Handlers.StateHandlers
{
    public class GetStudentReplyKeyBoard:IStateHandler
    {
        private readonly ITelegramBotClient _botClient;

        public GetStudentReplyKeyBoard(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task HandleAsync(Message message, Dictionary<long, UserState> userStates)
        {
            var replyKeyboard = new ReplyKeyboardMarkup(
              new[]
              {
                    new[]
                    {
                        new KeyboardButton("Получить список всех групп")
                    },
                    new[]
                    {
                        new KeyboardButton("Покинуть группу")
                    },
                    new[]
                    {
                        new KeyboardButton("Получить список предметов")
                    },
              })
            {
                ResizeKeyboard = true

            };
            await _botClient.SendMessage(
              chatId: message.Chat.Id,
              text: "Выберите команду:",
              replyMarkup: replyKeyboard
          );
        }
    }
}
