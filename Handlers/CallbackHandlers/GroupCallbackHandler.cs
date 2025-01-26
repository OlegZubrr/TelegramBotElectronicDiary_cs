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
    public class GroupCallbackHandler : ICallbackHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly TeachersRepository _teachersRepository;
        private readonly GroupsRepository _groupsRepository;
        private readonly UsersRepository _usersRepository;
        private readonly StudentsRepository _studentsRepository;

        public GroupCallbackHandler(
            ITelegramBotClient botClient,
            GroupsRepository groupsRepository,
            TeachersRepository teachersRepository,
            UsersRepository usersRepository,
            StudentsRepository studentsRepository) 
        {
            _botClient = botClient;
            _teachersRepository = teachersRepository;
            _groupsRepository = groupsRepository;
            _usersRepository = usersRepository;
            _studentsRepository = studentsRepository;
        }
        public async Task HandleCallbackAsync(CallbackQuery callbackQuery,Dictionary<long,UserState> userStates)
        {
            var chatId = callbackQuery.Message.Chat.Id;
            var groupId = Guid.Parse(callbackQuery.Data.Replace("group_", ""));
            var user = await _usersRepository.GetByTelegramId(chatId);
            var group = await _groupsRepository.GetById(groupId);
            if (group == null) 
            {
                await _botClient.SendMessage(chatId, "Выбранной вами группы не существует");
                return;
            }
            if (userStates.TryGetValue(chatId, out var state) && state == UserState.Teacher)
            {
                var teacher = await _teachersRepository.GetByUserId(user.Id);

                if (teacher != null)
                {
                    await _teachersRepository.Update(teacher.Id, teacher.Name, groupId,teacher.CurrentSubjectId,teacher.CurrentStudentId);

                    await _botClient.SendMessage(
                        chatId: chatId,
                        text: $"Вы выбрали группу: {group?.Name} \n "
                    );
                }
                else
                {
                    await _botClient.SendMessage(
                        chatId: chatId,
                        text: "Учитель не найден."
                    );
                }
            }
            else if (state == UserState.Student)
            {
                var student = await _studentsRepository.GetByUserId(user.Id);
                if (student.GroupId != null) 
                {
                    await _botClient.SendMessage(chatId, $"Вы не можете вступитьв группу {group.Name} т.к вы уже состоите в группе");
                    return;
                }
                await _studentsRepository.Update(student.Id,user.Id,groupId,student.Name);
                await _groupsRepository.AddStudent(group, student.Id);
                await _botClient.SendMessage(chatId,$"Вы успешно вступили в группу {group.Name}");
            }
            else 
            {
                await _botClient.SendMessage(chatId,"сначало получите роль /getRole");
            }
         
        }
    }
}
