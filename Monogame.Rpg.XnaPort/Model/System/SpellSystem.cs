using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncWorks.XNA.XTiled;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Model
{
    class SpellSystem
    {
        //Spells
        private List<Spell> m_activeSpells;
        
        //GlobalCd
        private float m_globalCd = 0.2f;
        
        //Typ-referenser
        public static Type INSTANT_HEAL = typeof(Model.InstantHeal);
        public static Type FIRE_BALL = typeof(Model.Fireball);
        public static Type SMITE = typeof(Model.Smite);

        //Konstruktor
        public SpellSystem()
        { 
            m_activeSpells = new List<Spell>();
        }

        internal void Update(float a_elapsedTime)
        {
            //Rensar listan från spells där.. 
            m_activeSpells.RemoveAll(Spell => (Spell.Duration == 0 && Spell.CoolDown <= 0) ||                     //Duration och Cooldown är noll
                                              (!Spell.Caster.IsAlive()) ||                                        //Kastaren är död
                                              (Spell.Caster.Target != null && (Spell.GetType() == FIRE_BALL && !Spell.Caster.Target.IsAlive())) ||
                                              (Spell.GetType() == FIRE_BALL && Spell.Caster.Target == null));  //Fireballens target är död eller null



            //Uppdaterar varje aktiv instantheal  
            foreach (Spell spell in m_activeSpells)
            { 
                #region InstanHeal
                if(spell.GetType() == SpellSystem.INSTANT_HEAL)
                {
                    InstantHeal instantHeal = spell as Model.InstantHeal;

                    //Om casttiden är klar och spellen inte är påbörjad
                    if (instantHeal.CastTime <= 0 && instantHeal.Duration != 0)
                    {
                        //kasta spell
                        instantHeal.Caster.IsCastingSpell = false;
                        CastInstantHeal(instantHeal);
                    }
                    //Annars om kast-tid finns: minska den
                    else if (spell.CastTime > 0)
                    {
                        if(spell.Caster.GetType() == GameModel.ENEMY_NPC)
                        {
                            spell.Caster.UnitState = Enemy.IS_CASTING_HEAL;
                        }
                        instantHeal.CastTime -= a_elapsedTime;
                    }
                    //Annars minska spellens cd        
                    else
                    {
                        instantHeal.CoolDown -= a_elapsedTime;
                    }
                } 
                #endregion
                
                #region FireBall
                else if(spell.GetType() == SpellSystem.FIRE_BALL)
                {
                    Fireball fireBall = spell as Model.Fireball;
                    //Uppdaterar fireballens target.
                    fireBall.Update(spell.Caster.Target);

                    //Om Casttime är nedräknad och spellen inte träffat sitt target.
                    if (fireBall.CastTime <= 0 && spell.Duration != 0)
                    {
                        //Gör så att fireballen fick status kastad om den inte hade det.
                        if(!fireBall.WasCasted)
                        {
                            CastFireBall(fireBall);
                            fireBall.WasCasted = true;
                        }

                        //Om spellen har träffat sitt target.
                        if (fireBall.Target.ThisUnit.Bounds.Intersects(fireBall.FireBallArea) && fireBall.Duration > 0)
                        {
                            //Gör skada
                            fireBall.Caster.Target.CurrentHp -= (int)fireBall.Damage + fireBall.Caster.SpellPower;

                            //Säger att spellen träffat.
                            spell.Duration = 0;
                        }

                        //Uppdaterar spellen.
                        fireBall.Direction = new Vector2(fireBall.Target.ThisUnit.Bounds.X, fireBall.Target.ThisUnit.Bounds.Y) - fireBall.Position;
                        Vector2 newCordinates = new Vector2();
                        newCordinates = fireBall.Direction;
                        newCordinates.Normalize();
                        fireBall.Position += newCordinates * 5;
                        fireBall.Caster.IsCastingSpell = false;
                    }
                    //Om kast-tid finns: minska den
                    if (spell.CastTime > 0)
                    {
                        //Medans spelares kastar så kollar han neråt.
                        spell.Caster.UnitState = Enemy.IS_CASTING_FIREBALL;

                        fireBall.CastTime -= a_elapsedTime;
                    }
                    //Annars minska spellens cd        
                    else
                    {
                        fireBall.CoolDown -= a_elapsedTime;
                    }
                }
                #endregion

                #region Smite

                else if (spell.GetType() == SpellSystem.SMITE)
                {
                    Smite smite = spell as Model.Smite;

                    //Om Casttime är nedräknad och spellen inte träffat sitt target.
                    if (smite.CastTime <= 0 && spell.Duration != 0)
                    {
                        CastSmite(smite);
                    }
                    //Om kast-tid finns: minska den
                    if (spell.CastTime > 0)
                    {
                        if (spell.Caster.GetType() == GameModel.PLAYER)
                        {
                            spell.Caster.UnitState = Player.FACING_CAMERA;
                        }

                        smite.CastTime -= a_elapsedTime;
                    }
                    //Annars minska spellens cd        
                    else
                    {
                        smite.CoolDown -= a_elapsedTime;
                    }
                }

                #endregion
            } 
        }

        public void CastInstantHeal(InstantHeal a_instantHeal)
        {
            //Sätter kastarens global cooldown
            a_instantHeal.Caster.GlobalCooldown = m_globalCd;

            //Healar
            a_instantHeal.Caster.CurrentHp += (int)a_instantHeal.Heal + a_instantHeal.Caster.SpellPower;

            //Sänker spelarens mana.
            a_instantHeal.Caster.CurrentMana -= a_instantHeal.ManaCost;

            //Kontrollerar att spelaren inte har mer hp än hans max. Om det är över så sätts livet till max hp.
            if (a_instantHeal.Caster.CurrentHp > a_instantHeal.Caster.TotalHp)
            {
                a_instantHeal.Caster.CurrentHp = a_instantHeal.Caster.TotalHp;
            }

            //Deklarrerar att spellen har uppdaterats färdigt
            a_instantHeal.Duration = 0;
        }

        public void CastFireBall(Fireball a_fireBall)
        {
            //Sätter kastarens global cooldown
            a_fireBall.Caster.GlobalCooldown = m_globalCd;
            //Sänker spelarens mana.
            a_fireBall.Caster.CurrentMana -= a_fireBall.ManaCost;

        }

        public void CastSmite(Smite a_smite)
        {
            //Sätter kastarens global cooldown
            a_smite.Caster.GlobalCooldown = m_globalCd;

            //Smite skadan.
            if(a_smite.Target != null)
            {
                //Ser till så att fienden som blir träffad av smiten börjar attackera.
                a_smite.Target.Target = a_smite.Caster;
                a_smite.Target.IsAttacking = true;
                a_smite.Target.CurrentHp -= a_smite.Damage;

                //Sänker spelarens mana.
                a_smite.Caster.CurrentMana -= a_smite.ManaCost;
            }

            //Säger till att spellen kastats.
            a_smite.Duration = 0;
            a_smite.Caster.IsCastingSpell = false;
        }

        internal void AddSpell(Type a_spellType, Unit a_caster)
        {
            //Kollar att kastaren har rätt att kasta.
            if (CanCastSpell(a_spellType, a_caster))
            {
                //Skapar ny spell enligt angiven typ.
                Spell spellToAdd = (Model.Spell)Activator.CreateInstance(a_spellType, a_caster);

                //Kontrollerar så att spelaren har tillräckligt med mana för att kasta spellen.
                if(spellToAdd.Caster.CurrentMana >= spellToAdd.ManaCost)
                {
                    a_caster.IsCastingSpell = true;
                    m_activeSpells.Add(spellToAdd);
                }
            }
        }

        internal bool CanCastSpell(Type a_spellType, Unit a_caster)
        {
            //Om kastaren inte har global CD.
            if (a_caster.GlobalCooldown <= 0)
            {
                //Om det INTE finns en spell i listan vars CasterID redan har en spell av samma sort med kvarvarande cd.
                return (!m_activeSpells.Exists(Spell => Spell.Caster.UnitId == a_caster.UnitId && Spell.CoolDown > 0 && Spell.GetType() == a_spellType));
            }
            return false;
        }

        internal List<Spell> ActiveSpells
        {
            get { return m_activeSpells; }
            set { m_activeSpells = value; }
        }
    }
}
