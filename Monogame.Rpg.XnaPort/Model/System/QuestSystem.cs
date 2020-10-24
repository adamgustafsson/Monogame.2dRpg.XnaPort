using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using LudosProduction.RpgLib;

namespace Model
{
    class QuestSystem
    {
        //QuestStatus
        public const int PRE = 2;
        public const int MID = 3;
        public const int END = 4;

        //ObjektIndex
        public const int ENEMY = 0;
        public const int FRIEND = 1;
        public const int ARMOR = 2;
        public const int QUEST_ITEM = 3;

        //ObjektIndex
        public const int WARRIOR = 0;


        private bool m_allQuestsCompleted;

        private bool m_isWatchingQuestLog = false;
        private int m_currentQuestIndex = 0;
        private List<Quest> m_questList;
        private List<Objective> m_objectiveList;

        private string m_currentMessage;
        private int m_activeNpc;
        private int m_recItemAmount;

        #region Get/Set
        public bool IsWatchingQuestLog
        {
            get { return m_isWatchingQuestLog; }
            set { m_isWatchingQuestLog = value; }
        }
        public int CurrentQuestIndex
        {
            get { return m_currentQuestIndex; }
            set { m_currentQuestIndex = value; }
        }

        public int ActiveNpc
        {
            get { return m_activeNpc; }
            set { m_activeNpc = value; }
        }
        public string CurrentMessage
        {
            get { return m_currentMessage; }
            set { m_currentMessage = value; }
        }
        public Quest CurrentQuest
        {
            get { return m_questList[m_currentQuestIndex]; }
            set { m_questList[m_currentQuestIndex] = value; }
        }
        public List<Objective> ObjectiveList
        {
            get { return m_objectiveList; }
            set { m_objectiveList = value; }
        }
        public int QuestStatus
        {
            get { return CurrentQuest.Status; }
            set { CurrentQuest.Status = value; }
        }
        public List<Quest> QuestList
        {
            get { return m_questList; }
            set { m_questList = value; }
        }

        public bool AllQuestsCompleted
        {
            get { return m_allQuestsCompleted; }
            set { m_allQuestsCompleted = value; }
        }
        #endregion

        public QuestSystem(ContentManager a_content)
        {
            m_questList = RpgXmlSerializer.LoadQuestsFromXml("Content/XML/quest.xml"); //a_content.Load<List<Reader.Quest>>("Content/XML/quest.xml");
            m_objectiveList = new List<Objective>();
            LoadObjectives();
            QuestStatus = PRE;
        }

        internal void UpdateActiveQuest(List<Enemy> a_enemyList, List<Friend> a_friendList, List<Item> a_inventoryList, Level a_level)
        {
            UpdateStatus();

            if (!IsQuestComplete())
            {
                if (CurrentQuest.Status != PRE)
                {
                    for (int i = 0; i < m_objectiveList.Count; i++)
                    {
                        if (m_objectiveList[i].Obj == ENEMY && m_objectiveList[i].Amount < CurrentQuest.Objectives[i].Amount)
                            UpdateEnemyStatus(a_enemyList, m_objectiveList[i]);

                        if (m_objectiveList[i].Obj == FRIEND)
                            UpdateNPCStatus(a_friendList, m_objectiveList[i]);

                        if (m_objectiveList[i].Obj == ARMOR && m_objectiveList[i].Amount < CurrentQuest.Objectives[i].Amount)
                            UpdateItemStatus(a_inventoryList, m_objectiveList[i], Model.GameModel.ARMOR);

                        if (m_objectiveList[i].Obj == QUEST_ITEM && m_objectiveList[i].Amount < CurrentQuest.Objectives[i].Amount)
                            UpdateItemStatus(a_inventoryList, m_objectiveList[i], Model.GameModel.QUEST_ITEM);
                    }
                }
            }
            else
            {
                QuestStatus = END;
            }
            RemoveProgressObstacle(a_level);
        }
        
        private void RemoveProgressObstacle(Level a_level)
        {
            if (m_currentQuestIndex == 2)
            {
                for (int i = 0; i < a_level.CollisionLayer.MapObjects.Length; i++)
                {
                    if (a_level.CollisionLayer.MapObjects[i].Name == "Gate1")
                    {
                        a_level.CollisionLayer.MapObjects[i].Name = "Open";
                    }
                }
            }
        }

        private void UpdateEnemyStatus(List<Enemy> a_enemyList, Objective a_objective)
        {
            foreach (Enemy enemy in a_enemyList)
            {
                if (enemy.CanAddToQuest && enemy.Type == a_objective.ObjType)
                {
                    a_objective.Amount++;
                    enemy.CanAddToQuest = false;
                }
            }
        }

        private void UpdateNPCStatus(List<Friend> a_friendList, Objective a_objective)
        { 
            //ObjType motsvarar det NPC-Id som skall pratas med
            Friend questNPC = a_friendList.Find(Friend => Friend.UnitId == a_objective.ObjType && Friend.CanAddToQuest);

            if (questNPC != null)
            {
                a_objective.Amount = 1;
                QuestStatus = END;
            }
            else
            {
                a_objective.Amount = 0;
                QuestStatus = MID;
            }
        }

        private void UpdateItemStatus(List<Item> a_inventoryList, Objective a_objective,  Type a_itemObjType)
        {
            a_objective.Amount = a_inventoryList.Count(Item => Item.GetType() == a_itemObjType && Item.Type == a_objective.ObjType);
        }

        public void LoadObjectives()
        {
            m_objectiveList = new List<Objective>();
            Objective objective = new Objective();
            for (int i = 0; i < CurrentQuest.Objectives.Count; i++)  //
            {

                objective.Obj = CurrentQuest.Objectives[i].Obj;
                objective.ObjType = CurrentQuest.Objectives[i].ObjType;
                objective.Amount = 0;
                objective.Name = CurrentQuest.Objectives[i].Name;

                m_objectiveList.Add(objective);
            }
        }
        
        public bool IsQuestComplete()
        {
            int nrOfObjectives = m_objectiveList.Count;
            int completedObjectives = 0;

            for (int i = 0; i < nrOfObjectives; i++)
            {
                if (m_objectiveList[i].Amount >= CurrentQuest.Objectives[i].Amount)
                {
                    completedObjectives++;
                }
            }

            if (completedObjectives == nrOfObjectives)
            {
                return true;
            }

            return false;
        }
        
        public void UpdateStatus()
        {
            switch (QuestStatus)
            {
                case PRE:
                    m_currentMessage = CurrentQuest.PreMessage;
                    m_activeNpc = CurrentQuest.QuestPickup;
                    break;
                case MID:
                    m_currentMessage = CurrentQuest.MidMessage;
                    m_activeNpc = CurrentQuest.QuestPickup;
                    break;
                case END:
                    m_currentMessage = CurrentQuest.EndMessage;
                    m_activeNpc = CurrentQuest.QuestTurnIn;
                    break;
            }
        }
        
        public void ActivateNextQuest()
        {
            if (!(m_questList.Last().Id == CurrentQuest.Id))
            {
                m_currentQuestIndex++;
                CurrentQuest.Status = PRE;
                LoadObjectives();
            }
            else
                m_allQuestsCompleted = true;
        }

    }
}
