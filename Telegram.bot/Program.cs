using BOT;
using Telegram.Bot;
using Telegram.Bot.Types;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BOT
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            try
            {
                TelegramBotHelper hlp = new TelegramBotHelper(token: "6138096254:AAFdT10g5QBBsQRCMog4-Y8mG9t1dbvUOWg");
                hlp.GetUpdates();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}