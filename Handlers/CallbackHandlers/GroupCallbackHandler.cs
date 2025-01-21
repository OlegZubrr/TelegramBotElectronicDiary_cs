using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotEFCore.DataBase.Repositories;
using TelegramBotEFCore.Handlers.Interfaces;

namespace TelegramBotEFCore.Handlers.CallbackHandlers
{
    public class GroupCallbackHandler : ICallbackHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly TeachersRepository _teachersRepository;
        private readonly GroupsRepository _groupsRepository;
        private readonly UsersRepository _usersRepository;
        public GroupCallbackHandler(
            ITelegramBotClient botClient,
            GroupsRepository groupsRepository,
            TeachersRepository teachersRepository,
            UsersRepository usersRepository) 
        {
            _botClient = botClient;
            _teachersRepository = teachersRepository;
            _groupsRepository = groupsRepository;
            _usersRepository = usersRepository;
        }
        public async Task HandleCallbackAsync(CallbackQuery callbackQuery)
        {
            var chatId = callbackQuery.Message.Chat.Id;
            var groupId = Guid.Parse(callbackQuery.Data.Replace("group_", ""));
            var user = await _usersRepository.GetByTelegramId(chatId);
            var teacher = await _teachersRepository.GetByUserId(user.Id);

            if (teacher != null)
            {
                await _teachersRepository.Update(teacher.Id,teacher.Name,groupId);

                var group = await _groupsRepository.GetById(groupId);

                await _botClient.SendMessage(
                    chatId: callbackQuery.Message.Chat.Id,
                    text: $"Вы выбрали группу: {group?.Name}"
                );
            }
            else
            {
                await _botClient.SendMessage(
                    chatId: callbackQuery.Message.Chat.Id,
                    text: "Учитель не найден."
                );
            }
        }
    }
}
