using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOT
{
    internal class TreningProgram
    {
        public int Id { get; set; }
        private string name { get; set; }
        private string userName { get; set; }
        private long userID { get; set; }
        private string treningName { get; set; }
        private string groupMuscle { get; set; }
        private int countExercises { get; set; }
        private int countRepertitions { get; set; }

        public  string Name 
        { 
            get { return name; }
            set { name = value; }
        }
        public string UserName 
        {
            get { return userName; }
            set { userName = value; }
        }
        public long UserID
        {
            get { return userID; }
            set { userID = value;  }
        }
        public string? TreningName 
        { 
            get { return treningName; }
            set { treningName = value; }
        }
        public string? GroupMuscle
        {
            get { return groupMuscle; }
            set { groupMuscle = value; }
        }
        public int CountExercises 
        {
            get { return countExercises; }
            set { countExercises= value; } 
        }
        public int CountRepertitions
        {
            get { return countRepertitions; }
            set { countRepertitions= value; }
        }
        public TreningProgram()
        {
            this.Name = string.Empty;
            this.UserName = string.Empty;
            this.UserID = 0;
            this.TreningName = string.Empty;
            this.GroupMuscle = string.Empty;
            this.CountExercises = 0;
            this.CountRepertitions = 0;
        }
    }
}
