﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotEFCore.Handlers.Interfaces;
using TelegramBotEFCore.Models;

namespace TelegramBotEFCore.Handlers
{
    public class StartCommandHandler : IMessageHandler
    {
        private readonly ITelegramBotClient _botClient;

        public StartCommandHandler(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }
        public async Task HandleMessageAsync(Message message, Dictionary<long, UserState> userStates)
        {
            string responce = "Привет";
            await _botClient.SendMessage(message.Chat.Id,responce);
        }
    }
}
