using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotEFCore.DataBase.Models;

namespace TelegramBotEFCore.Services
{
    public class MarksServise
    {
        private readonly BotMessageService _botMessageService;

        public MarksServise(BotMessageService botMessageService)
        {
            _botMessageService = botMessageService;
        }

        public async Task SendMarksInlineKeyboard(List<MarkEntity> marks, long chatId, string name)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(
                marks.Select(m => InlineKeyboardButton.WithCallbackData(
                    text: m.Value.ToString(),
                    callbackData: $"deleteMark_{m.Id}"
                )).Chunk(5)
            );

            float gpa = (float)marks.Sum(m => m.Value) / marks.Count;

            await _botMessageService.SendAndStoreMessage(
                chatId,
                $"Отметки студента {name}: \n" +
                $"Средний балл {gpa:F2} \n" +
                $"🚫 нажмите на отметку чтобы удалить",
                inlineKeyboard
            );
        }

        public async Task SendNewMarksInlineKeyboard(long chatId)
        {
            int[] marks = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var inlineKeyboard = new InlineKeyboardMarkup(
                marks.Select(m => InlineKeyboardButton.WithCallbackData(
                    text: m.ToString(),
                    callbackData: $"newMark_{m}"
                )).Chunk(3)
            );

            await _botMessageService.SendAndStoreMessage(
                chatId,
                "Нажмите чтобы добавить отметку:",
                inlineKeyboard
            );
        }
    }
}
