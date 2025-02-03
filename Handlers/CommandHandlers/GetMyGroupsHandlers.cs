using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotEFCore.DataBase.Models;
using TelegramBotEFCore.DataBase.Repositories;
using TelegramBotEFCore.Handlers.Interfaces;
using TelegramBotEFCore.Models;
using TelegramBotEFCore.Services;

namespace TelegramBotEFCore.Handlers.CommandHandlers
{
    public class GetMyGroupsHandlers : IMessageHandler
    {
        private readonly BotMessageService _botMessageService;
        private readonly UsersRepository _usersRepository;
        private readonly TeachersRepository _teachersRepository;
        private readonly GroupsRepository _groupsRepository;

        public GetMyGroupsHandlers(
            BotMessageService botMessageService,
            UsersRepository usersRepository,
            TeachersRepository teachersRepository,
            GroupsRepository groupsRepository)
        {
            _botMessageService = botMessageService;
            _usersRepository = usersRepository;
            _teachersRepository = teachersRepository;
            _groupsRepository = groupsRepository;
        }

        public async Task HandleMessageAsync(Message message, Dictionary<long, UserState> userStates)
        {
            var chatId = message.Chat.Id;
            if (userStates.TryGetValue(chatId, out var state) && state == UserState.Teacher)
            {
                var user = await _usersRepository.GetByTelegramId(chatId);
                var teacher = await _teachersRepository.GetByUserId(user.Id);
                var groupsIds = teacher.GroupsIds.ToList();
                var groups = await _groupsRepository.GetByIds(groupsIds);
                if (groups == null || !groups.Any())
                {
                    await _botMessageService.SendAndStoreMessage(chatId, "Группы не найдены.");
                    return;
                }
                await SendGroupsInlineKeyboardAsync(groups, chatId);
            }
            else
            {
                await _botMessageService.SendAndStoreMessage(chatId, "у вас недостаточно прав сделать это ");
            }
        }

        private async Task SendGroupsInlineKeyboardAsync(List<GroupEntity> groups, long chatId)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(
                groups.Select(g => InlineKeyboardButton.WithCallbackData(
                    text: g.Name,
                    callbackData: $"group_{g.Id}"
                )).Chunk(1)
            );
            await _botMessageService.SendAndStoreMessage(chatId, "Выберите группу:", inlineKeyboard);
        }
    }
}
