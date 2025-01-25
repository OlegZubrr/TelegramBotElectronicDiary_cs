using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotEFCore.Handlers.Interfaces;
using TelegramBotEFCore.Models;

namespace TelegramBotEFCore.Handlers.CommandHandlers
{
    public class GetRoleCommandHandler : IMessageHandler
    {
        private readonly ITelegramBotClient _botClient;

        public GetRoleCommandHandler(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }
        public async Task HandleMessageAsync(Message message, Dictionary<long, UserState> userStates)
        {
            var chatId = message.Chat.Id;

            if (userStates.TryGetValue(chatId, out var state) && state == UserState.None || state == UserState.WaitingForRole) 
            {
                userStates[chatId] = UserState.WaitingForRole;
                string responce = $"напишите {"\n"}/becomeStudent - чтобы стать учеником {"\n"}/becomeTeacher - чтобы стать учителем";
                await _botClient.SendMessage(message.Chat.Id, responce);
            }
            else 
            {
                await _botClient.SendMessage(chatId, "сейчас вы не можит сделать это");
            }
            

            
        }
    }
}
