using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotEFCore.DataBase.Models;
using TelegramBotEFCore.DataBase.Repositories;
using TelegramBotEFCore.Handlers.Interfaces;
using TelegramBotEFCore.Models;
using TelegramBotEFCore.Services;

namespace TelegramBotEFCore.Handlers.CallbackHandlers
{
    public class StudentCallbackHandler : ICallbackHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly UsersRepository _usersRepository;
        private readonly TeachersRepository _teachersRepository;
        private readonly StudentsRepository _studentsRepository;
        private readonly MarksRepository _marksRepository;
        private readonly MarksServise _marksServise;
        
        public StudentCallbackHandler(
            ITelegramBotClient botClient,
            UsersRepository usersRepository,
            TeachersRepository teachersRepository,
            StudentsRepository studentsRepository,
            MarksRepository marksRepository,
            MarksServise marksServise
            ) 
        {
            _botClient = botClient;
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
                await _botClient.SendMessage(chatId,"Пользователь не найден");
                return;
            }
            var student = await _studentsRepository.GetById(studentId);
            if (student == null) 
            {
                await _botClient.SendMessage(chatId, "Студент не найден");
                return;
            }
            if (student == null)
            {
                await _botClient.SendMessage(chatId, "студент не найден");
            }
            if (userStates.TryGetValue(chatId, out var state) && state == UserState.Teacher) 
            {
                var teacher = await _teachersRepository.GetByUserId(user.Id);
                if(teacher == null) 
                {
                    await _botClient.SendMessage(chatId,"Преподаватель не найден");
                    return;
                }
                if(teacher.CurrentGroupId == null) 
                {
                    await _botClient.SendMessage(chatId, "Сначало выберите группу /getMyGroups");
                    return;
                }
                if (teacher.CurrentSubjectId == null)
                {
                    await _botClient.SendMessage(chatId, "Сначало выберите предмет");
                    return;
                }
                await _teachersRepository.Update(teacher.Id,teacher.Name,teacher.CurrentStudentId,teacher.CurrentSubjectId,studentId);
                var subjectId = (Guid)teacher.CurrentSubjectId;
                var marks = await _marksRepository.GetByStudentAndSubjectId(studentId, subjectId);
                if (marks != null && marks.Count > 0) 
                {
                    await _marksServise.SendMarksInlineKeyboard(marks,chatId,student.Name);
                }
                else
                {
                    await _botClient.SendMessage(chatId, "У студента ещё нету отметок по этому предмету");
                }
                await GetMarksInlineKeyboard(chatId);
            }
            else
            {
                await _botClient.SendMessage(chatId,"сначало получите роль /getRole");
            }
        }
       
        private async Task GetMarksInlineKeyboard(long chatId)
        {
            int[] marks = {0,1,2,3,4,5,6,7,8,9,10};
            var inlineKeyboard = new InlineKeyboardMarkup(
               marks.Select(m => InlineKeyboardButton.WithCallbackData(
                   text: m.ToString(),
                   callbackData: $"newMark_{m}"
               )).Chunk(3)
           );

            await _botClient.SendMessage(
              chatId: chatId,
              text: "Нажмите чтобы добавить отметку:",
              replyMarkup: inlineKeyboard
          );
        }
    }
}
