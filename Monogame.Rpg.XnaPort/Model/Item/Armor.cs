using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework;

namespace Model
{
    class Armor : Item
    {
        #region Variabler

        //Olika typer av armor
        public const int HEAD_ARMOR = 1;
        public const int CHEST_ARMOR = 2;

        //Värden för armor.
        private int m_armorValue;
        private int m_magicResistValue;
        private int m_healthValue;
        private int m_manaValue;

        #endregion

        #region Konstruktorer

        //Används för armor som laddas in från kartan.
        public Armor(MapObject a_armor, int a_id)
        {
            this.ThisItem = a_armor;
            m_armorValue = Convert.ToInt32(a_armor.Properties["Armor"].AsInt32);
            this.Type = Convert.ToInt32(a_armor.Properties["Type"].AsInt32);
            this.ThisItem.Bounds.Width = 48;
            this.ThisItem.Bounds.Height = 48;
            this.CanAddToQuest = true;
            this.WasLooted = false;
            this.ItemId = a_id;
        }

        //Används för armor som skapas i C#.
        public Armor(int a_armorValue, int a_type)
        {
            m_armorValue = a_armorValue;
            this.Type = a_type;
            this.CanAddToQuest = true;
            this.WasLooted = false;
            this.ThisItem = new MapObject();
            this.ThisItem.Bounds.Width = 48;
            this.ThisItem.Bounds.Height = 48;
        }

        #endregion

        #region Get/Set

        public int ArmorValue
        {
            get { return m_armorValue; }
            set { m_armorValue = value; }
        }

        public int MagicResistValue
        {
            get { return m_magicResistValue; }
            set { m_magicResistValue = value; }
        }

        public int HealthValue
        {
            get { return m_healthValue; }
            set { m_healthValue = value; }
        }

        public int ManaValue
        {
            get { return m_manaValue; }
            set { m_manaValue = value; }
        }


        #endregion
    }
}
