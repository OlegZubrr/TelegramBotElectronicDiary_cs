using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotEFCore.DataBase.Repositories;
using TelegramBotEFCore.Handlers.Interfaces;
using TelegramBotEFCore.Models;
using TelegramBotEFCore.Services;

namespace TelegramBotEFCore.Handlers.CommandHandlers
{
    public class StartCommandHandler : IMessageHandler
    {
        private readonly BotMessageService _botMessageService;
        private readonly UsersRepository _usersRepository;
        private readonly MessagesRepository _messagesRepository;
        

        public StartCommandHandler(BotMessageService botMessageService, UsersRepository usersRepository,MessagesRepository messagesRepository)
        {
            _botMessageService = botMessageService;
            _usersRepository = usersRepository;
            _messagesRepository = messagesRepository;
        }
        public async Task HandleMessageAsync(Message message, Dictionary<long, UserState> userStates)
        {
            var chatId = message.Chat.Id;
            var user = await _usersRepository.GetByTelegramId(chatId);
            string responce = "Привет \n мои команды \n /getRole";
            await _botMessageService.SendAndStoreMessage(chatId,responce);

        }
    }
}
