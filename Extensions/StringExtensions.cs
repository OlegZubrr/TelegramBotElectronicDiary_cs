using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TelegramBotEFCore.Extensions
{
    public static class StringExtensions
    {
        public static bool IsValidName(this string name) 
        {
            if (string.IsNullOrWhiteSpace(name)) 
                return false;
            if(name.Length > 20)
                return false;

            return Regex.IsMatch(name, @"^[a-zA-Zа-яА-ЯёЁ]+$");
        }
    }
}
