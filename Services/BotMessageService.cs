using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotEFCore.DataBase.Repositories;

namespace TelegramBotEFCore.Services
{
    public class BotMessageService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly UsersRepository _usersRepository;
        private readonly MessagesRepository _messagesRepository;

        public BotMessageService (ITelegramBotClient botClient,UsersRepository usersRepository,MessagesRepository messagesRepository) 
        {
            _botClient = botClient;
            _usersRepository = usersRepository;
            _messagesRepository = messagesRepository;
        }

        public async Task SendAndStoreMessage(long chatId,string responce , InlineKeyboardMarkup? inlineKeyboard = null) 
        {
            var botMessage = await _botClient.SendMessage(
                chatId: chatId,
                text: responce,
                replyMarkup: inlineKeyboard
                );

            var messageId = botMessage.MessageId;
            var user = await _usersRepository.GetByTelegramId(chatId);
            var userId = (Guid)user.Id;
            var guid = Guid.NewGuid();

            await _messagesRepository.Add(
                userId: userId,
                id: guid,
                chatId: chatId,
                messageId: messageId,
                user:user
                );

            user.MesssageIds.Add(guid);

            await _usersRepository.Update(
                id: userId,
                telegramId: chatId,
                userName: user.UserName,
                messageIds: user.MesssageIds
                );


        } 



    }
}
