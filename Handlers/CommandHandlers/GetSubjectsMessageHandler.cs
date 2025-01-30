﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotEFCore.DataBase.Repositories;
using TelegramBotEFCore.Handlers.Interfaces;
using TelegramBotEFCore.Models;
using TelegramBotEFCore.Services;

namespace TelegramBotEFCore.Handlers.CommandHandlers
{
    public class GetSubjectsMessageHandler:IMessageHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly UsersRepository _usersRepository;
        private readonly StudentsRepository _studentsRepository;
        private readonly TeachersRepository _teachersRepository;
        private readonly SubjectsService _subjectsService;
        private readonly GroupsRepository _groupsRepository;

        public GetSubjectsMessageHandler(
            ITelegramBotClient botClient,
            UsersRepository usersRepository,
            StudentsRepository studentsRepository,
            TeachersRepository teachersRepository,
            SubjectsService subjectsService,
            GroupsRepository groupsRepository
            )
        {
            _botClient = botClient;
            _usersRepository = usersRepository;
            _studentsRepository = studentsRepository;
            _teachersRepository = teachersRepository;
            _subjectsService = subjectsService;
            _groupsRepository = groupsRepository;
        }

        public async Task HandleMessageAsync(Message message, Dictionary<long, UserState> userStates)
        {
            var chatId = message.Chat.Id;
            var user = await _usersRepository.GetByTelegramId(chatId);
            if (user == null) 
            {
                await _botClient.SendMessage(chatId, "Пользователь не найден");
                return;
            }
            if (userStates.TryGetValue(chatId, out var state) && state == UserState.Teacher)
            {
                var teacher = await _teachersRepository.GetByUserId(user.Id);
                if (teacher == null) 
                {
                    await _botClient.SendMessage(chatId,"Преподаватель не найден");
                    return;
                }
                
                if (teacher.CurrentGroupId == null) 
                {
                    await _botClient.SendMessage(chatId,"Сначало выберете группу");
                    return;
                }
              

                if (teacher != null)
                {
                    var groupId = (Guid)teacher.CurrentGroupId;
                    var group = await _groupsRepository.GetById(groupId);
                    var subjects = await _subjectsService.GetSubjects(chatId,groupId);
                    await _teachersRepository.Update(teacher.Id, teacher.Name, groupId, teacher.CurrentSubjectId, teacher.CurrentStudentId);

                    await _subjectsService.SendSubjectsInlineKeyboardAsync(chatId, group.Name, subjects);
                }
                else
                {
                    await _botClient.SendMessage(
                        chatId: chatId,
                        text: "Преподаватель не найден."
                    );
                }
            }
            else if (state == UserState.Student)
            {
                var student = await _studentsRepository.GetByUserId(user.Id);
                if (student == null)
                {
                    await _botClient.SendMessage(chatId, "Студент не найден");
                    return;
                }
                
                if (student.GroupId == null) 
                {
                    await _botClient.SendMessage(chatId, "Сначало выберите группу");
                    return;

                }
                var groupId = (Guid)student.GroupId;
                var group = await _groupsRepository.GetById(groupId);
                if (group == null) 
                {
                    await _botClient.SendMessage(chatId,"Группа не найдена");
                    return;
                }
                var subjects = await _subjectsService.GetSubjects(chatId, groupId);
                await _subjectsService.SendSubjectsInlineKeyboardAsync(chatId, group.Name, subjects);

            }
            else
            {
                await _botClient.SendMessage(chatId, "сначало получите роль /getRole");
            }

        }
    }
}
