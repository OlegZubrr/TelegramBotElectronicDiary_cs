using System;
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
        private readonly StudentsRepository _studentsRepository;
        private readonly MarksRepository _marksRepository;
        private readonly SubjectsService _subjectsService;

        public CommandDispatcher
            (
            ITelegramBotClient botClient,
            UserRoleVerificationRepository userRoleVerificationRepository,
            UsersRepository usersRepository,
            TeachersRepository teachersRepository,
            UserRoleService userRoleService,
            GroupsRepository groupsRepository,
            SubjectsRepository subjectsRepository,
            StudentsRepository studentsRepository,
            MarksRepository marksRepository,
            SubjectsService subjectsService
            ) 
        {
            _botClient = botClient;
            _userRoleVerificationEntity = userRoleVerificationRepository;
            _usersRepository = usersRepository;
            _teachersRepository = teachersRepository;
            _userRoleService = userRoleService;
            _groupsRepository = groupsRepository;
            _subjectsRepository = subjectsRepository;
            _studentsRepository = studentsRepository;
            _marksRepository = marksRepository;
            _subjectsService = subjectsService;
            _handlers = new Dictionary<string, IMessageHandler> 
            {
                {"/start",new StartCommandHandler(botClient)},
                {"/getRole",new GetRoleCommandHandler(botClient)},
                {"Добавить группу",new AddGroupHandler(botClient)},
                {"Получить список моих групп",new GetMyGroupsHandlers(botClient,usersRepository,teachersRepository,groupsRepository)},
                {"Добавить предмет",new AddSubjectHandler(botClient)},
                {"Получить список всех групп",new GetGroupsHandler(botClient,groupsRepository)},
                {"Покинуть группу",new LeaveGroupMessageHandler(botClient,usersRepository,studentsRepository,groupsRepository)},

            };
            _stateHandlers = new Dictionary<UserState, IStateHandler>
            {
                
                {UserState.GettingTeacherData,new GettingTeacherDataHandler(teachersRepository,usersRepository,botClient) },
                {UserState.GettingStudentData,new GettingStudentDataHandler(botClient,usersRepository,studentsRepository) },
                {UserState.GettingGroupData,new GettingGroupDataHandler(botClient,groupsRepository,teachersRepository,usersRepository) },
                {UserState.GettingSubjectData,new GettingSubjectDataHandler(botClient,subjectsRepository,usersRepository,teachersRepository,groupsRepository) },
                {UserState.Teacher,new GetTeacherReplyKeyboard(botClient)},
                {UserState.Student,new GetStudentReplyKeyBoard(botClient)},
            };
            _callbackHandlers = new Dictionary<string, ICallbackHandler>
            {
                {"group_", new GroupCallbackHandler(botClient, groupsRepository,teachersRepository,usersRepository,studentsRepository,subjectsRepository,subjectsService) },
                {"subject_",new SubjectCallbackHandler(botClient,usersRepository,subjectsRepository,groupsRepository,teachersRepository,studentsRepository,marksRepository) },
                {"student_",new StudentCallbackHandler(botClient,usersRepository,teachersRepository,studentsRepository,marksRepository) },
                {"newMark_",new NewMarkCallbackHandler(botClient,usersRepository,teachersRepository,marksRepository) },
                {"cancel_",new GettingRoleCallbackHandler(botClient,usersRepository) },
                {"accept_",new GettingRoleCallbackHandler(botClient,usersRepository) },
                {"becomeStudent",new BecomeStudentCallbackHandler(botClient,userRoleVerificationRepository,usersRepository)},
                {"becomeTeacher",new BecomeTeacherCallbackHandler(botClient,userRoleVerificationRepository,usersRepository)},
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
            Console.WriteLine(callbackQuery.Data);
            var chatId = callbackQuery.Message.Chat.Id;

            if (!_userStates.TryGetValue(chatId, out var currentState))
            {
                currentState = await _userRoleService.GetState(chatId);
                _userStates[chatId] = currentState;
            }
            Console.WriteLine(currentState);

            var callbackData = callbackQuery.Data;
            var handlerEntry = _callbackHandlers.FirstOrDefault(h => callbackQuery.Data.StartsWith(h.Key));
            if (handlerEntry.Value != null)
            {
                await handlerEntry.Value.HandleCallbackAsync(callbackQuery,_userStates);
            }
        }
        private async Task HandleUnknownCommand(Message message)
        {
            string response = "Извините, я не понимаю эту команду.";
            await _botClient.SendMessage(message.Chat.Id, response);
        }
    }
}
