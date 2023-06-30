using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOT
{
    internal class BotUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public long UserID { get; set; } 
        public int? RoleID { get; set; } 
        public long? ChatID { get; set; }
        public bool? notificationEn { get; set; }
        
    }
}
