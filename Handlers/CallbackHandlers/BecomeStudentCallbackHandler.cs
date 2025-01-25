using System;
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
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotEFCore.Handlers.CallbackHandlers
{
    public class BecomeStudentCallbackHandler : ICallbackHandler
    {
        private readonly ITelegramBotClient _botClient;
        private UserRoleVerificationRepository _userRoleVerificationRepository;
        private UsersRepository _usersRepository;

        public BecomeStudentCallbackHandler(
            ITelegramBotClient botClient,
            UserRoleVerificationRepository userRoleVerificationRepository,
            UsersRepository usersRepository
            )
        {
            _botClient = botClient;
            _usersRepository = usersRepository;
            _userRoleVerificationRepository = userRoleVerificationRepository;
        }

        public async Task HandleCallbackAsync(CallbackQuery callbackQuery, Dictionary<long, UserState> userStates)
        {
            var chatId = callbackQuery.Message.Chat.Id;
            if (userStates.TryGetValue(chatId, out var state) && state == UserState.WaitingForRole) 
            {
                var user = await _usersRepository.GetByTelegramId(chatId);

                await _botClient.SendMessage(chatId, "Дождитесь подтверждения от администратора");
                string adminChatId = ConfigurationManager.AppSettings[nameof(adminChatId)];

                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("принять", $"accept_{user.Id}"),
                        InlineKeyboardButton.WithCallbackData("отклонить", $"cancel_{user.Id}")
                    }
                });
                await _botClient.SendMessage(
                    adminChatId,
                    $"Пользователь {callbackQuery.Message.Chat.Username} запросил стать студентом.\n",
                    replyMarkup: inlineKeyboard
                );
                userStates[chatId] = UserState.BecomingStudent;
            }
            else 
            {
                await _botClient.SendMessage(chatId, "Сначала выполните команду /getRole.");
            }
        }

       

    }
}
