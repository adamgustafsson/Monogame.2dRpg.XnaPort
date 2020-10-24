using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Model
{
    class Fireball : Spell
    {
        //Totala skadan en spell gör.
        private float m_damage;
        private Rectangle m_fireBallArea;
        private Unit m_target;
        private Vector2 m_direction;
        private bool m_wasCasted = false;

        public Fireball(Unit a_caster)
        {   //8 är för att förflytta spellen till mitten på gubben när han är 48x48 pch spellen är 32x32     48-32 = 16, 16/2 = 8
            Vector2 position = new Vector2(a_caster.ThisUnit.Bounds.X + 8, a_caster.ThisUnit.Bounds.Y + 8);
            this.ManaCost = 10;
            m_damage = 8;
            this.Position = position;
            this.CastTime = 3f;
            this.FullCastTime = this.CastTime;
            this.Duration = 1;
            this.Range = 0;
            this.CoolDown = 5;
            this.Caster = a_caster;
            Update(a_caster.Target);
        }

        public void Update(Unit a_target)
        {
            m_fireBallArea = new Rectangle((int)this.Position.X, (int)this.Position.Y, 32, 32);
            m_target = a_target;
        }

        public Vector2 Direction
        {
            get { return m_direction; }
            set { m_direction = value; }
        }

        internal Unit Target
        {
            get { return m_target; }
            set { m_target = value; }
        }

        public float Damage
        {
            get { return m_damage; }
            set { m_damage = value; }
        }

        public Rectangle FireBallArea
        {
            get { return m_fireBallArea; }
            set { m_fireBallArea = value; }
        }

        public bool WasCasted
        {
            get { return m_wasCasted; }
            set { m_wasCasted = value; }
        }
    }
}
