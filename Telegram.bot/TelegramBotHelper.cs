using Polly;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BOT
{
    internal class TelegramBotHelper
    {

        private const string TEXT_1 = "Help me";
        private const string TEXT_2 = "Зарегистрироваться";
        private const string TEXT_3 = "Мои курсы тренировок";
        private string _token;
        private int count = 1;
        TelegramBotClient botClient;
        private string testListTrenName = ApplicationConnect.stringTrenName;
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
                        var updates = botClient.GetUpdatesAsync(offset,null,1).Result;
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
                }
            }
        }

        private async void processUpdate(Update? update)
        {
            #region Work in update.type

            switch (update.Type)
            {
                case Telegram.Bot.Types.Enums.UpdateType.Message:
                    var vars = ApplicationConnect.TreningCreatedActual;
                    var chatID = update.Message.Chat.Id;
                    var text = update.Message.Text;
                    var chatName = update.Message.Chat.FirstName;
                    var result = DataWorker.GetAllTreningUser(chatID);
                    
                    int count = 1;
                    var execises = ApplicationConnect.ExecisesCratedActual;
                    //switch (update.Message.ReplyToMessage.Text)
                    //{
                    //    case null:
                    //        Console.WriteLine("null");
                    //        break;
                    //    case "Название программы":
                    //        string msg = update.Message.Text;
                    //        if (DataWorker.TreningTrueOrFalse(chatID, msg) == true)
                    //        {
                    //            await botClient.SendTextMessageAsync(chatID,
                    //                "Такое название уже есть, придумай новое");
                    //            await botClient.SendTextMessageAsync(chatID,
                    //                "Название программы",
                    //            parseMode: default,
                    //            replyMarkup: new ForceReplyMarkup { Selective = true });
                    //            return;
                    //        }
                    //        else
                    //        {
                    //            vars.TreningName = msg;
                    //            execises.TreningName = msg;
                    //            await botClient.SendTextMessageAsync(chatID, "Записал Как :" + msg);
                    //            await botClient.SendTextMessageAsync(chatID, "Какие группы мышц",
                    //            parseMode: default,
                    //            replyMarkup: new ForceReplyMarkup { Selective = true });
                    //            Console.WriteLine(chatName + " на " + 2 + " этапе создания программы");
                    //            return;
                    //        }
                    //        break;
                    //}

                    switch (text)
                    {
                        case "/start":
                            ApplicationConnect application = new ApplicationConnect();
                            if (DataWorker.RegistrTrueOrFalse(chatID) == true)
                            {  await botClient.SendTextMessageAsync(chatID, "Пожалуй начнём",
                                replyMarkup: Buttons.GetButton());
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(chatID, "Пройди регистрацию",
                                replyMarkup: Buttons.GetRegistratoinButton());
                            }
                            break;
                        case "Создать программу":
                            var countTEST = DataWorker.CountTreninigName(chatID);
                            if (countTEST.HasRows == false)
                            { vars.NumberName = 1; }
                            else
                            {
                                while (countTEST.Read())
                                {
                                    vars.NumberName = countTEST.GetInt32(0);
                                    break;
                                }
                            }
                            count = 1;
                            await botClient.SendTextMessageAsync(chatID, "Название программы",
                            parseMode: default,
                            replyMarkup: new ForceReplyMarkup { Selective = true });
                            //Console.WriteLine(chatName + " на " + 1 + " этапе создания программы");
                            return;
                        case "Начать тренировку":
                            if (DataWorker.SearchTraining(chatID) == true) 
                            {
                                var resultExercises = DataWorker.OutputExecises(chatID);
                                if (resultExercises.HasRows == true)
                                {
                                    while (resultExercises.Read())
                                    {
                                        var nameTren = resultExercises.GetString(0);
                                        await botClient.SendTextMessageAsync(chatID, $"{count}. {nameTren}");
                                        count++;
                                    }
                                    await botClient.SendTextMessageAsync(chatID,
                                        "Напишите название программы"
                                        , replyMarkup: new ForceReplyMarkup { Selective = true });
                                }
                                else
                                {
                                    await botClient.SendTextMessageAsync(chatID, "У вас нет программ",
                                        replyMarkup: Buttons.GetButtonBack());
                                    Console.WriteLine("Записей нету");
                                }
                            }
                            else 
                            {
                                await botClient.SendTextMessageAsync(chatID,
                                    "У вас нету созданных программ тренировок");
                            }
                                break;
                        case "Help me":
                            await botClient.SendTextMessageAsync(chatID,
                                "Контента не подвезли, пока что");
                            break;
                        case "Зарегистрироваться":
                            DataWorker.Registaration(update, botClient);
                            await botClient.SendTextMessageAsync(chatID, "Пожалуй начнём",
                                    replyMarkup: Buttons.GetButton());
                            break;
                        case "Мои курсы тренировок":
                            if (DataWorker.RegistrTrueOrFalse(chatID) != true)
                            {
                                await botClient.SendTextMessageAsync(chatID,
                                    "Сначала пройдите регистрацию");
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(chatID,
                                "Ваши курсы",
                                replyMarkup: Buttons.GetCreatedTreningButton());
                            }
                            break;
                        case "Мои программы":
                            if (result.HasRows == true)
                            {
                                while (result.Read())
                                {
                                    var nameTren = result.GetString(0);
                                    var nameMuscl = result.GetString(1);
                                    await botClient.SendTextMessageAsync(chatID, $"{count}. {nameTren}\n {nameMuscl}",
                                        replyMarkup: Buttons.GetButtonBack());
                                    count++;
                                }
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(chatID, "У вас нет программ",
                                    replyMarkup:Buttons.GetButtonBack());
                                Console.WriteLine("Записей нету");
                            }
                            break;
                        case "Удалить программу":
                            await botClient.SendTextMessageAsync(chatID,
                                "Для выбора\nНапишите название программы\n/start чтобы вернуться назад",
                                replyMarkup: new ForceReplyMarkup { Selective = true });
                            if (result.HasRows == true)
                            {
                                while (result.Read())
                                {
                                    var nameTren = result.GetString(0);
                                    await botClient.SendTextMessageAsync(chatID, $"{count}. {nameTren}");
                                    count++;
                                }
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(chatID, "У вас нет программ",
                                    replyMarkup: Buttons.GetButtonBack());
                                Console.WriteLine("Записей нету");
                            }
                            break;
                        case "Выбрать тренировку":
                            break;
                        case "Назад":
                            goto case "/start";
                        case "Вернуться в главное меню":
                            goto case "/start";
                        case "Редактировать вес":
                            await botClient.SendTextMessageAsync(chatID,
                                "Выберите упражнение в котором необходимо исправить вес",
                                replyMarkup: Buttons.
                                GetInlineKeyboardButtons(chatID,update,botClient, testListTrenName));
                            break; 
                        default:
                            await botClient.SendTextMessageAsync(chatID, "/start для начала");
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
                    //var test = update.PollAnswer;//update.Poll.Options;
                    Console.WriteLine($"Прилетел Pool #Voter Count# меняет 0 на 1 на выбранном");
                    break;
                default:
                    Console.WriteLine(update.Type + " Not ipmlemented");
                    break;
            }
            #endregion
            #region Created Trening
            //if (update.CallbackQuery.Data != null)
            //{
            //    var TEST = update.CallbackQuery.Data;
            //    Console.WriteLine(TEST);
            //    return;
            //}
            if (update.Message.Text != null)
            {
                try
                {
                    var vars = ApplicationConnect.TreningCreatedActual;
                    var text = update.Message.Text;
                    var chatID = update.Message.Chat.Id;
                    var chatName = update.Message.Chat.FirstName;
                    int countExercises = vars.CountExercises;
                    var execises = ApplicationConnect.ExecisesCratedActual;
                    if (update.Message.ReplyToMessage != null &&
                        update.Message.ReplyToMessage.Text.Contains("Для выбора\nНапишите название программы\n" +
                        "/start чтобы вернуться назад"))
                    {
                        DataWorker.DeleteTrening(text, chatID);
                        await botClient.SendTextMessageAsync(chatID, "Программа с названием : " + text +
                            "\n Успешно удалена", replyMarkup: Buttons.GetButtonBack());
                        return;
                    }
                    
                    if (update.Message.ReplyToMessage != null &&
                        update.Message.ReplyToMessage.Text.Contains("Напишите название программы"))
                    {
                        testListTrenName= text;
                        var res = DataWorker.OutputAllExercises2(testListTrenName, chatID);
                        if (res.HasRows == true)
                        {
                            count = 1;
                            while (res.Read())
                            {
                                var nameTren = res.GetString(0);
                                var countRepear = res.GetInt32(2);
                                var weight = res.GetDecimal(3);
                                await botClient.SendTextMessageAsync(chatID, $"{count}. {nameTren}\n" +
                                $" Повторений : {countRepear} раз // вес : {weight}кг", 
                                replyMarkup: Buttons.GetButtonsRedactorWeight());
                                count++;
                            }
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(chatID, "Вы ввели неверное название"
                            , replyMarkup: Buttons.GetButtonBack());
                        }
                        return;
                    }
                    if (update.Message.Text == "Создать программу")
                    {
                        var countTEST = DataWorker.CountTreninigName(chatID);
                        if (countTEST.HasRows == false)
                        { vars.NumberName = 1; }
                        else
                        {
                            while (countTEST.Read())
                            {
                                vars.NumberName = countTEST.GetInt32(0);
                                break;
                            }
                        }
                        count = 1;
                        await botClient.SendTextMessageAsync(chatID, "Название программы",
                        parseMode: default,
                        replyMarkup: new ForceReplyMarkup { Selective = true });
                        Console.WriteLine(chatName + " на " + 1 + " этапе создания программы");
                        return;
                    }
                    if (update.Message.ReplyToMessage != null &&
                        update.Message.ReplyToMessage.Text.Contains("Название программы"))
                    {
                        string msg = update.Message.Text;
                        if (DataWorker.TreningTrueOrFalse(chatID, msg) == true)
                        {
                            await botClient.SendTextMessageAsync(chatID,
                                "Такое название уже есть, придумай новое");
                            await botClient.SendTextMessageAsync(chatID,
                                "Название программы",
                            parseMode: default,
                            replyMarkup: new ForceReplyMarkup { Selective = true });
                            return;
                        }
                        else
                        {
                            vars.TreningName = msg;
                            execises.TreningName = msg;
                            await botClient.SendTextMessageAsync(chatID, "Записал Как :" + msg);
                            await botClient.SendTextMessageAsync(chatID, "Какие группы мышц",
                            parseMode: default,
                            replyMarkup: new ForceReplyMarkup { Selective = true });
                            Console.WriteLine(chatName + " на " + 2 + " этапе создания программы");
                            return;
                        }
                    }
                    if (update.Message.ReplyToMessage != null &&
                        update.Message.ReplyToMessage.Text.Contains("Какие группы мышц"))
                    {
                        string msg = update.Message.Text;
                        vars.GroupMuscle = msg;
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
                            vars.CountExercises = Convert.ToInt32(msg);
                            countExercises = vars.CountExercises;
                            await botClient.SendTextMessageAsync(chatID, "Записал Как :" + msg,
                                replyMarkup: Buttons.GetButton());
                            await botClient.SendTextMessageAsync(chatID, $"Упражнение {count} из {countExercises}");
                            await botClient.SendTextMessageAsync(chatID, $"Название упражнения",
                                    parseMode: default,
                                    replyMarkup: new ForceReplyMarkup { Selective = true });
                            return;
                        }
                        catch
                        {
                            await botClient.SendTextMessageAsync(chatID, "Необходимо вводить цифры, начни сначала");
                            await botClient.SendTextMessageAsync(chatID, "Название программы",
                            parseMode: default,
                            replyMarkup: new ForceReplyMarkup { Selective = true });
                        }
                    }
                    if (update.Message.ReplyToMessage != null &&
                        update.Message.ReplyToMessage.Text.Contains($"Название упражнения"))
                    {
                        string msg = update.Message.Text;
                        execises.ExercisesName = msg;
                        await botClient.SendTextMessageAsync(chatID, "Какой вес?",
                            replyMarkup: new ForceReplyMarkup { Selective = true });
                        return;
                    }
                    if (update.Message.ReplyToMessage != null &&
                        update.Message.ReplyToMessage.Text.Contains("Какой вес?"))
                    {
                        try
                        {
                            decimal msg = Convert.ToDecimal(update.Message.Text);
                            execises.Weight = msg;
                            await botClient.SendTextMessageAsync(chatID, "Сколько повторений?",
                            replyMarkup: new ForceReplyMarkup { Selective = true });
                        }
                        catch (Exception ex)
                        {
                            await botClient.SendTextMessageAsync(chatID,
                            "Вводите вес целым числом '100'" +
                            ", либом через запятую '100,5'");
                            await botClient.SendTextMessageAsync(chatID, "Какой вес?",
                            replyMarkup: new ForceReplyMarkup { Selective = true });
                            Console.WriteLine(ex.Message);
                        }
                    }
                    if (update.Message.ReplyToMessage != null &&
                        update.Message.ReplyToMessage.Text.Contains("Сколько повторений?"))
                    {
                        string msg = update.Message.Text;
                        execises.CountRepertion = Convert.ToInt32(msg);
                        execises.NumberName = count;
                        int countExecises = vars.CountExercises;
                        DataWorker.CreateExecisesTrening(update, execises);
                        if (countExecises == count)
                        {
                            DataWorker.GetDateTreningProgram(vars, update, botClient);
                            count = 1;
                            return;
                        }
                        while (countExecises >= count)
                        {
                            if (countExecises > count)
                            {
                                count++;
                                await botClient.SendTextMessageAsync(chatID, $"Название упражнения",
                                parseMode: default,
                                replyMarkup: new ForceReplyMarkup { Selective = true });
                                await botClient.SendTextMessageAsync(chatID, $"Упражнение {count} из {countExercises}");
                                break;
                            }
                            else { break; }
                        }
                        return;
                    }
                }
                catch
                {
                    var chatID = update.Message.Chat.Id;
                    await botClient.SendTextMessageAsync(chatID, "Ты все сломал, давай сначала /start");
                }
            }
            else { Console.WriteLine("Null"); }
            #endregion
        }
    }
}