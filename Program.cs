using Microsoft.AspNetCore.Components;
using Microsoft.VisualBasic;
using System;
using System.Configuration;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TelegramBotEFCore.DataBase;
using TelegramBotEFCore.DataBase.Repositories;
using TelegramBotEFCore.Handlers;
using TelegramBotEFCore.Infrastructure;
namespace TelegramBotEFCore
{
    internal class Program
    {

        static async Task Main(string[] args)
        {
            string botToken = ConfigurationManager.AppSettings[nameof(botToken)];
            var client = new TelegramBotClient(botToken);
            var commandDispatcher = new CommandDispatcher(client);
            var dbContext = new ApplicationDbContext();
            var usersRepository = new UsersRepository(dbContext);
            var telegramBot = new TelegramBot(botToken, commandDispatcher,usersRepository);
            telegramBot.Start();

            Console.WriteLine("Нажмите Enter для завершения...");
            Console.ReadLine();
        }

       
    }
}
