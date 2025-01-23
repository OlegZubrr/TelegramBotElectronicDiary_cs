﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotEFCore.DataBase.Models;
using TelegramBotEFCore.DataBase.Repositories;
using TelegramBotEFCore.Handlers.Interfaces;
using TelegramBotEFCore.Models;

namespace TelegramBotEFCore.Handlers.CommandHandlers
{
    public class BecomeStudentHandler : IMessageHandler
    {
        private readonly ITelegramBotClient _botClient;
        private UserRoleVerificationRepository _userRoleVerificationRepository;
        private UsersRepository _usersRepository;

        public BecomeStudentHandler(
            ITelegramBotClient botClient,
            UserRoleVerificationRepository userRoleVerificationRepository,
            UsersRepository usersRepository
            )
        {
            _botClient = botClient;
            _usersRepository = usersRepository;
            _userRoleVerificationRepository = userRoleVerificationRepository;
        }

        public async Task HandleMessageAsync(Message message, Dictionary<long, UserState> userStates)
        {
            var chatId = message.Chat.Id;

            if (userStates.TryGetValue(chatId, out var state) && state == UserState.WaitingForRole)
            {
                var userEntity = await _usersRepository.GetByTelegramId(chatId);
                UserRoleVerificationEntity? userRoleVerification = await _userRoleVerificationRepository.GetByUserId(userEntity.Id);
                if (userRoleVerification != null)
                {
                    await _botClient.SendMessage(chatId, "Вы уже получили код дождитесь подтверждения ");
                    return;
                }
                userStates[chatId] = UserState.BecomingStudent;
                await _botClient.SendMessage(chatId, "Введите код который вы получили от администратора");
                string adminChatId = ConfigurationManager.AppSettings[nameof(adminChatId)];
                var registerCode = Guid.NewGuid();
                var guid = Guid.NewGuid();
                await _botClient.SendMessage(
                    adminChatId,
                    $"Пользователь {message.Chat.Username} запросил стать студентом.\n" +
                    $"Код подтверждения:\n🚨\n<code>{registerCode}</code> \n🚨",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html
                );
                await _userRoleVerificationRepository.Add(userEntity.Id, guid, registerCode);
            }
            else
            {
                await _botClient.SendMessage(chatId, "Сначала выполните команду /getRole.");
            }
        }

    }
}
