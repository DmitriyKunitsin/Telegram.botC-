﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOT
{
    internal class TreningExercises
    {
        public int Id { get; set; }
        private long userID { get; set; }
        private string treningName { get; set; }
        private string exercisesName { get; set; }
        private int countRepertion { get; set; }
        private int weight { get; set; }
        public long UserID { get { return userID; } set { userID = value; } }
        public string TreningName { get { return treningName; } set { treningName = value; } }
        public string ExercisesName { get { return exercisesName; } set {  exercisesName = value; } }
        public int CountRepertion { get { return countRepertion; } set { countRepertion = value; } }
        public int Weight { get { return weight; } set { weight = value; } }
        public TreningExercises()
        {
            this.UserID= 0;
            this.TreningName = string.Empty;
            this.ExercisesName = string.Empty;
            this.CountRepertion = 0;
            this.Weight = 0;
        }
    }
}
