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
    public class GetGroupsHandler : IMessageHandler
    {
        private readonly BotMessageService _botMessageService;
        private readonly GroupsRepository _groupsRepository;

        public GetGroupsHandler(BotMessageService botMessageService, GroupsRepository groupsRepository)
        {
            _botMessageService = botMessageService;
            _groupsRepository = groupsRepository;
        }

        public async Task HandleMessageAsync(Message message, Dictionary<long, UserState> userStates)
        {
            var chatId = message.Chat.Id;
            if (userStates.TryGetValue(chatId, out var state) && (state == UserState.Teacher || state == UserState.Student))
            {
                var groups = await _groupsRepository.Get();
                await SendGroupsInlineKeyboardAsync(groups, chatId, state);
            }
            else
            {
                await _botMessageService.SendAndStoreMessage(chatId, "Сначало получите роль /getRole");
            }
        }

        private async Task SendGroupsInlineKeyboardAsync(List<GroupEntity> groups, long chatId, UserState state)
        {
            string responce = "";
            if (state == UserState.Teacher)
            {
                responce = "";
            }
            var inlineKeyboard = new InlineKeyboardMarkup(
                groups.Select(g => InlineKeyboardButton.WithCallbackData(
                    text: g.Name,
                    callbackData: $"group{responce}_{g.Id}"
                )).Chunk(1)
            );
            await _botMessageService.SendAndStoreMessage(chatId, "Выберите группу:", inlineKeyboard);
        }
    }
}
