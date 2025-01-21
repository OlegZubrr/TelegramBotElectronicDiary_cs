using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TelegramBotEFCore.DataBase.Repositories;
using TelegramBotEFCore.Handlers;

namespace TelegramBotEFCore.Infrastructure
{
    public class TelegramBot
    {
        private readonly ITelegramBotClient _botClient;
        private readonly CommandDispatcher _commandDispatcher;
        private readonly UsersRepository _usersRepository;

        public TelegramBot(string token ,CommandDispatcher commandDispatcher,UsersRepository usersRepository) 
        {
            _botClient = new TelegramBotClient(token);
            _commandDispatcher = commandDispatcher;
            _usersRepository = usersRepository;
        }
        public void Start() 
        {
            _botClient.StartReceiving(
                 HandleUpdateAsync,
                 HandleErrorAsync
                );
            Console.WriteLine("Бот запущен...");
        }
        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var message = update.Message;

            if (message == null)
            {
                return;
            }
            var user = await _usersRepository.GetOrAddUserAsync(message.Chat.Id, message.Chat.Username);
            await _commandDispatcher.DispatchAsync(message);
    
        }
        private static async Task HandleErrorAsync(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
        }
    }
}
