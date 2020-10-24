using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FuncWorks.XNA.XTiled;

namespace Model
{
    class InstantHeal : Spell
    {
        //Totala hälsan man får av en heal.
        private float m_heal;

        public InstantHeal(Unit a_caster)
        {
            this.ManaCost = 5;
            this.CastTime = 1f;
            this.FullCastTime = this.CastTime;
            this.Duration = 1;
            this.Range = 0;
            this.CoolDown = 5;
            this.Caster = a_caster;
            //Gör att man healar en 10del av livet.
            m_heal = a_caster.TotalHp / 4;
        }

        public float Heal
        {
            get { return m_heal; }
            set { m_heal = value; }
        }
    }
}
