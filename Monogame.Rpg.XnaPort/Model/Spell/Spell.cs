using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FuncWorks.XNA.XTiled;

namespace Model
{
    class Spell
    {
        private float m_castTime;
        private float m_fullCastTime;
        private float m_coolDown;
        private float m_manaCost;
        private float m_range;
        private float m_duration;
        private Unit m_caster;
        private Vector2 m_position;

        //public Spell(Unit a_caster)
        //{

        //}

        public float FullCastTime
        {
            get { return m_fullCastTime; }
            set { m_fullCastTime = value; }
        }

        public Vector2 Position
        {
            get { return m_position; }
            set { m_position = value; }
        }

        internal Unit Caster
        {
            get { return m_caster; }
            set { m_caster = value; }
        }

        public float Duration
        {
            get { return m_duration; }
            set { m_duration = value; }
        }

        public float CoolDown
        {
            get { return m_coolDown; }
            set { m_coolDown = value; }
        }

        public float Range
        {
            get { return m_range; }
            set { m_range = value; }
        }

        public float ManaCost
        {
            get { return m_manaCost; }
            set { m_manaCost = value; }
        }

        public float CastTime
        {
            get { return m_castTime; }
            set { m_castTime = value; }
        }
    }
}
