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

        public async Task<long> SendMarksInlineKeyboard(List<MarkEntity> marks, long chatId, string name)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(
                marks.Select(m => InlineKeyboardButton.WithCallbackData(
                    text: m.Value.ToString(),
                    callbackData: $"deleteMark_{m.Id}"
                )).Chunk(1)
            );

            float gpa = (float)marks.Sum(m => m.Value) / marks.Count;

            var sentMessage = await _botClient.SendMessage(
                chatId: chatId,
                text: $"Отметки студента {name}: \n" +
                      $"Средний балл {gpa:F2} \n" +
                      $"🚫 нажмите на отметку чтобы удалить",
                replyMarkup: inlineKeyboard
            );

            return sentMessage.MessageId;
        }

        public async Task<long> SendNewMarksInlineKeyboard(long chatId)
        {
            int[] marks = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var inlineKeyboard = new InlineKeyboardMarkup(
                marks.Select(m => InlineKeyboardButton.WithCallbackData(
                    text: m.ToString(),
                    callbackData: $"newMark_{m}"
                )).Chunk(3)
            );

            var sentMessage = await _botClient.SendMessage(
                chatId: chatId,
                text: "Нажмите чтобы добавить отметку:",
                replyMarkup: inlineKeyboard
            );

            return sentMessage.MessageId;
        }

    }
}
