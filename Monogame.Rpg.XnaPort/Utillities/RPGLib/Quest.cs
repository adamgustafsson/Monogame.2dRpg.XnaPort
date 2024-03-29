﻿using System.Collections.Generic;
namespace LudosProduction.RpgLib
{
    public class Quest
    {
        public int Id;
        public int QuestPickup;
        public int QuestTurnIn;
        public int Status;
        public string PreMessage;
        public string MidMessage;
        public string EndMessage;
        public List<Objective> Objectives;
    }
    public class Objective
    {
        public int Obj;
        public int ObjType;
        public int Amount;
        public string Name;
    }
}
