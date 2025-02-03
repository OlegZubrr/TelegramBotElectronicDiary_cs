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
        private readonly BotMessageService _botMessageService;
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
        private readonly MarksServise _marksServise;
        private readonly MessagesRepository _messagesRepository;

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
            SubjectsService subjectsService,
            MarksServise marksServise,
            MessagesRepository messagesRepository,
            BotMessageService botMessageService
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
            _marksServise = marksServise;
            _messagesRepository = messagesRepository;
            _botMessageService = botMessageService;
            _handlers = new Dictionary<string, IMessageHandler> 
            {
                {"/start",new StartCommandHandler(botMessageService,usersRepository,messagesRepository)},
                {"/getRole",new GetRoleCommandHandler(botMessageService)},
                {"Добавить группу",new AddGroupHandler(botMessageService)},
                {"Получить список моих групп",new GetMyGroupsHandlers(botMessageService,usersRepository,teachersRepository,groupsRepository)},
                {"Добавить предмет",new AddSubjectHandler(botMessageService)},
                {"Получить список всех групп",new GetGroupsHandler(botMessageService,groupsRepository)},
                {"Покинуть группу",new LeaveGroupMessageHandler(botMessageService,usersRepository,studentsRepository,groupsRepository)},
                {"Получить список предметов",new GetSubjectsMessageHandler(botMessageService,usersRepository,studentsRepository,teachersRepository,subjectsService,groupsRepository) }
                
            };
            _stateHandlers = new Dictionary<UserState, IStateHandler>
            {
                
                {UserState.GettingTeacherData,new GettingTeacherDataHandler(teachersRepository,usersRepository,botMessageService) },
                {UserState.GettingStudentData,new GettingStudentDataHandler(botMessageService,usersRepository,studentsRepository) },
                {UserState.GettingGroupData,new GettingGroupDataHandler(botMessageService,groupsRepository,teachersRepository,usersRepository) },
                {UserState.GettingSubjectData,new GettingSubjectDataHandler(botMessageService,subjectsRepository,usersRepository,teachersRepository,groupsRepository) },
                {UserState.Teacher,new GetTeacherReplyKeyboard(botClient)},
                {UserState.Student,new GetStudentReplyKeyBoard(botClient)},
            };
            _callbackHandlers = new Dictionary<string, ICallbackHandler>
            {
                {"group_", new GroupCallbackHandler(botMessageService, groupsRepository,teachersRepository,usersRepository,studentsRepository,subjectsRepository,subjectsService) },
                {"subject_",new SubjectCallbackHandler(botMessageService,usersRepository,subjectsRepository,groupsRepository,teachersRepository,studentsRepository,marksRepository) },
                {"student_",new StudentCallbackHandler(botMessageService,usersRepository,teachersRepository,studentsRepository,marksRepository,marksServise) },
                {"newMark_",new NewMarkCallbackHandler(botMessageService,usersRepository,teachersRepository,marksRepository,marksServise,studentsRepository) },
                {"cancel_",new GettingRoleCallbackHandler(botMessageService,usersRepository) },
                {"accept_",new GettingRoleCallbackHandler(botMessageService,usersRepository) },
                {"becomeStudent",new BecomeStudentCallbackHandler(botMessageService,userRoleVerificationRepository,usersRepository)},
                {"becomeTeacher",new BecomeTeacherCallbackHandler(botMessageService,userRoleVerificationRepository,usersRepository)},
                {"deleteMark_",new DeleteMarkCallbackHandler(botMessageService,marksRepository) },
                {"acceptDelitingMark_",new DelitingMarkCallbackHandler(botMessageService,marksRepository,marksServise,usersRepository,teachersRepository,studentsRepository) },
                {"cancelDelitingMark_",new DelitingMarkCallbackHandler(botMessageService, marksRepository, marksServise,usersRepository, teachersRepository, studentsRepository) }
            };
        }
        public async Task DispatchAsync(Message message) 
        {
            if (message == null || string.IsNullOrEmpty(message.Text)) 
            {
                return;
            }
            var chatId = message.Chat.Id;
            

            if (!_userStates.TryGetValue(chatId, out var currentState))
            {
                currentState = await _userRoleService.GetState(chatId);
                _userStates[chatId] = currentState;
            }
            Console.WriteLine(currentState);

            if (_handlers.TryGetValue(message.Text, out var handler))
            {
                await DeletePreviousMessages(chatId);

                await handler.HandleMessageAsync(message,_userStates);
           
            }
            else if (_stateHandlers.TryGetValue(currentState, out var statehandler)) 
            {
                await DeletePreviousMessages(chatId);

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
                await DeletePreviousMessages(chatId);

                await handlerEntry.Value.HandleCallbackAsync(callbackQuery,_userStates);
            }
        }
        private async Task HandleUnknownCommand(Message message)
        {
            string response = "Извините, я не понимаю эту команду.";
            await _botClient.SendMessage(message.Chat.Id, response);
        }
        private async Task DeletePreviousMessages(long chatId)
        {
            var user = await _usersRepository.GetByTelegramId(chatId);
            var messageIds = user.MesssageIds;
            var messages = await _messagesRepository.GetByIds(messageIds);
            foreach (var m in messages)
            {
                await _botClient.DeleteMessage(chatId, (int)m.MessageId);
            }
            user.MesssageIds.Clear();
            await _usersRepository.Update(user.Id, user.TelegramId, user.UserName, user.MesssageIds);
        }
    }
}
