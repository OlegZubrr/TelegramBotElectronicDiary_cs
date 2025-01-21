﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotEFCore.DataBase.Models;
using TelegramBotEFCore.DataBase.Repositories;
using TelegramBotEFCore.Handlers.CallbackHandlers;
using TelegramBotEFCore.Handlers.CommandHandlers;
using TelegramBotEFCore.Handlers.Interfaces;
using TelegramBotEFCore.Handlers.StateHandlers;
using TelegramBotEFCore.Models;
using TelegramBotEFCore.Services;

namespace TelegramBotEFCore.Handlers
{
    public class CommandDispatcher
    {
        private readonly Dictionary<string, IMessageHandler> _handlers;
        private readonly ITelegramBotClient _botClient;
        private readonly Dictionary<long, UserState> _userStates = new Dictionary<long, UserState>();
        private readonly Dictionary<UserState, IStateHandler> _stateHandlers;
        private readonly Dictionary<string, ICallbackHandler> _callbackHandlers;
        private readonly UserRoleVerificationRepository _userRoleVerificationEntity;
        private readonly UsersRepository _usersRepository;
        private readonly TeachersRepository _teachersRepository;
        private readonly UserRoleService _userRoleService;
        private readonly GroupsRepository _groupsRepository;
        private readonly SubjectsRepository _subjectsRepository;

        public CommandDispatcher
            (
            ITelegramBotClient botClient,
            UserRoleVerificationRepository userRoleVerificationRepository,
            UsersRepository usersRepository,
            TeachersRepository teachersRepository,
            UserRoleService userRoleService,
            GroupsRepository groupsRepository,
            SubjectsRepository subjectsRepository
            ) 
        {
            _botClient = botClient;
            _userRoleVerificationEntity = userRoleVerificationRepository;
            _usersRepository = usersRepository;
            _teachersRepository = teachersRepository;
            _userRoleService = userRoleService;
            _groupsRepository = groupsRepository;
            _subjectsRepository = subjectsRepository;
            _handlers = new Dictionary<string, IMessageHandler> 
            {
                {"/start",new StartCommandHandler(botClient)},
                {"/getRole",new GetRoleCommandHandler(botClient)},
                {"/becomeStudent",new BecomeStudentHandler(botClient)},
                {"/becomeTeacher",new BecomeTeacherHandler(botClient,userRoleVerificationRepository,usersRepository)},
                {"/addGroup",new AddGroupHandler(botClient)},
                {"/getGroups",new GetGroupsHandlers(botClient,usersRepository,teachersRepository,groupsRepository) },
                {"/addSubject",new AddSubjectHandler(botClient)},

            };
            _stateHandlers = new Dictionary<UserState, IStateHandler>
            {
                {UserState.BecomingTeacher,new BecomingTeacherHandler(botClient,userRoleVerificationRepository,usersRepository,teachersRepository) },
                {UserState.GettingTeacherData,new GettingTeacherDataHandler(teachersRepository,usersRepository,botClient) },
                {UserState.GettingGroupData,new GettingGroupDataHandler(botClient,groupsRepository,teachersRepository,usersRepository) },
                {UserState.GettingSubjectData,new GettingSubjectDataHandler(botClient,subjectsRepository,usersRepository,teachersRepository,groupsRepository) }
            };
            _callbackHandlers = new Dictionary<string, ICallbackHandler>
            {
                { "group_", new GroupCallbackHandler(botClient, groupsRepository,teachersRepository,usersRepository) }
            };
        }
        public async Task DispatchAsync(Message message) 
        {
            if (message == null || string.IsNullOrEmpty(message.Text)) 
            {
                return;
            }

            var userId = message.Chat.Id;

            if (!_userStates.TryGetValue(userId, out var currentState))
            {
                currentState = await _userRoleService.GetState(userId);
                _userStates[userId] = currentState;
            }
            Console.WriteLine(currentState);

            if (_handlers.TryGetValue(message.Text, out var handler))
            {
                await handler.HandleMessageAsync(message,_userStates);
            }
            else if (_stateHandlers.TryGetValue(currentState, out var statehandler)) 
            {
                await statehandler.HandleAsync(message,_userStates);
            }
            else
            {
                await HandleUnknownCommand(message);
            }
        }
        public async Task HandleCallbackQueryAsync(CallbackQuery callbackQuery)
        {
            if (callbackQuery == null || string.IsNullOrEmpty(callbackQuery.Data))
            {
                return;
            }
            var callbackData = callbackQuery.Data;
            var handlerEntry = _callbackHandlers.FirstOrDefault(h => callbackQuery.Data.StartsWith(h.Key));
            if (handlerEntry.Value != null)
            {
                await handlerEntry.Value.HandleCallbackAsync(callbackQuery);
            }
        }
        private async Task HandleUnknownCommand(Message message)
        {
            string response = "Извините, я не понимаю эту команду.";
            await _botClient.SendMessage(message.Chat.Id, response);
        }
    }
}
