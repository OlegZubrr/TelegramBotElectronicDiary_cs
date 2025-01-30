using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotEFCore.DataBase.Models;
using TelegramBotEFCore.DataBase.Repositories;

namespace TelegramBotEFCore.Services
{
    public class SubjectsService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly GroupsRepository _groupsRepository;
        private readonly SubjectsRepository _subjectsRepository;

        public SubjectsService(ITelegramBotClient botClient,GroupsRepository groupsRepository,SubjectsRepository subjectsRepository)
        {
            _botClient = botClient;
            _groupsRepository = groupsRepository;
            _subjectsRepository = subjectsRepository;
        }
        public async Task<List<SubjectEntity?>> GetSubjects(long chatId,Guid groupId) 
        {
            var group = await _groupsRepository.GetById(groupId);
            if (group == null)
            {
                return new List<SubjectEntity?>();
            }
            var subjects = await _subjectsRepository.GetByIds(group.SubjectIds);
            return subjects;
        }
        public async Task SendSubjectsInlineKeyboardAsync(long chatId, string groupName, List<SubjectEntity> subjects)
        {
            if (subjects == null || !subjects.Any())
            {
                await _botClient.SendMessage(chatId, "Нет доступных предметов для отображения.");
                return;
            }

            var inlineKeyboard = new InlineKeyboardMarkup(
                subjects.Select(s => InlineKeyboardButton.WithCallbackData(
                    text: s.Name,
                    callbackData: $"subject_{s.Id}"
                )).Chunk(1)
            );

            string message = $"Вы выбрали группу {groupName}\nВыберите предмет:";

            await _botClient.SendMessage(
                chatId: chatId,
                text: message,
                replyMarkup: inlineKeyboard
            );
        }
    }
}
