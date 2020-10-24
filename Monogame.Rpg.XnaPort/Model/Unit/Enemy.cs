using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework;

namespace Model
{
    class Enemy : Unit
    {
        //Typ ID
        public const int CLASS_WARRIOR = 0;
        public const int CLASS_ARCHER = 1;
        public const int CLASS_MAGE = 2;
        public const int CLASS_GOBLIN = 3;
        public const int BOSS_A = 4;

        //Spawntimer och position
        private Point m_spawnPosition;

        //Fiendezonen
        private Rectangle m_enemyZone;

        //Rörelse
        private Vector2 m_direction;

        //Aggroavstånd
        private Vector2 m_aggroRange;
        private bool m_isEvading;

        private bool m_isActive;

        //TARGET DISLOCATION - JOHAN SPECIAL
        private int m_targetDisLocationX;
        private int m_targetDisLocationY;

        public Enemy(MapObject a_thisUnit, int a_type, int a_enemyId, Rectangle a_enemyZone)
        {
            this.GlobalCooldown = 0.5f;
            m_enemyZone = a_enemyZone;
            this.UnitId = a_enemyId;
            this.Type = a_type;
            m_spawnPosition = a_thisUnit.Bounds.Location;
            this.ThisUnit = a_thisUnit;
            this.ThisUnit.Bounds.Width = 64;
            this.ThisUnit.Bounds.Height = 64;
            this.CanAddToQuest = true;

            //Kollar vilken typ av fiende.
            //WARRIOR
            if(a_thisUnit.Properties["Type"].AsInt32 == CLASS_WARRIOR)
            {
                this.TotalHp = 100;
                this.AutohitDamage = 3;
                this.MoveSpeed = 2.0f;
            }
            //GOBLIN
            if (a_thisUnit.Properties["Type"].AsInt32 == CLASS_GOBLIN)
            {
                this.TotalHp = 85;
                this.AutohitDamage = 2;
                this.MoveSpeed = 3.0f;
            }
            //MAGE
            if (a_thisUnit.Properties["Type"].AsInt32 == CLASS_MAGE)
            {
                this.TotalHp = 75;
                this.TotalMana = 20;
                this.CurrentMana = this.TotalMana;
                this.AutohitDamage = 1;
                this.MoveSpeed = 2.0f;
            }
            //Första bossen.
            if (a_thisUnit.Properties["Type"].AsInt32 == BOSS_A)
            {
                this.TotalHp = 125;
                this.TotalMana = 50;
                this.SpellPower = 5;
                //this.Armor = 5;
                this.CurrentMana = this.TotalMana;
                this.AutohitDamage = 5;
                this.MoveSpeed = 2.0f;
                Model.QuestItem questItem = new Model.QuestItem(QuestItem.ENEMY_HEAD);
                this.BackPack.BackpackItems.Add(questItem);
            }

            this.CurrentHp = this.TotalHp;
        }

        #region Spawn
        public Point SpawnPosition
        {
            get { return m_spawnPosition; }
            set { m_spawnPosition = value; }
        }
        #endregion

        #region Aggro
        public Vector2 AggroRange
        {
            get { return m_aggroRange; }
            set { m_aggroRange = value; }
        }
        #endregion

        public int TargetDisLocationX
        {
            get { return m_targetDisLocationX; }
            set { m_targetDisLocationX = value; }
        }

        public int TargetDisLocationY
        {
            get { return m_targetDisLocationY; }
            set { m_targetDisLocationY = value; }
        }

        public bool IsActive
        {
            get { return m_isActive; }
            set { m_isActive = value; }
        }

        public bool IsEvading
        {
            get { return m_isEvading; }
            set { m_isEvading = value; }
        }

        public bool WaitingToSpawn()
        {
            if (SpawnTimer > 0)
                return true;
            return false;
        }

        public Rectangle EnemyZone
        {
            get { return m_enemyZone; }
            set { m_enemyZone = value; }
        }

        public Vector2 Direction
        {
            get { return m_direction; }
            set { m_direction = value; }
        }
    }
}
