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
    public class CommandDispatcher
    {
        private readonly Dictionary<string, IMessageHandler> _handlers;
        private readonly ITelegramBotClient _botClient;
        private readonly Dictionary<long, UserState> _userStates = new Dictionary<long, UserState>();

        public CommandDispatcher(ITelegramBotClient botClient) 
        {
            _botClient = botClient;
            _handlers = new Dictionary<string, IMessageHandler> 
            {
                {"/start",new StartCommandHandler(botClient)},
                {"/getRole",new GetRoleCommandHandler(botClient)},
                {"/becomeStudent",new BecomeStudentHandler(botClient)},
                {"/becomeTeacher",new BecomeTeacherHandler(botClient)},
            };
        }
        public async Task DispatchAsync(Message message) 
        {
            if (message == null || string.IsNullOrEmpty(message.Text)) 
            {
                return;
            }
            if (_handlers.TryGetValue(message.Text, out var handler))
            {
                await handler.HandleMessageAsync(message,_userStates);
            }
            else
            {
                await HandleUnknownCommand(message);
            }

        }
        private async Task HandleUnknownCommand(Message message)
        {
            string response = "Извините, я не понимаю эту команду.";
            await _botClient.SendMessage(message.Chat.Id, response);
        }

    }
}
