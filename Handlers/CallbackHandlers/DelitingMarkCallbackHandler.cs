using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotEFCore.DataBase.Repositories;
using TelegramBotEFCore.Handlers.Interfaces;
using TelegramBotEFCore.Models;
using TelegramBotEFCore.Services;

namespace TelegramBotEFCore.Handlers.CallbackHandlers
{
    public class DelitingMarkCallbackHandler : ICallbackHandler
    {
        private readonly BotMessageService _botMessageService;
        private readonly MarksRepository _marksRepository;
        private readonly MarksServise _marksServise;
        private readonly UsersRepository _usersRepository;
        private readonly TeachersRepository _teachersRepository;
        private readonly StudentsRepository _studentsRepository;

        public DelitingMarkCallbackHandler(
            BotMessageService botMessageService,
            MarksRepository marksRepository,
            MarksServise marksServise,
            UsersRepository usersRepository,
            TeachersRepository teachersRepository,
            StudentsRepository studentsRepository)
        {
            _botMessageService = botMessageService;
            _marksRepository = marksRepository;
            _marksServise = marksServise;
            _usersRepository = usersRepository;
            _teachersRepository = teachersRepository;
            _studentsRepository = studentsRepository;
        }

        public async Task HandleCallbackAsync(CallbackQuery callbackQuery, Dictionary<long, UserState> userStates)
        {
            var chatId = callbackQuery.Message.Chat.Id;
            var user = await _usersRepository.GetByTelegramId(chatId);
            if (user == null)
            {
                await _botMessageService.SendAndStoreMessage(chatId, "Пользователь не найден");
                return;
            }
            var teacher = await _teachersRepository.GetByUserId(user.Id);
            if (teacher == null)
            {
                await _botMessageService.SendAndStoreMessage(chatId, "Преподаватель не найден");
                return;
            }
            if (teacher.CurrentSubjectId == null)
            {
                await _botMessageService.SendAndStoreMessage(chatId, "Сначало выберите предмет");
                return;
            }
            var subjectId = (Guid)teacher.CurrentSubjectId;
            if (teacher.CurrentStudentId == null)
            {
                await _botMessageService.SendAndStoreMessage(chatId, "Сначало выберите студента");
                return;
            }
            var studentId = (Guid)teacher.CurrentStudentId;
            var student = await _studentsRepository.GetById(studentId);
            if (student == null)
            {
                await _botMessageService.SendAndStoreMessage(chatId, "Студент не найден");
                return;
            }
            var splitedCallback = callbackQuery.Data.Split('_');
            string answer = splitedCallback[0];
            Guid markId = Guid.Parse(splitedCallback[1]);
            var mark = await _marksRepository.GetById(markId);
            if (mark == null)
            {
                await _botMessageService.SendAndStoreMessage(chatId, "Отметка не найдена");
                return;
            }
            if (answer == "acceptDelitingMark")
            {
                await _marksRepository.Delete(markId);
                await _botMessageService.SendAndStoreMessage(chatId, $"Отметка {mark.Value} была удалена");
            }
            else
            {
                await _botMessageService.SendAndStoreMessage(chatId, $"Отметка {mark.Value} не была удалена");
            }
            var marks = await _marksRepository.GetByStudentAndSubjectId(studentId, subjectId);
            await _marksServise.SendMarksInlineKeyboard(marks, chatId, student.Name);
            await _marksServise.SendNewMarksInlineKeyboard(chatId);
        }
    }
}
