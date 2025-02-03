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
using TelegramBotEFCore.Services;

namespace TelegramBotEFCore.Handlers.CommandHandlers
{
    public class GetRoleCommandHandler : IMessageHandler
    {
        private readonly BotMessageService _botMessageService;

        public GetRoleCommandHandler(BotMessageService botMessageService)
        {
            _botMessageService = botMessageService;
        }
        public async Task HandleMessageAsync(Message message, Dictionary<long, UserState> userStates)
        {
            var chatId = message.Chat.Id;

            if (userStates.TryGetValue(chatId, out var state) && state == UserState.None || state == UserState.WaitingForRole) 
            {
                userStates[chatId] = UserState.WaitingForRole;
                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Стать учеником", "becomeStudent"),
                        InlineKeyboardButton.WithCallbackData("Стать преподавателем", "becomeTeacher")
                    }
                });
                string responce = "Выберите действие:";
                await _botMessageService.SendAndStoreMessage(chatId,responce,inlineKeyboard);

            }
            else 
            {
                
                string responce = "Cейчас вы не можит сделать это";
                await _botMessageService.SendAndStoreMessage(chatId,responce);
            }
            

            
        }
    }
}
