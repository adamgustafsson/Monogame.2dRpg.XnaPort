using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Model
{
    class Backpack
    {
        private bool m_isOpen = false;
        private List<Item> m_backpackItems;
        private Vector2 m_position;

        public Backpack()
        {
            m_backpackItems = new List<Item>();
            m_position = new Vector2(855.0f, 322.0f);
        }

        //Görs för att mapobjekten ska följa med backpacken.
        public void UpdateBackpackItemPositions()
        {
            
        }

        public Vector2 Position
        {
            get { return m_position; }
            set { m_position = value; }
        }

        internal List<Item> BackpackItems
        {
            get { return m_backpackItems; }
            set { m_backpackItems = value; }
        }

        public bool IsOpen
        {
            get { return m_isOpen; }
            set { m_isOpen = value; }
        }
        
    }
}
