﻿using System;
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
using TelegramBotEFCore.Migrations;
using TelegramBotEFCore.Models;

namespace TelegramBotEFCore.Handlers.CommandHandlers
{
    public class GetSubjectsHandler:IMessageHandler
    {
        private readonly ITelegramBotClient _botClient;
        private UsersRepository _usersRepository;
        private TeachersRepository _teachersRepository;
        private GroupsRepository _groupsRepository;
        private SubjectsRepository _subjectsRepository;
        private StudentsRepository _studentsRepository;

        public GetSubjectsHandler(
            ITelegramBotClient botClient, 
            UsersRepository usersRepository, 
            TeachersRepository teachersRepository,
            GroupsRepository groupsRepository,
            SubjectsRepository subjectsRepository,
            StudentsRepository studentsRepository)
        {
            _botClient = botClient;
            _usersRepository = usersRepository;
            _teachersRepository = teachersRepository;
            _groupsRepository = groupsRepository;
            _subjectsRepository = subjectsRepository;
            _studentsRepository = studentsRepository;
        }

        public async Task HandleMessageAsync(Message message, Dictionary<long, UserState> userStates)
        {
            var chatId = message.Chat.Id;
            var user = await _usersRepository.GetByTelegramId(chatId);
            if (userStates.TryGetValue(chatId, out var state) && state == UserState.Teacher)
            {
                var teacher = await _teachersRepository.GetByUserId(user.Id);
                if (teacher.CurrentGroupId == null)
                {
                    await _botClient.SendMessage(chatId, "Вы не выбрали группу \nвведите /getGroups чтобы сделать это");
                    return;
                }
                var currentGroupId = (Guid)teacher.CurrentGroupId;

                var group = await _groupsRepository.GetById(currentGroupId);
                if (group == null) 
                {
                    await _botClient.SendMessage(chatId, "Выбранной группы не существует");
                    return;
                }
                var subjects = await _subjectsRepository.GetByIds(group.SubjectIds);
                await SendSubjecysInlineKeyboardAsync(subjects,chatId);
            }
            else if (state == UserState.Student) 
            {
                var student = await _studentsRepository.GetByUserId(user.Id);
                if (student == null) 
                {
                    await _botClient.SendMessage(chatId,"студент не найден");
                    return;
                }
                if (student.GroupId == null) 
                {
                    await _botClient.SendMessage(chatId, "Вы не выбрали группу \nвведите /getGroups чтобы сделать это");
                    return;
                }
                var groupId = (Guid)student.GroupId;
                var group = await _groupsRepository.GetById(groupId);
                if (group == null)
                {
                    await _botClient.SendMessage(chatId, "Выбранной группы не существует");
                    return;
                }
                var subjects = await _subjectsRepository.GetByIds(group.SubjectIds);
                await SendSubjecysInlineKeyboardAsync(subjects, chatId);
            }
            else 
            {
                await _botClient.SendMessage(chatId, "у вас недостаточно прав");
            }
        }
        private async Task SendSubjecysInlineKeyboardAsync(List<SubjectEntity> subjects, long chatId)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(
                subjects.Select(s => InlineKeyboardButton.WithCallbackData(
                    text: s.Name,
                    callbackData: $"subject_{s.Id}"
                )).Chunk(1)
            );

            await _botClient.SendMessage(
                chatId: chatId,
                text: "Выберите предмет:",
                replyMarkup: inlineKeyboard
            );
        }
    }
}
