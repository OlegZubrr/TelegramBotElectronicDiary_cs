using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramBotEFCore.Handlers.Interfaces
{
    public interface ICallbackHandler
    {
        Task HandleCallbackAsync(CallbackQuery callbackQuery);
    }
}
