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

namespace TelegramBotEFCore.Handlers.CallbackHandlers
{
    public class SubjectCallbackHandler : ICallbackHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly UsersRepository _usersRepository;
        private readonly SubjectsRepository _subjectsRepository;
        private readonly GroupsRepository _groupsRepository;
        private readonly TeachersRepository _teachersRepository;
        private readonly StudentsRepository _studentsRepository;
        private readonly MarksRepository _marksRepository;

        public SubjectCallbackHandler(
            ITelegramBotClient botClient,
            UsersRepository usersRepository,
            SubjectsRepository subjectsRepository,
            GroupsRepository groupsRepository,
            TeachersRepository teachersRepository,
            StudentsRepository studentsRepository,
            MarksRepository marksRepository) 
        {
            _botClient = botClient;
            _usersRepository = usersRepository;
            _subjectsRepository = subjectsRepository;
            _groupsRepository = groupsRepository;
            _teachersRepository = teachersRepository;
            _studentsRepository = studentsRepository;
            _marksRepository = marksRepository;
        }
        public async Task HandleCallbackAsync(CallbackQuery callbackQuery, Dictionary<long, UserState> userStates)
        {
            var chatId = callbackQuery.Message.Chat.Id;
            var subjectId = Guid.Parse(callbackQuery.Data.Replace("subject_", ""));
            var user = await _usersRepository.GetByTelegramId(chatId);
            var subject = await _subjectsRepository.GetById(subjectId);

            if (userStates.TryGetValue(chatId, out var state) && state == UserState.Teacher)
            {
                var teacher = await _teachersRepository.GetByUserId(user.Id);
                if (teacher == null)
                {
                    await _botClient.SendMessage(chatId, "Преподаватель не найден");
                    return;
                }
                if (teacher.CurrentGroupId == null)
                {
                    await _botClient.SendMessage(chatId, "Вы не выбрали группу ");
                    return;
                }

                var groupId = (Guid)teacher.CurrentGroupId;
                var group = await _groupsRepository.GetById(groupId);

                var studentIds = group.StudentIds;

                var students = await _studentsRepository.GetByIds(studentIds);

                await SendStudentsInlineKeyboard(students, chatId);

                await _teachersRepository.Update(teacher.Id,teacher.Name,teacher.CurrentGroupId,subjectId,teacher.CurrentStudentId);

            }
            else if (state == UserState.Student) 
            {
                var student = await _studentsRepository.GetByUserId(user.Id);
                if (student == null)
                {
                    await _botClient.SendMessage(chatId,"студент не найден");
                    return;
                }
                var marks = await _marksRepository.GetByStudentAndSubjectId(student.Id, subjectId);
                if (marks != null && marks.Count > 0)
                {
                    await SendMarksInlineKeyboard(marks, chatId, student.Name);
                }
                else
                {
                    await _botClient.SendMessage(chatId, "Вы ещё не получали отметки по данному предмету");
                }

            }
            else
            {
                await _botClient.SendMessage(chatId, "Сначало получите роль /getRole");
            }
        }
        private async Task SendStudentsInlineKeyboard(List<StudentEntity> students, long chatId)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(
                students.Select(s => InlineKeyboardButton.WithCallbackData(
                    text: s.Name,
                    callbackData: $"student_{s.Id}"
                )).Chunk(1)
            );

            await _botClient.SendMessage(
                chatId: chatId,
                text: "Выберите студента:",
                replyMarkup: inlineKeyboard
            );
        }
        private async Task SendMarksInlineKeyboard(List<MarkEntity> marks, long chatId, string name)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(
                marks.Select(m => InlineKeyboardButton.WithCallbackData(
                    text: m.Value.ToString(),
                    callbackData: $"mark_{m.Id}"
                )).Chunk(1)
            );
            float gpa = (float)marks.Sum(m => m.Value) / marks.Count;
            await _botClient.SendMessage(
                chatId: chatId,
                text: $"{name} Ваши отметки по данному предмету: \n" +
                $"Ваш средний балл {gpa:F2}",
                replyMarkup: inlineKeyboard
            );
        }
    }
}
