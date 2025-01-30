using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotEFCore.DataBase.Repositories;
using TelegramBotEFCore.Handlers.Interfaces;
using TelegramBotEFCore.Models;

namespace TelegramBotEFCore.Handlers.CallbackHandlers
{
    public class DeleteMarkCallbackHandler : ICallbackHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly MarksRepository _marksRepository;

        public DeleteMarkCallbackHandler(ITelegramBotClient botClient, MarksRepository marksRepository)
        {
            _botClient = botClient;
            _marksRepository = marksRepository;
        }

        public async Task HandleCallbackAsync(CallbackQuery callbackQuery, Dictionary<long, UserState> userStates)
        {
            var markId = Guid.Parse(callbackQuery.Data.Replace("deleteMark_", ""));
            var chatId = callbackQuery.Message.Chat.Id;
            var mark = await _marksRepository.GetById(markId);
            if (mark == null) 
            {
                await _botClient.SendMessage(chatId,"Отметка не найдена");
                return;
            }

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
               {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Подтвердить ✅", $"acceptDelitingMark_{markId}"),
                        InlineKeyboardButton.WithCallbackData("Отклонить ❌", $"cancelDelitingMark_{markId}")
                    }
                });

            await _botClient.SendMessage(
                   chatId,
                   $"Вы действительно хотите удалить отметку {mark.Value}?",
                   replyMarkup: inlineKeyboard
               );

        }
    }
}
