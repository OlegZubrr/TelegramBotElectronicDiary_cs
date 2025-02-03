using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotEFCore.DataBase.Models;
using TelegramBotEFCore.DataBase.Repositories;
using TelegramBotEFCore.Handlers.Interfaces;
using TelegramBotEFCore.Models;
using TelegramBotEFCore.Services;

namespace TelegramBotEFCore.Handlers.CallbackHandlers
{
    public class GroupCallbackHandler : ICallbackHandler
    {
        private readonly BotMessageService _botMessageService;
        private readonly TeachersRepository _teachersRepository;
        private readonly GroupsRepository _groupsRepository;
        private readonly UsersRepository _usersRepository;
        private readonly StudentsRepository _studentsRepository;
        private readonly SubjectsRepository _subjectsRepository;
        private readonly SubjectsService _subjectsService;

        public GroupCallbackHandler(
            BotMessageService botMessageService,
            GroupsRepository groupsRepository,
            TeachersRepository teachersRepository,
            UsersRepository usersRepository,
            StudentsRepository studentsRepository,
            SubjectsRepository subjectsRepository,
            SubjectsService subjectsService)
        {
            _botMessageService = botMessageService;
            _teachersRepository = teachersRepository;
            _groupsRepository = groupsRepository;
            _usersRepository = usersRepository;
            _studentsRepository = studentsRepository;
            _subjectsRepository = subjectsRepository;
            _subjectsService = subjectsService;
        }

        public async Task HandleCallbackAsync(CallbackQuery callbackQuery, Dictionary<long, UserState> userStates)
        {
            var chatId = callbackQuery.Message.Chat.Id;
            var groupId = Guid.Parse(callbackQuery.Data.Replace("group_", ""));

            var user = await _usersRepository.GetByTelegramId(chatId);
            var group = await _groupsRepository.GetById(groupId);
            var subjects = await _subjectsService.GetSubjects(chatId, groupId);
            if (group == null)
            {
                await _botMessageService.SendAndStoreMessage(chatId, "Выбранной вами группы не существует");
                return;
            }
            if (userStates.TryGetValue(chatId, out var state) && state == UserState.Teacher)
            {
                var teacher = await _teachersRepository.GetByUserId(user.Id);
                if (teacher != null)
                {
                    await _teachersRepository.Update(teacher.Id, teacher.Name, groupId, teacher.CurrentSubjectId, teacher.CurrentStudentId);
                    await _subjectsService.SendSubjectsInlineKeyboardAsync(chatId, group.Name, subjects);
                }
                else
                {
                    await _botMessageService.SendAndStoreMessage(chatId, "Преподаватель не найден.");
                }
            }
            else if (state == UserState.Student)
            {
                var student = await _studentsRepository.GetByUserId(user.Id);
                if (student.GroupId == groupId)
                {
                    await _subjectsService.SendSubjectsInlineKeyboardAsync(chatId, group.Name, subjects);
                    return;
                }
                if (student.GroupId == null)
                {
                    await _studentsRepository.Update(student.Id, user.Id, groupId, student.Name);
                    await _groupsRepository.AddStudent(group, student.Id);
                    await _botMessageService.SendAndStoreMessage(chatId, $"Вы успешно вступили в группу {group.Name}");
                }
                else
                {
                    await _botMessageService.SendAndStoreMessage(chatId, "Вы уже состоите в группе");
                }
            }
            else
            {
                await _botMessageService.SendAndStoreMessage(chatId, "сначало получите роль /getRole");
            }
        }
    }
}
