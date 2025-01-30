using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotEFCore.DataBase.Models;

namespace TelegramBotEFCore.Services
{
    public class MarksServise
    {
        private readonly ITelegramBotClient _botClient;

        public MarksServise(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task SendMarksInlineKeyboard(List<MarkEntity> marks, long chatId, string name)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(
                marks.Select(m => InlineKeyboardButton.WithCallbackData(
                    text: m.Value.ToString(),
                    callbackData: $"mark_{m.Id}"
                )).Chunk(1)
            );
            float gpa = (float)marks.Sum(m => m.Value) / marks.Count;
            await _botClient.SendMessage(
                chatId: chatId,
                text: $"Отметки студунта {name}: \n" +
                $"Средний балл {gpa:F2}",
                replyMarkup: inlineKeyboard
            );
        }
    }
}
