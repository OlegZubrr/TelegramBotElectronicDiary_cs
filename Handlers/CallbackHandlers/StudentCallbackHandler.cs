using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotEFCore.DataBase.Repositories;
using TelegramBotEFCore.Handlers.Interfaces;
using TelegramBotEFCore.Models;
using TelegramBotEFCore.Services;

namespace TelegramBotEFCore.Handlers.CallbackHandlers
{
    public class StudentCallbackHandler : ICallbackHandler
    {
        private readonly BotMessageService _botMessageService;
        private readonly UsersRepository _usersRepository;
        private readonly TeachersRepository _teachersRepository;
        private readonly StudentsRepository _studentsRepository;
        private readonly MarksRepository _marksRepository;
        private readonly MarksServise _marksServise;

        public StudentCallbackHandler(
            BotMessageService botMessageService,
            UsersRepository usersRepository,
            TeachersRepository teachersRepository,
            StudentsRepository studentsRepository,
            MarksRepository marksRepository,
            MarksServise marksServise)
        {
            _botMessageService = botMessageService;
            _usersRepository = usersRepository;
            _teachersRepository = teachersRepository;
            _studentsRepository = studentsRepository;
            _marksRepository = marksRepository;
            _marksServise = marksServise;
        }

        public async Task HandleCallbackAsync(CallbackQuery callbackQuery, Dictionary<long, UserState> userStates)
        {
            var chatId = callbackQuery.Message.Chat.Id;
            var studentId = Guid.Parse(callbackQuery.Data.Replace("student_", ""));
            var user = await _usersRepository.GetByTelegramId(chatId);
            if (user == null)
            {
                await _botMessageService.SendAndStoreMessage(chatId, "Пользователь не найден");
                return;
            }
            var student = await _studentsRepository.GetById(studentId);
            if (student == null)
            {
                await _botMessageService.SendAndStoreMessage(chatId, "Студент не найден");
                return;
            }
            if (userStates.TryGetValue(chatId, out var state) && state == UserState.Teacher)
            {
                var teacher = await _teachersRepository.GetByUserId(user.Id);
                if (teacher == null)
                {
                    await _botMessageService.SendAndStoreMessage(chatId, "Преподаватель не найден");
                    return;
                }
                if (teacher.CurrentGroupId == null)
                {
                    await _botMessageService.SendAndStoreMessage(chatId, "Сначало выберите группу");
                    return;
                }
                if (teacher.CurrentSubjectId == null)
                {
                    await _botMessageService.SendAndStoreMessage(chatId, "Сначало выберите предмет");
                    return;
                }
                await _teachersRepository.Update(teacher.Id, teacher.Name, teacher.CurrentStudentId, teacher.CurrentSubjectId, studentId);
                var subjectId = (Guid)teacher.CurrentSubjectId;
                var marks = await _marksRepository.GetByStudentAndSubjectId(studentId, subjectId);
                if (marks != null && marks.Count > 0)
                {
                    await _marksServise.SendMarksInlineKeyboard(marks, chatId, student.Name);
                }
                else
                {
                    await _botMessageService.SendAndStoreMessage(chatId, "У студента ещё нету отметок по этому предмету");
                }
                await _marksServise.SendNewMarksInlineKeyboard(chatId);
            }
            else
            {
                await _botMessageService.SendAndStoreMessage(chatId, "Сначало получите роль /getRole");
            }
        }
    }
}
