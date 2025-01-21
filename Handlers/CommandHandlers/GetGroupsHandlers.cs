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
    public class GetGroupsHandlers:IMessageHandler
    {
        private ITelegramBotClient _botClient;
        private UsersRepository _usersRepository;
        private TeachersRepository _teachersRepository;
        private GroupsRepository _groupsRepository;

        public GetGroupsHandlers(
            ITelegramBotClient botClient,
            UsersRepository usersRepository,
            TeachersRepository teachersRepository,
            GroupsRepository groupsRepository

            ) 
        {
            _botClient = botClient;
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
                var groupsIds =  teacher.GroupsIds.ToList();

                var groups = await _groupsRepository.GetByIds(groupsIds);
                if (groups == null || !groups.Any())
                {
                    await _botClient.SendMessage(chatId, "Группы не найдены.");
                    return;
                }
                await SendGroupsInlineKeyboardAsync(groups,chatId);

            }
            else
            {
                await _botClient.SendMessage(chatId,"у вас недостаточно прав сделать это ");
            }
        }
        private async Task SendGroupsInlineKeyboardAsync( List<GroupEntity> groups,long chatId)
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
