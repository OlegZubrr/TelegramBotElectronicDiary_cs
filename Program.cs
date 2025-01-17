using System;
using System.Configuration;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
namespace TelegramBotEFCore
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //string botToken = ConfigurationManager.AppSettings[nameof(botToken)];
            //var client = new TelegramBotClient(botToken);
            //client.StartReceiving(OnUpdate,OnError);
            //Console.WriteLine("Bot Started");
            //Console.ReadLine();
            //Console.WriteLine("Bot Stoped");
        }

        private static async Task OnUpdate(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var mesage = update.Message;
            if (mesage == null) 
            {
                return;
            }
            await client.SendMessage(mesage.Chat.Id,mesage.Text);
            
        }

        private static async Task OnError(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
        }
    }
}
