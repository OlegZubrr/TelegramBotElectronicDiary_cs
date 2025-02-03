using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotEFCore.DataBase.Repositories;
using TelegramBotEFCore.Handlers.Interfaces;
using TelegramBotEFCore.Models;
using TelegramBotEFCore.Services;

namespace TelegramBotEFCore.Handlers.StateHandlers
{
    public class GettingSubjectDataHandler : IStateHandler
    {
        private readonly BotMessageService _botMessageService;
        private readonly SubjectsRepository _subjectsRepository;
        private readonly UsersRepository _usersRepository;
        private readonly TeachersRepository _teachersRepository;
        private readonly GroupsRepository _groupsRepository;

        public GettingSubjectDataHandler(
            BotMessageService botMessageService,
            SubjectsRepository subjectsRepository,
            UsersRepository usersRepository,
            TeachersRepository teachersRepository,
            GroupsRepository groupsRepository)
        {
            _botMessageService = botMessageService;
            _subjectsRepository = subjectsRepository;
            _usersRepository = usersRepository;
            _teachersRepository = teachersRepository;
            _groupsRepository = groupsRepository;
        }

        public async Task HandleAsync(Message message, Dictionary<long, UserState> userStates)
        {
            if (message == null || message.Text == null)
            {
                return;
            }
            string name = message.Text;
            var chatId = message.Chat.Id;
            var user = await _usersRepository.GetByTelegramId(chatId);
            var teacher = await _teachersRepository.GetByUserId(user.Id);
            userStates[chatId] = UserState.Teacher;
            if (teacher.CurrentGroupId == null)
            {
                await _botMessageService.SendAndStoreMessage(chatId, "Вы не выбрали группу \nвведите /getMyGroups чтобы сделать это");
                return;
            }
            Guid guid = Guid.NewGuid();
            Guid currentGroupId = (Guid)teacher.CurrentGroupId;
            var group = await _groupsRepository.GetById(currentGroupId);
            if (group == null)
            {
                await _botMessageService.SendAndStoreMessage(chatId, "текущей группы не существует");
                return;
            }
            await _subjectsRepository.Add(currentGroupId, group, guid, name);
            await _botMessageService.SendAndStoreMessage(chatId, $"предмет {name} был успешно добавлен");
        }
    }
}
