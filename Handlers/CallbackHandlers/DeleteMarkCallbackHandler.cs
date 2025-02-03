using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotEFCore.DataBase.Repositories;
using TelegramBotEFCore.Handlers.Interfaces;
using TelegramBotEFCore.Models;
using TelegramBotEFCore.Services;

namespace TelegramBotEFCore.Handlers.CallbackHandlers
{
    public class DeleteMarkCallbackHandler : ICallbackHandler
    {
        private readonly BotMessageService _botMessageService;
        private readonly MarksRepository _marksRepository;

        public DeleteMarkCallbackHandler(BotMessageService botMessageService, MarksRepository marksRepository)
        {
            _botMessageService = botMessageService;
            _marksRepository = marksRepository;
        }

        public async Task HandleCallbackAsync(CallbackQuery callbackQuery, Dictionary<long, UserState> userStates)
        {
            var markId = Guid.Parse(callbackQuery.Data.Replace("deleteMark_", ""));
            var chatId = callbackQuery.Message.Chat.Id;
            var mark = await _marksRepository.GetById(markId);
            if (mark == null)
            {
                await _botMessageService.SendAndStoreMessage(chatId, "Отметка не найдена");
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

            await _botMessageService.SendAndStoreMessage(chatId, $"Вы действительно хотите удалить отметку {mark.Value}?", inlineKeyboard);
            
        }
    }
}
