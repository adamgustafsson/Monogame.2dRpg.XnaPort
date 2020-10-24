using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework;

namespace Model
{
    class Unit
    {
        private Unit m_target;
        private MapObject m_thisUnit;

        private int m_weaponState;
        private int m_unitId;
        private bool m_isAttacking;
        private bool m_inCombat;
        private int m_autohitDamage;
        private float m_swingTime;
        private float m_globalCooldown;        
        
        private float m_totalHp;
        private float m_currentHp;
        private float m_totalMana;
        private float m_currentMana;
        private float m_manaRegen;
        private float m_hpRegen;
        private float m_armor;
        private float m_resist;
        private float m_spellPower;

        private float m_spawnTimer;
        
        private float m_moveSpeed;
        private bool m_canAddToQuest;
        private bool m_isCastingSpell;
        private Vector2 m_direction;
        private Vector2 m_moveToPosition;
        private int m_type;
        private Backpack m_backPack = new Backpack();

        //UnitStates
        #region UnitStates Konstanter
        public const int FACING_CAMERA = 1;
        public const int FACING_LEFT = 2;
        public const int FACING_RIGHT = 3;
        public const int FACING_AWAY = 4;
        public const int MOVING_LEFT = 5;
        public const int MOVING_RIGHT = 6;
        public const int MOVING_UP = 7;
        public const int MOVING_DOWN = 8;
        public const int IS_DEAD = 9;
        public const int WAS_HEALED = 10;
        public const int IS_CASTING_FIREBALL = 11;
        public const int IS_CASTING_HEAL = 12;
        #endregion

        private int m_unitState;

        public Unit()
        {
            m_isCastingSpell = false;
            m_manaRegen = 1;
            m_hpRegen = 1;
        }

        #region  Target/ThisUnit
        public Unit Target
        {
            get { return m_target; }
            set { m_target = value; }
        }
        public MapObject ThisUnit
        {
            get { return m_thisUnit; }
            set { m_thisUnit = value; }
        }
        #endregion

        #region IsAttacking/DMG/SwingTime/GCD
        public bool IsAttacking
        {
            get { return m_isAttacking; }
            set { m_isAttacking = value; }
        }
        public int AutohitDamage
        {
            get { return m_autohitDamage; }
            set { m_autohitDamage = value; }
        }
        public float SwingTime
        {
            get { return m_swingTime; }
            set { m_swingTime = value; }
        }
        public float GlobalCooldown
        {
            get { return m_globalCooldown; }
            set { m_globalCooldown = value; }
        }
        #endregion

        #region TotalHp/CurrentHp/IsAlive

        public float Armor
        {
            get { return m_armor; }
            set { m_armor = value; }
        }
        public float Resist
        {
            get { return m_resist; }
            set { m_resist = value; }
        }

        public float SpellPower
        {
            get { return m_spellPower; }
            set { m_spellPower = value; }
        }

        public int UnitId
        {
            get { return m_unitId; }
            set { m_unitId = value; }
        }
        public float TotalHp
        {
            get { return m_totalHp; }
            set { m_totalHp = value; }
        }
        public float CurrentHp
        {
            get { return m_currentHp; }
            set { m_currentHp = value; }
        }
        public float CurrentMana
        {
            get { return m_currentMana; }
            set { m_currentMana = value; }
        }
        public float TotalMana
        {
            get { return m_totalMana; }
            set { m_totalMana = value; }
        }
        public float ManaRegen
        {
            get { return m_manaRegen; }
            set { m_manaRegen = value; }
        }
        public float HpRegen
        {
            get { return m_hpRegen; }
            set { m_hpRegen = value; }
        }
        internal bool IsAlive()
        {
            if (m_currentHp <= 0)
            {
                return false;
            }
            return true;
        }
        #endregion

        public int Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public float SpawnTimer
        {
            get { return m_spawnTimer; }
            set { m_spawnTimer = value; }
        }

        public bool CanAddToQuest
        {
            get { return m_canAddToQuest; }
            set { m_canAddToQuest = value; }
        }

        public Vector2 Direction
        {
            get { return m_direction; }
            set { m_direction = value; }
        }

        public int UnitState
        {
            get { return m_unitState; }
            set { m_unitState = value; }
        }

        public float MoveSpeed
        {
            get { return m_moveSpeed; }
            set { m_moveSpeed = value; }
        }

        public bool IsCastingSpell
        {
            get { return m_isCastingSpell; }
            set { m_isCastingSpell = value; }
        }

        internal Backpack BackPack
        {
            get { return m_backPack; }
            set { m_backPack = value; }
        }

        public Vector2 MoveToPosition
        {
            get { return m_moveToPosition; }
            set { m_moveToPosition = value; }
        }

        public int WeaponState
        {
            get { return m_weaponState; }
            set { m_weaponState = value; }
        }

        public bool InCombat
        {
            get { return m_inCombat; }
            set { m_inCombat = value; }
        }
    }
}
