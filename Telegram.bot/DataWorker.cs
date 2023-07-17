using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bots;
using Telegram.Bots.Http;

namespace BOT
{
    internal class DataWorker
    {
        internal static SqlDataReader GetAllTreningUser(long chatID)
        {
            SqlConnection conn = new SqlConnection("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=TelegaBot;Trusted_Connection=True;");
            conn.Open();
            SqlDataReader reader = null;
            string getAllTren = $"SELECT TreningName,GroupMuscle FROM Treninng WHERE UserID={chatID}";
            SqlCommand command = new SqlCommand(getAllTren,conn);
            reader = command.ExecuteReader();
            return reader;
        }
        public static void DeleteTrening(string treningName,long chatId)
        {
            SqlConnection conn = new SqlConnection("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=TelegaBot;Trusted_Connection=True;");
            try
            {
                conn.Open();
                string deleteName = $"DELETE Treninng WHERE TreningName=N'{treningName}' AND UserID={chatId}";
                SqlCommand command = new SqlCommand(deleteName, conn);
                command.ExecuteNonQuery();
                string deleteExecises = $"DELETE Exercises WHERE TreningName=N'{treningName}' AND UserID={chatId}";
                SqlCommand command2 = new SqlCommand(deleteExecises, conn);
                command2.ExecuteNonQuery();
            }
            catch (Exception ex) {Console.WriteLine(ex.Message);}
            finally { conn.Close(); }
        }
        public static async void CreateExecisesTrening(Update update, TreningExercises trening)
        {
            ApplicationConnect connect = new ApplicationConnect();
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
                if (ChatId == TestId || TestId != null)
                {
                    conn.Open();
                    string AddExecises = "INSERT INTO Exercises (UserID,NumberName,TreningName,ExercisesName,CountRepertion,Weight)" +
                        " VALUES (@UserID,@NumberName,@TreningName,@ExercisesName, @CountRepertion , @Weight)";
                    SqlCommand command = new SqlCommand(AddExecises, conn);
                    command.Parameters.AddWithValue("@UserID", ChatId );
                    command.Parameters.AddWithValue("@NumberName", trening.NumberName);
                    command.Parameters.AddWithValue("@TreningName", trening.TreningName );
                    command.Parameters.AddWithValue("@ExercisesName",trening.ExercisesName );
                    command.Parameters.AddWithValue("@CountRepertion",trening.CountRepertion );
                    command.Parameters.AddWithValue("@Weight",trening.Weight );
                    await command.ExecuteNonQueryAsync();
                }
                else
                {
                    Console.WriteLine($"Аккаунт для {ChatName} еще не зарегистрирован"); ;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { conn.Close(); }
        }
        public static bool RegistrTrueOrFalse(long chatId)
        {
            SqlConnection conn = new SqlConnection("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=TelegaBot;Trusted_Connection=True;");
            conn.Open();
            string search = $"SELECT UserID FROM users WHERE UserID={chatId}";
            SqlCommand command = new SqlCommand(search, conn);
            var result = command.ExecuteReader();
            var output = result.HasRows;
            return output;
        }
        public static bool TreningTrueOrFalse(long chatId, string treningName)
        {
            SqlDataReader reader = null;
            SqlConnection conn = new SqlConnection("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=TelegaBot;Trusted_Connection=True;");
            conn.Open();
            string search = $"SELECT UserID,TreningName FROM Treninng WHERE UserID={chatId}";
            SqlCommand command = new SqlCommand(search, conn);
            reader = command.ExecuteReader();
            bool output = false;
            while (reader.Read())
            {
                var res = reader.GetString(1);
                if (treningName == res)
                {
                    output = true;
                    break;
                }
            }
            return output;
        }
        public static async void GetDateTreningProgram(TreningProgram vars, Update update, TelegramBotClient botClient)
        {
            SqlConnection conn = new SqlConnection("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=TelegaBot;Trusted_Connection=True;");
            conn.Open();
            try
            {
                string textInputList = $"Вы успешно создали программу \nНазвание : {vars.TreningName} \nГруппа мышц : {vars.GroupMuscle}" +
                    $"\nКолличество упражнений : {vars.CountExercises}";
                Console.WriteLine($"Создал программу {update.Message.Chat.FirstName}");
                string addTreningUsers = "INSERT INTO Treninng (Name,NumberName,UserID,UserName,TreningName,GroupMuscle,CountExercises,CountRepertitions)" +
                    " VALUES (@Name,@NumberName, @UserID , @UserName,@TreningName,@GroupMuscle,@CountExercises,@CountRepertitions)";
                SqlCommand comm = new SqlCommand(addTreningUsers, conn);
                comm.Parameters.AddWithValue("@Name", update.Message.Chat.FirstName);
                comm.Parameters.AddWithValue("@NumberName", vars.NumberName);
                comm.Parameters.AddWithValue("@UserName", update.Message.Chat.Username);
                comm.Parameters.AddWithValue("@UserID", update.Message.Chat.Id);
                comm.Parameters.AddWithValue("@TreningName", vars.TreningName);
                comm.Parameters.AddWithValue("@GroupMuscle", vars.GroupMuscle);
                comm.Parameters.AddWithValue("@CountExercises", vars.CountExercises);
                comm.Parameters.AddWithValue("@CountRepertitions", vars.CountRepertitions);
                await comm.ExecuteNonQueryAsync();
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, textInputList, replyMarkup: Buttons.GetButtonBack());
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally { conn.Close(); }
        }
        public static async void Registaration(Update update, TelegramBotClient botClient)
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
        public static bool SearchTraining(long chatID )
        {
            SqlConnection conn = new SqlConnection("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=TelegaBot;Trusted_Connection=True;");
            conn.Open();
            SqlDataReader reader = null;
            string searchTreningUser = $"SELECT TreningName FROM Treninng WHERE UserID={chatID}";
            SqlCommand command = new SqlCommand(searchTreningUser, conn);
            reader = command.ExecuteReader();
            var result = reader.HasRows;
            conn.Close();
            return result;
        }
        public static SqlDataReader OutputAllExercises(string trenName, long chatID)
        {
            SqlConnection conn = new SqlConnection("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=TelegaBot;Trusted_Connection=True;");
            conn.Open();
            SqlDataReader reader = null;
            string searchExercises = $"SELECT ExercisesName,CountRepertion,Weight FROM Exercises WHERE TreningName=N'{trenName}' and UserID={chatID}";
            SqlCommand command = new SqlCommand(searchExercises, conn);
            reader = command.ExecuteReader();
            return reader;
        }
        public static SqlDataReader OutputAllExercises2(string trenName, long chatID)
        {
            SqlConnection conn = new SqlConnection("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=TelegaBot;Trusted_Connection=True;");
            conn.Open();
            SqlDataReader reader = null;
            string searchExercises = $"SELECT ExercisesName,NumberName,CountRepertion,Weight FROM Exercises WHERE TreningName=N'{trenName}' and UserID={chatID}";
            SqlCommand command = new SqlCommand(searchExercises, conn);
            reader = command.ExecuteReader();
            return reader;
        }
        public static SqlDataReader OutputExecises(long chatID)
        {
            SqlConnection conn = new SqlConnection("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=TelegaBot;Trusted_Connection=True;");
            conn.Open();
            SqlDataReader reader = null;
            string searchExe = $"SELECT DISTINCT  TreningName  FROM Exercises WHERE UserID={chatID}";
            SqlCommand command = new SqlCommand(searchExe, conn);
            reader = command.ExecuteReader();
            return reader;
        }
        public static SqlDataReader CountTreninigName(long chatID)
        {
            SqlConnection conn = new SqlConnection("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=TelegaBot;Trusted_Connection=True;");
            conn.Open();
            SqlDataReader reader = null;
            string countTren = $"SELECT COUNT(TreningName) FROM Treninng WHERE UserID={chatID}";
            SqlCommand command = new SqlCommand(countTren, conn);
            reader = command.ExecuteReader();
            return reader;
        }
        public static SqlDataReader CountExecisesName(string trenName,long chatID)
        {
            SqlConnection conn = new SqlConnection("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=TelegaBot;Trusted_Connection=True;");
            conn.Open();
            SqlDataReader reader = null;
            string countTren = $"SELECT COUNT(ExercisesName) FROM Exercises WHERE TreningName=N'{trenName}' AND UserID={chatID}";
            SqlCommand command = new SqlCommand(countTren, conn);
            reader = command.ExecuteReader();
            return reader;
        }
        public static void UpdateWeightInExercises(long chatID,int numberName, string trenName, decimal weight)
        {
            SqlConnection conn = new SqlConnection("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=TelegaBot;Trusted_Connection=True;");
            conn.Open();
            string updateWeight =$"UPDATE Exercises SET weight ={weight} WHERE UserID={chatID} " +
                $"AND NumberName={numberName}  AND TreningName=N'{trenName}'";
            SqlCommand command = new SqlCommand(updateWeight, conn);
            command.ExecuteNonQuery();
        }
        public static string[] GetStringsExecises(string trenName,long chatID)
        {
            int count = 0;
            var res = DataWorker.OutputAllExercises2(trenName,chatID);
            var countExecises = CountExecisesName(trenName, chatID);
            while (countExecises.Read()) { count = countExecises.GetInt32(0); }
            int i = 0;
            string[] strings = new string[count] ;
            if (res.HasRows == true)
            {
                while (res.Read())
                {
                    strings[i] = res.GetString(0);
                    i++;
                }
            }
            else { Console.WriteLine("False"); }
            return strings;
        }
    }

}

