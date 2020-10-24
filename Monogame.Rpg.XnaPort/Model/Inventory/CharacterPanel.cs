using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Model
{
    class CharacterPanel
    {
        private bool m_isOpen = false;
        private List<Item> m_equipedItems;
        private Vector2 m_position;

        public CharacterPanel()
        {
            m_equipedItems = new List<Item>();
            m_position = new Vector2(250.0f, 150.0f);
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

        internal List<Item> EquipedItems
        {
            get { return m_equipedItems; }
            set { m_equipedItems = value; }
        }

        public bool IsOpen
        {
            get { return m_isOpen; }
            set { m_isOpen = value; }
        }
    }
}
