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

namespace TelegramBotEFCore.Handlers.CallbackHandlers
{
    public class NewMarkCallbackHandler:ICallbackHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly UsersRepository _usersRepository;
        private readonly TeachersRepository _teachersRepository;
        private readonly MarksRepository _marksRepository;

        public NewMarkCallbackHandler(
            ITelegramBotClient botClient,
            UsersRepository usersRepository,
            TeachersRepository teachersRepository,
            MarksRepository marksRepository
            )
        {
            _botClient = botClient;
            _usersRepository = usersRepository;
            _teachersRepository = teachersRepository;
            _marksRepository = marksRepository;
        }

        public async Task HandleCallbackAsync(CallbackQuery callbackQuery, Dictionary<long, UserState> userStates)
        {
            var chatId = callbackQuery.Message.Chat.Id;
            var markVal = int.Parse(callbackQuery.Data.Replace("newMark_", ""));
            var user = await _usersRepository.GetByTelegramId(chatId);
            var teacher = await _teachersRepository.GetByUserId(user.Id);
            if (teacher == null) 
            {
                await _botClient.SendMessage(chatId,"преподаватель не найден");
                return;
            }
            
            if (teacher.CurrentStudentId == null) 
            {
                await _botClient.SendMessage(chatId,"Сначало выберие студента");
                return;
            }
            var studentId = (Guid)teacher.CurrentStudentId;
            if (teacher.CurrentSubjectId == null)
            {
                await _botClient.SendMessage(chatId, "Сначало выберие предмет");
                return;
            }
            var subjectId = (Guid)teacher.CurrentSubjectId; 
            Guid guid = Guid.NewGuid();

            await _marksRepository.Add(studentId, subjectId, guid,markVal);

            await _botClient.SendMessage(chatId, $"Отметка {markVal} была добавлена студенту ");


        }
    }
}
