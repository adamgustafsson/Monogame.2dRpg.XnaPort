using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Model
{
    class UnitSystem
    {
        #region MOVEMENT
        internal bool ArrivedToPosition(Rectangle a_player, Vector2 a_moveTo, int a_accuracy)
        {
            Vector2 difference = new Vector2(a_player.Center.X - (int)a_moveTo.X, a_player.Center.Y - (int)a_moveTo.Y);
            if ((difference.X > -a_accuracy && difference.X < a_accuracy) && (difference.Y > -a_accuracy && difference.Y < a_accuracy))
                return true;
            return false;
        }
        #endregion

        #region Regeneration HP / MANA
        public void RegenerateMana(float a_elapsedTime, Unit a_unit)
        {
            if (a_unit.IsAlive())
            {
                a_unit.ManaRegen -= a_elapsedTime;

                //Testar om det är dags för att regga mana, och att man inte redan har maxmana.
                if (a_unit.ManaRegen < 0 && a_unit.CurrentMana != a_unit.TotalMana)
                {
                    //Kan kolla om det är fienden eller player här för att bestämma hur mkt mana som ska reggas.
                    a_unit.CurrentMana += 1.5f;
                    if (a_unit.CurrentMana > a_unit.TotalMana)
                    {
                        a_unit.CurrentMana = a_unit.TotalMana;
                    }

                    a_unit.ManaRegen = 1;
                } 
            }
        }

        public void RegenerateHp(float a_elapsedTime, Unit a_unit)
        {
            if (!a_unit.IsAttacking && a_unit.IsAlive())
            {
                a_unit.HpRegen -= a_elapsedTime;
                if (a_unit.HpRegen < 0 && a_unit.CurrentHp != a_unit.TotalHp)
                {
                    //Kan kolla om det är fienden eller player här för att bestämma hur mkt hp som ska reggas. 
                    //Just nu så ska inte fiender regga hp.
                    a_unit.CurrentHp += 0.5f;
                    if (a_unit.CurrentHp > a_unit.TotalHp)
                    {
                        a_unit.CurrentHp = a_unit.TotalHp;
                    }
                    a_unit.HpRegen = 1;
                }
            }
        }
        #endregion

        public void DecreaseGlobalCD(Unit a_unit, float a_elapsedTime)
        {
            //Minskar spelarens global CD om sådan finns
            if (a_unit.GlobalCooldown > 0)
            {
                a_unit.GlobalCooldown -= a_elapsedTime;
                if (a_unit.GlobalCooldown < 0)
                {
                    a_unit.GlobalCooldown = 0;
                }
            }
        }
    }
}
