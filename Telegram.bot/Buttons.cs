using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.Mime.MediaTypeNames;

namespace BOT
{
    internal class Buttons
    {
        public static IReplyMarkup? GetButtunsTraining()
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "Выбрать тренировку" },
                new KeyboardButton[] { "Редактировать вес" },
                new KeyboardButton[] { "Вернуться в главное меню" }
            })
            {
                ResizeKeyboard = true
            };
            return replyKeyboardMarkup;
        }
        private const string TEXT_1 = "Help me";
        private const string TEXT_2 = "Зарегистрироваться";
        private const string TEXT_3 = "";
        public static IReplyMarkup? GetButton()
        {
            ReplyKeyboardMarkup replyKeyboardMarkup =
            new(new[]
            {
                new KeyboardButton[] { "Help me"},
                new KeyboardButton[] { "Мои курсы тренировок"},
                new KeyboardButton[] {"Начать тренировку"}
            })
            {
                ResizeKeyboard = true
            };
            return replyKeyboardMarkup;
        }
        public static IReplyMarkup? GetRegistratoinButton()
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] {"Help me"},
                new KeyboardButton[] { "Зарегистрироваться" }
            });
            return replyKeyboardMarkup;
        }
        public static IReplyMarkup? GetCreatedTreningButton()
        {
            ReplyKeyboardMarkup replyKeyboardMarkup =
            new(new[]
            {
                new KeyboardButton[] { "Мои программы" },
                new KeyboardButton[] { "Удалить программу", "Создать программу"}  ,
                new KeyboardButton[] {"Вернуться в главное меню"}
            })
            {
                ResizeKeyboard = true
            };
            return replyKeyboardMarkup;
        }
        public static IReplyMarkup? GetButtonBack()
        {
            ReplyKeyboardMarkup replyKeyboardMarkup =
                new(new[]
                {
                    new KeyboardButton[] {"Назад"}
                })
                { ResizeKeyboard = true };
            return replyKeyboardMarkup;
        }
    }
}
