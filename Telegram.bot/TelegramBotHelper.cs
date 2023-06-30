using BOT;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bots.Http;

using static System.Net.Mime.MediaTypeNames;

namespace BOT
{
    internal class TelegramBotHelper
    {

        private const string TEXT_1 = "Help me";
        private const string TEXT_2 = "Зарегистрироваться";
        private const string TEXT_3 = "Мои курсы тренировок";
        private const string TEXT_4 = "Опрос про Киселя";
        private string _token;
        TelegramBotClient botClient;
        public TelegramBotHelper(string token)
        {
            this._token = token;

        }

        internal void GetUpdates()
        {

            botClient = new TelegramBotClient(_token);
            var my = botClient.GetMeAsync().Result;
            if (my != null && !string.IsNullOrEmpty(my.Username))
            {
                int offset = 0;
                while (true)
                {
                    try
                    {
                        var updates = botClient.GetUpdatesAsync(offset).Result;
                        if (updates != null && updates.Count() > 0)
                        {
                            foreach (var update in updates)
                            {
                                processUpdate(update);
                                offset = update.Id + 1;
                            }
                        }
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }
                    Thread.Sleep(1000);
                }
            }
        }

        private async void processUpdate(Update update)
        {
            #region Created Trening
            try
            {

                List<TreningProgram> vars = new List<TreningProgram>();
                var text = update.Message.Text;
                var chatID = update.Message.Chat.Id;
                var chatName = update.Message.Chat.FirstName;

                if (text == "Создать программу")
                {
                    await botClient.SendTextMessageAsync(chatID, "Название программы",
                    parseMode: default,
                    replyMarkup: new ForceReplyMarkup { Selective = true });
                    Console.WriteLine(chatName +" на "+ 1+ " этапе создания программы");
                    return;
                }
                if (update.Message.ReplyToMessage != null &&
                    update.Message.ReplyToMessage.Text.Contains("Название программы"))
                {
                    string msg = update.Message.Text;
                    vars.Add(new TreningProgram { TreningName = msg });
                    await botClient.SendTextMessageAsync(chatID, "Записал Как :" + msg);
                    await botClient.SendTextMessageAsync(chatID, "Какие группы мышц",
                    parseMode: default,
                    replyMarkup: new ForceReplyMarkup { Selective = true });
                    Console.WriteLine(chatName + " на " + 2 + " этапе создания программы");
                    return;
                }
                if (update.Message.ReplyToMessage != null &&
                    update.Message.ReplyToMessage.Text.Contains("Какие группы мышц"))
                {
                    string msg = update.Message.Text;
                    vars.Add(new TreningProgram { GroupMuscle = msg });
                    await botClient.SendTextMessageAsync(chatID, "Записал Как :" + msg);
                    await botClient.SendTextMessageAsync(chatID, "Сколько упражнений будет в твоей программе?",
                    parseMode: default,
                    replyMarkup: new ForceReplyMarkup { Selective = true });
                    Console.WriteLine(chatName + " на " + 3 + " этапе создания программы");
                    return;
                }
                if (update.Message.ReplyToMessage != null &&
                    update.Message.ReplyToMessage.Text.Contains("Сколько упражнений будет в твоей программе?"))
                {
                    string msg = update.Message.Text;
                    try
                    {
                        vars.Add(new TreningProgram { CountExercises = Convert.ToInt32(msg) });
                        await botClient.SendTextMessageAsync(chatID, "Записал Как :" + msg);
                        await botClient.SendTextMessageAsync(chatID, "Сколько повторений?",
                        parseMode: default,
                        replyMarkup: new ForceReplyMarkup { Selective = true });
                        Console.WriteLine(chatName + " на " + 4 + " этапе создания программы");
                        return;
                    }
                    catch
                    {
                        botClient.SendTextMessageAsync(chatID, "Необходимо вводить цифры, начни сначала");
                        await botClient.SendTextMessageAsync(chatID, "Название программы",
                        parseMode: default,
                        replyMarkup: new ForceReplyMarkup { Selective = true });
                    }
                }
                if (update.Message.ReplyToMessage != null &&
                    update.Message.ReplyToMessage.Text.Contains("Сколько повторений?"))
                {
                    string msg = update.Message.Text;
                    vars.Add(new TreningProgram { CountRepertitions = Convert.ToInt32(msg) });
                    await botClient.SendTextMessageAsync(chatID, "Записал Как :" + msg);
                    await botClient.SendTextMessageAsync(chatID, "Готово");
                    Console.WriteLine(chatName + " на " + 5 + " этапе создания программы");
                    Console.WriteLine(vars);
                    return;
                }
            }
            catch 
            {
                var chatID = update.Message.Chat.Id;
                botClient.SendTextMessageAsync(chatID, "Ты все сломал, давай сначала");
            }

            #endregion

            #region Work in update.type


            switch (update.Type)
            {
                case Telegram.Bot.Types.Enums.UpdateType.Message:
                    var chatID = update.Message.Chat.Id;
                    var text = update.Message.Text;
                    var ChatName = update.Message.Chat.FirstName;
                    switch (text)
                    {
                        case "/start":
                            await botClient.SendTextMessageAsync(chatID, "Пожалуй начнём",
                                replyMarkup: GetButton());
                            break;
                        case "Help me":
                            botClient.SendTextMessageAsync(chatID, 
                                "Контента не подвезли, пока что");
                            break;
                        case "Зарегистрироваться":
                            Registaration(update);
                            break;
                        case "Мои курсы тренировок":
                            if (ResitrTrueOrFalse(chatID) != true)
                            {
                                botClient.SendTextMessageAsync(chatID, 
                                    "Сначала пройдите регистрацию");
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(chatID,
                                "Ваши курсы",
                                replyMarkup: GetCreatedTreningButton());
                            }
                            break;
                        case "Опрос про Киселя":
                            await botClient.SendPollAsync(chatID,
                                question: "Вы считаете Алексея Киселева пидором?",
                                options: new[]
                            {
                                        "Да , конечно, пидорас конченый!",
                                        "Нет, не считаю, но пидор тот еще"
                            });
                            break;

                        default:
                            botClient.SendTextMessageAsync(chatID, "/start для начала");
                            break;
                    }
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.CallbackQuery:
                    switch (update.CallbackQuery.Data)
                    {
                        case "11":
                            await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, 
                                "Ты выбрал 1.1");
                            break;
                        case "12":
                            await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, 
                                "Ты выбрал 1.2");
                            break;
                        case "21":
                            await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id,
                                "Ты выбрал 2.1");
                            break;
                        case "22":
                            await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id,
                                "Ты выбрал 2.2");
                            break;
                        case "31":
                            await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id,
                                "Ты выбрал 3.1");
                            break;
                        case "32":
                            await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id,
                                "Ты выбрал 3.2");
                            break;
                    }
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.Poll:
                    var test = update.Poll.Options;
                    Console.WriteLine($"Прилетел Pool #Voter Count# меняет 0 на 1 на выбранном");
                    break;
                default:
                    Console.WriteLine(update.Type + " Not ipmlemented");
                    break;
            }
        }
        #endregion
     
        #region Methods buttons
        public IReplyMarkup? GetInlinerDialog()
        {
            InlineKeyboardMarkup inlineDialog = new(new[]
            {new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "1.1 ", callbackData:"11"),
                InlineKeyboardButton.WithCallbackData(text: "1.2 ", callbackData:"12"),

            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "2.1", callbackData: "21"),
                InlineKeyboardButton.WithCallbackData(text: "2.2", callbackData: "22")
            },new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "3.1", callbackData: "31"),
                InlineKeyboardButton.WithCallbackData(text: "3.2", callbackData: "32")
            },
            });
            return inlineDialog;
        }
        public IReplyMarkup? GetButton()
        {
            ReplyKeyboardMarkup replyKeyboardMarkup =
            new(new[]
            {
                new KeyboardButton[] { TEXT_1,TEXT_2 },
                             new KeyboardButton[] { TEXT_3},
                             new KeyboardButton[] { TEXT_4}
            })
            {
                ResizeKeyboard = true
            };
            return replyKeyboardMarkup;
        }public IReplyMarkup? GetCreatedTreningButton()
        {
            ReplyKeyboardMarkup replyKeyboardMarkup =
            new(new[]
            {
                new KeyboardButton[] { "Моя программа" },
                new KeyboardButton[] { "Удалить мою программу", "Создать программу"}                             
            })
            {
                ResizeKeyboard = true
            };
            return replyKeyboardMarkup;
        }
        public IReplyMarkup? GetInlinerButton()
        {
            InlineKeyboardMarkup inlineKeyboard = new(new[]
            {
                InlineKeyboardButton.WithUrl(
                    text:"site url buttons",
                    url:"https://telegrambots.github.io/book/2/reply-markup.html")
            });
            return inlineKeyboard;
        }
        #endregion

        #region work in data methods
        private async void Registaration(Update update)
        {
            var ChatId = update.Message.Chat.Id;
            var ChatName = update.Message.Chat.FirstName;
            SqlConnection conn = new SqlConnection("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=TelegaBot;Trusted_Connection=True;");
            conn.Open();
            SqlDataReader reader = null;
            try
            {
                string checkIdInDate = $"SELECT UserID FROM Users WHERE UserID={ChatId}";
                SqlCommand command1 = new SqlCommand(checkIdInDate, conn);
                reader = command1.ExecuteReader();
                var TestId = 0f;
                while (reader.Read())
                {
                    TestId = reader.GetInt64(0);

                    break;
                }
                conn.Close();
                if (ChatId != TestId || TestId == null)
                {
                    conn.Open();
                    Console.WriteLine($"Аккунт создается для {ChatName}");
                    string addUser = "INSERT INTO Users (Name,UserID,UserName) VALUES (@Name, @UserID , @UserName)";
                    SqlCommand command = new SqlCommand(addUser, conn);
                    command.Parameters.AddWithValue("@Name", ChatName);
                    command.Parameters.AddWithValue("@UserName", update.Message.Chat.Username);
                    command.Parameters.AddWithValue("@UserID", ChatId);
                    await command.ExecuteNonQueryAsync();
                    await botClient.SendTextMessageAsync(ChatId, $"Вы успешно зарегистрировались {ChatName}, поздравляю!");
                }
                else
                {
                    Console.WriteLine($"Аккаунт для {ChatName} уже зарегистрирован"); ;
                    await botClient.SendTextMessageAsync(ChatId, "Вы уже зарегестрированы");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { conn.Close(); }
        }
        
        private bool  ResitrTrueOrFalse(long chatId)
        {

            SqlConnection conn = new SqlConnection("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=TelegaBot;Trusted_Connection=True;");
            conn.Open();
            string search = $"SELECT UserID FROM users WHERE UserID={chatId}";
            SqlCommand command = new SqlCommand(search, conn);
            var result = command.ExecuteReader();
            var output = result.HasRows;
            return output;
        }
        private async void CreatTrening() 
        {
            
        }
        #endregion
    }
}