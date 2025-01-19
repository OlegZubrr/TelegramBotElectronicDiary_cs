﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotEFCore.Models;

namespace TelegramBotEFCore.Handlers.Interfaces
{
    public interface IStateHandler
    {
        Task HandleAsync(Message message, Dictionary<long, UserState> userStates);
    }
}
