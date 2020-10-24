using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    class Smite : Spell
    {
        private Unit m_target;
        private int m_damage = 15;

        public Smite(Unit a_caster)
        {
            this.ManaCost = 10;
            this.CastTime = 0;
            this.FullCastTime = this.CastTime;
            this.Duration = 1;
            this.Range = 0;
            this.CoolDown = 2;
            this.Caster = a_caster;
            this.Target = this.Caster.Target;
        }

        internal Unit Target
        {
            get { return m_target; }
            set { m_target = value; }
        }

        public int Damage
        {
            get { return m_damage; }
            set { m_damage = value; }
        }
    }
}
