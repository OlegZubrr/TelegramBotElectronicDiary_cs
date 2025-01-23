using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotEFCore.DataBase.Models;
using TelegramBotEFCore.DataBase.Repositories;
using TelegramBotEFCore.Handlers.Interfaces;
using TelegramBotEFCore.Models;

namespace TelegramBotEFCore.Handlers.CommandHandlers
{
    public class GetGroupsHandler:IMessageHandler
    {
        private readonly ITelegramBotClient _botClient;
        private  GroupsRepository _groupsRepository;
        public GetGroupsHandler(ITelegramBotClient botClient,GroupsRepository groupsRepository) 
        {
            _botClient = botClient;
            _groupsRepository = groupsRepository;
        }

        public async Task HandleMessageAsync(Message message, Dictionary<long, UserState> userStates)
        {
            var chatId = message.Chat.Id;
            if (userStates.TryGetValue(chatId, out var state) && state == UserState.Teacher || state == UserState.Student)
            {
                var groups = await _groupsRepository.Get();
                await SendGroupsInlineKeyboardAsync(groups, chatId);
            }
            else 
            {
                await _botClient.SendMessage(chatId,"Сначало получите роль /getRole");
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

            await _botClient.SendMessage(
                chatId: chatId,
                text: "Выберите группу:",
                replyMarkup: inlineKeyboard
            );
        }
    }
}
