using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncWorks.XNA.XTiled;

namespace Model
{
    class Item
    {
        private MapObject m_thisItem;
        private float m_spawnTime = 0;
        private float m_spawnCounter;
        private int m_type;
        private int m_itemId;
        private bool m_canAddToQuest;
        private bool m_wasLooted;

        public int ItemId
        {
            get { return m_itemId; }
            set { m_itemId = value; }
        }

        public bool WasLooted
        {
            get { return m_wasLooted; }
            set { m_wasLooted = value; }
        }

        public bool CanAddToQuest
        {
            get { return m_canAddToQuest; }
            set { m_canAddToQuest = value; }
        }

        public float SpawnCounter
        {
            get { return m_spawnCounter; }
            set { m_spawnCounter = value; }
        }

        public float SpawnTime
        {
            get { return m_spawnTime; }
            set { m_spawnTime = value; }
        }

        public int Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public MapObject ThisItem
        {
            get { return m_thisItem; }
            set { m_thisItem = value; }
        }
    }
}
