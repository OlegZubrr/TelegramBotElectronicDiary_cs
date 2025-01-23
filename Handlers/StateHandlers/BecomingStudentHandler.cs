using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using TelegramBotEFCore.DataBase.Repositories;
using TelegramBotEFCore.Models;
using TelegramBotEFCore.Handlers.Interfaces;

namespace TelegramBotEFCore.Handlers.StateHandlers
{
    public class BecomingStudentHandler:IStateHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly UserRoleVerificationRepository _userRoleVerificationRepository;
        private readonly UsersRepository _usersRepository;

        public BecomingStudentHandler(ITelegramBotClient botClient,
            UserRoleVerificationRepository userRoleVerificationRepository,
            UsersRepository usersRepository
            )
        {
            _botClient = botClient;
            _userRoleVerificationRepository = userRoleVerificationRepository;
            _usersRepository = usersRepository;
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
            userStates[chatId] = UserState.GettingStudentData;
            await _userRoleVerificationRepository.Delete(userRoleVerification.Id);
            Guid guid = Guid.NewGuid();
            await _botClient.SendMessage(chatId, "Введите своё имя");

        }
    }
}
