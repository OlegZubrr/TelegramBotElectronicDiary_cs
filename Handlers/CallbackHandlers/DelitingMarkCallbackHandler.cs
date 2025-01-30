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
using TelegramBotEFCore.Services;

namespace TelegramBotEFCore.Handlers.CallbackHandlers
{
    public class DelitingMarkCallbackHandler:ICallbackHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly MarksRepository _marksRepository;
        private readonly MarksServise _marksServise;
        private readonly UsersRepository _usersRepository;
        private readonly TeachersRepository _teachersRepository;
        private readonly StudentsRepository _studentsRepository;

        public DelitingMarkCallbackHandler(
            ITelegramBotClient botClient,
            MarksRepository marksRepository,
            MarksServise marksServise,
            UsersRepository usersRepository,
            TeachersRepository teachersRepository,
            StudentsRepository studentsRepository
            )
        {
            _botClient = botClient;
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
                await _botClient.SendMessage(chatId,"Пользователь не найден");
                return;
            }
            var teacher = await _teachersRepository.GetByUserId(user.Id);
            if (teacher == null) 
            {
                await _botClient.SendMessage(chatId,"Преподаватель не найден");
                return;
            }
            if(teacher.CurrentSubjectId == null) 
            {
                await _botClient.SendMessage(chatId, "Сначало выберите предмет");
                return;
            }
            var subjectId = (Guid)teacher.CurrentSubjectId;
            if (teacher.CurrentStudentId == null)
            {
                await _botClient.SendMessage(chatId, "Сначало выберите студента");
                return;
            }
            var studentId = (Guid)teacher.CurrentStudentId;

            var student = await _studentsRepository.GetById(studentId);
            if (student == null) 
            {
                await _botClient.SendMessage(chatId,"Студент не найден");
                return;
            }

            

            var splitedCallback = callbackQuery.Data.Split('_');
            string asnwer = splitedCallback[0];
            Guid markId = Guid.Parse(splitedCallback[1]);
            var mark = await _marksRepository.GetById(markId);
            if (mark == null) 
            {
                await _botClient.SendMessage(chatId, "Отметка не найдена");
                return;
            }
            if (asnwer == "acceptDelitingMark")
            {
                await _marksRepository.Delete(markId);
                await _botClient.SendMessage(chatId, $"Отметка {mark.Value} была удалена");
            }
            else 
            {
                await _botClient.SendMessage(chatId, $"Отметка {mark.Value} не была удалена");
            }
            var marks = await _marksRepository.GetByStudentAndSubjectId(studentId, subjectId);

            await _marksServise.SendMarksInlineKeyboard(marks, chatId, student.Name);
        }
    }
}
