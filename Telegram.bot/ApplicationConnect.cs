using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BOT
{
    internal class ApplicationConnect : DbContext
    {
        public static TreningProgram TreningCreatedActual = new TreningProgram();
        public static TreningExercises ExecisesCratedActual = new TreningExercises();
        public static string stringTrenName { get; set; }
        public DbSet<BotUser> Users { get; set; } 
        public DbSet<TreningProgram> Treninng { get; set; }
        public DbSet<TreningExercises> Exercises { get; set; }

        public ApplicationConnect()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TelegaBot;Trusted_Connection=True;");

        }

        //public static void SaveDate(ApplicationConnect data) 
        //{
        //    string serializedData = JsonConvert.SerializeObject(data);
        //    if(serializedData != null)
        //    {
        //        string[] str = new string[1];
        //        str[0] = serializedData;
        //        if (!System.IO.File.Exists("data.txt"))
        //            File.Create("data.txt");
        //        File.WriteAllLines("data.txt", str);
        //    }
        //    else Console.WriteLine(" ");
        //}
        //public static ApplicationConnect InitData() 
        //{
        //    if (File.Exists("data.txt"))
        //    {
        //        string data = File.ReadAllText("data.txt");
        //        ApplicationConnect parcedData = JsonConvert.DeserializeObject<ApplicationConnect>(data);
        //        return parcedData;
        //    }
        //    else return null;
        //}
    }
}
