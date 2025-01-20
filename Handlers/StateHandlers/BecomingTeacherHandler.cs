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

namespace TelegramBotEFCore.Handlers.StateHandlers
{
    public class BecomingTeacherHandler : IStateHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly UserRoleVerificationRepository _userRoleVerificationRepository;
        private readonly UsersRepository _usersRepository;
        private readonly TeachersRepository _teachersRepository;

        public BecomingTeacherHandler(ITelegramBotClient botClient,
            UserRoleVerificationRepository userRoleVerificationRepository,
            UsersRepository usersRepository,
            TeachersRepository teachersRepository
            )
        {
            _botClient = botClient;
            _userRoleVerificationRepository = userRoleVerificationRepository;
            _usersRepository = usersRepository;
            _teachersRepository = teachersRepository;
        }
        public async Task HandleAsync(Message message, Dictionary<long, UserState> userStates)
        {
            if (message == null) return;
            var chatId = message.Chat.Id;
            var user = await _usersRepository.GetByTelegramId(chatId);
            var userRoleVerification = await _userRoleVerificationRepository.GetByUserId(user.Id);
            string registerCode = userRoleVerification.RegisterCode.ToString();
            if (registerCode != message.Text)
            {
                await _botClient.SendMessage(chatId, "неправельный код попробуйте ещё раз");
                return;
            }
            userStates[chatId] = UserState.GettingTeacherData;
            await _userRoleVerificationRepository.Delete(userRoleVerification.Id);
            Guid guid = Guid.NewGuid();
            await _botClient.SendMessage(chatId, "Введите своё имя");

        }

    }
}
