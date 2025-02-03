using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotEFCore.DataBase.Repositories;
using TelegramBotEFCore.Handlers.Interfaces;
using TelegramBotEFCore.Models;
using TelegramBotEFCore.Services;

namespace TelegramBotEFCore.Handlers.CallbackHandlers
{
    public class BecomeStudentCallbackHandler : ICallbackHandler
    {
        private readonly BotMessageService _botMessageService;
        private readonly UserRoleVerificationRepository _userRoleVerificationRepository;
        private readonly UsersRepository _usersRepository;

        public BecomeStudentCallbackHandler(
            BotMessageService botMessageService,
            UserRoleVerificationRepository userRoleVerificationRepository,
            UsersRepository usersRepository)
        {
            _botMessageService = botMessageService;
            _userRoleVerificationRepository = userRoleVerificationRepository;
            _usersRepository = usersRepository;
        }

        public async Task HandleCallbackAsync(CallbackQuery callbackQuery, Dictionary<long, UserState> userStates)
        {
            var chatId = callbackQuery.Message.Chat.Id;
            if (userStates.TryGetValue(chatId, out var state) && state == UserState.WaitingForRole)
            {
                var user = await _usersRepository.GetByTelegramId(chatId);
                await _botMessageService.SendAndStoreMessage(chatId, "Дождитесь подтверждения от администратора");
                string adminChatId = ConfigurationManager.AppSettings[nameof(adminChatId)];
                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Принять ✅", $"accept_{user.Id}"),
                        InlineKeyboardButton.WithCallbackData("Отклонить ❌", $"cancel_{user.Id}")
                    }
                });
                await _botMessageService.SendAndStoreMessage(long.Parse(adminChatId), $"Пользователь {callbackQuery.Message.Chat.Username} запросил стать студентом.\n", inlineKeyboard);
                userStates[chatId] = UserState.BecomingStudent;
            }
            else
            {
                await _botMessageService.SendAndStoreMessage(chatId, "Сначала выполните команду /getRole.");
            }
        }
    }
}
