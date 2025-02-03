﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotEFCore.DataBase.Repositories;
using TelegramBotEFCore.Handlers.Interfaces;
using TelegramBotEFCore.Models;
using TelegramBotEFCore.Services;

namespace TelegramBotEFCore.Handlers.CallbackHandlers
{
    public class NewMarkCallbackHandler : ICallbackHandler
    {
        private readonly BotMessageService _botMessageService;
        private readonly UsersRepository _usersRepository;
        private readonly TeachersRepository _teachersRepository;
        private readonly MarksRepository _marksRepository;
        private readonly MarksServise _marksServise;
        private readonly StudentsRepository _studentsRepository;

        public NewMarkCallbackHandler(
            BotMessageService botMessageService,
            UsersRepository usersRepository,
            TeachersRepository teachersRepository,
            MarksRepository marksRepository,
            MarksServise marksServise,
            StudentsRepository studentsRepository)
        {
            _botMessageService = botMessageService;
            _usersRepository = usersRepository;
            _teachersRepository = teachersRepository;
            _marksRepository = marksRepository;
            _marksServise = marksServise;
            _studentsRepository = studentsRepository;
        }

        public async Task HandleCallbackAsync(CallbackQuery callbackQuery, Dictionary<long, UserState> userStates)
        {
            var chatId = callbackQuery.Message.Chat.Id;
            var markVal = int.Parse(callbackQuery.Data.Replace("newMark_", ""));
            var user = await _usersRepository.GetByTelegramId(chatId);
            var teacher = await _teachersRepository.GetByUserId(user.Id);
            if (teacher == null)
            {
                await _botMessageService.SendAndStoreMessage(chatId, "преподаватель не найден");
                return;
            }
            if (teacher.CurrentStudentId == null)
            {
                await _botMessageService.SendAndStoreMessage(chatId, "Сначало выберие студента");
                return;
            }
            var studentId = (Guid)teacher.CurrentStudentId;
            if (teacher.CurrentSubjectId == null)
            {
                await _botMessageService.SendAndStoreMessage(chatId, "Сначало выберие предмет");
                return;
            }
            var subjectId = (Guid)teacher.CurrentSubjectId;
            Guid guid = Guid.NewGuid();

            await _marksRepository.Add(studentId, subjectId, guid, markVal);

            var marks = await _marksRepository.GetByStudentAndSubjectId(studentId, subjectId);
            var student = await _studentsRepository.GetById(studentId);
            string studentName = student.Name;
            await _marksServise.SendMarksInlineKeyboard(marks, chatId, studentName);
            await _marksServise.SendNewMarksInlineKeyboard(chatId);
        }
    }
}
