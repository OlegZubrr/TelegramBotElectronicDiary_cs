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
using TelegramBotEFCore.Services;
namespace TelegramBotEFCore
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string botToken = ConfigurationManager.AppSettings[nameof(botToken)] ?? throw new ConfigurationException("botToken");
            var client = new TelegramBotClient(botToken);
            var dbContext = new ApplicationDbContext();
            var usersRepository = new UsersRepository(dbContext);
            var userRoleVerificationRepository = new UserRoleVerificationRepository(dbContext);
            var teacherRepository = new TeachersRepository(dbContext);
            var studentsRepository = new StudentsRepository(dbContext);
            var userRoleService = new UserRoleService(usersRepository, teacherRepository, studentsRepository);
            var groupRepository = new GroupsRepository(dbContext);
            var subjectRepository = new SubjectsRepository(dbContext);
            var studentRepository = new StudentsRepository(dbContext);
            var commandDispatcher = new CommandDispatcher(client, userRoleVerificationRepository, usersRepository, teacherRepository, userRoleService, groupRepository, subjectRepository,studentsRepository);
            var telegramBot = new TelegramBot(botToken, commandDispatcher, usersRepository);
            telegramBot.Start();

            Console.WriteLine("Нажмите Enter для завершения...");
            Console.ReadLine();
        }  
    }
}
