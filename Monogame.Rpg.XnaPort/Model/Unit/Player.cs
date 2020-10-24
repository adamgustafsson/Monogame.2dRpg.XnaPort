using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework;

namespace Model
{
    class Player : Unit
    {
        private bool m_isLookingAtMap = false;
        private Level m_level;

        private Point m_lastPosition;

        private Item m_itemTarget;
        private Item m_BackpackTarget;
        private Item m_charPanelTarget;

        private Enemy m_lootTarget;

        private bool m_hasHelm = false;

        private CharacterPanel m_charPanel;

        private Rectangle m_playerArea;
        private Rectangle m_collisionArea;
        private Rectangle m_maxRangeArea;

        private bool m_canMoveRight = true;
        private bool m_canMoveLeft = true;
        private bool m_canMoveUp = true;
        private bool m_canMoveDown = true;
        private bool m_isWithinMeleRange;
        private Random m_crittChance;

        //Klasser
        public const int TEMPLAR = 0;
        public const int PROPHET = 1;
        public const int DESCENDANT = 2;
        
        public Player(Level a_level)
        {
            this.ThisUnit = a_level.PlayerLayer.MapObjects[0];
            this.ThisUnit.Bounds.Width = 48;
            this.ThisUnit.Bounds.Height = 48;

            this.Type = TEMPLAR;

            this.SpawnTimer = 2f;

            //STATS
            this.TotalHp = 100;
            this.CurrentHp = this.TotalHp;
            this.TotalMana = 50;
            this.CurrentMana = this.TotalMana;
            this.Armor = 0;
            this.Resist = 0;

            m_charPanel = new CharacterPanel();
            m_level = a_level;

            this.GlobalCooldown = 0;
            this.AutohitDamage = 10;

            this.m_crittChance = new Random();

            this.Update();
        }

        #region ITEMS / BACKPACK

        internal CharacterPanel CharPanel
        {
            get { return m_charPanel; }
            set { m_charPanel = value; }
        }

        internal Item BackpackTarget
        {
            get { return m_BackpackTarget; }
            set { m_BackpackTarget = value; }
        }

        public Item ItemTarget
        {
            get { return m_itemTarget; }
            set { m_itemTarget = value; }
        }

        internal Item CharPanelTarget
        {
            get { return m_charPanelTarget; }
            set { m_charPanelTarget = value; }
        }

        public bool HasHelm
        {
            get { return m_hasHelm; }
            set { m_hasHelm = value; }
        }

        internal Enemy LootTarget
        {
            get { return m_lootTarget; }
            set { m_lootTarget = value; }
        }
        
        #endregion

        #region CanMove
        public bool CanMoveDown
        {
            get { return m_canMoveDown; }
            set { m_canMoveDown = value; }
        }
        public bool CanMoveUp
        {
            get { return m_canMoveUp; }
            set { m_canMoveUp = value; }
        }
        public bool CanMoveLeft
        {
            get { return m_canMoveLeft; }
            set { m_canMoveLeft = value; }
        }
        public bool CanMoveRight
        {
            get { return m_canMoveRight; }
            set { m_canMoveRight = value; }
        }
        #endregion

        #region Intersect/Collision
        public Rectangle PlayerArea
        {
            get { return m_playerArea; }
            set { m_playerArea = value; }
        }

        public Rectangle CollisionArea
        {
            get { return m_collisionArea; }
            set { m_collisionArea = value; }
        }
        public Rectangle MaxRangeArea
        {
            get { return m_maxRangeArea; }
            set { m_maxRangeArea = value; }
        }

        #endregion

        #region Positioner

        public Point LastPosition
        {
            get { return m_lastPosition; }
            set { m_lastPosition = value; }
        }
        #endregion

        //Hanterar kollisioner
        public void Update()
        {
            m_playerArea = new Rectangle(this.ThisUnit.Bounds.X, this.ThisUnit.Bounds.Y, this.ThisUnit.Bounds.Width, this.ThisUnit.Bounds.Height);
            m_collisionArea = new Rectangle(this.ThisUnit.Bounds.X - 30, this.ThisUnit.Bounds.Y - 30, this.ThisUnit.Bounds.Width + 60, this.ThisUnit.Bounds.Height + 60);
            m_maxRangeArea = new Rectangle(this.ThisUnit.Bounds.X - 150, this.ThisUnit.Bounds.Y - 150, this.ThisUnit.Bounds.Width + 300, this.ThisUnit.Bounds.Height + 300);

            if (m_crittChance.Next(1, 10) == 1 && this.SwingTime == 50)
                this.AutohitDamage += m_crittChance.Next(2, 7);
            else if (this.AutohitDamage != 10 && this.SwingTime == 50)
                this.AutohitDamage = 10;
        }

        public void Spawn()
        {
            Point playerPos = ThisUnit.Bounds.Location;
            Point nearestGraveyard = new Point();
            int nearestDiffernce = 0;

            for (int i = 0; i < m_level.GraveyardLayer.MapObjects.Count(); i++)
            {
                double p1 = Math.Pow((playerPos.X - m_level.GraveyardLayer.MapObjects[i].Bounds.Location.X), 2);
                double p2 = Math.Pow((playerPos.Y - m_level.GraveyardLayer.MapObjects[i].Bounds.Location.Y), 2);
                double r = p1 + p2;
                int differnce = (int)Math.Sqrt(r);

                if (nearestGraveyard == new Point())
                {
                    nearestGraveyard = m_level.GraveyardLayer.MapObjects[i].Bounds.Location;
                    nearestDiffernce = differnce;
                }
                else if (differnce < nearestDiffernce)
                {
                    nearestGraveyard = m_level.GraveyardLayer.MapObjects[i].Bounds.Location;
                    nearestDiffernce = differnce;
                }
            }
            this.ThisUnit.Bounds.Location = nearestGraveyard;
            this.CurrentHp = this.TotalHp;
            this.IsCastingSpell = false;
            this.SpawnTimer = 2;
            //Resettar "move to location"
            this.MoveToPosition = Vector2.Zero;
            this.Direction = Vector2.Zero;
        }

        public bool IsLookingAtMap
        {
            get { return m_isLookingAtMap; }
            set { m_isLookingAtMap = value; }
        }    
   
        public bool IsWithinMeleRange
        {
            get { return m_isWithinMeleRange; }
            set { m_isWithinMeleRange = value; }
        }
    }
}
