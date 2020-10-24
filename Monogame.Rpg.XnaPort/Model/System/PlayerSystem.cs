using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    class PlayerSystem : UnitSystem
    {
        public Player m_player;
        public SpellSystem m_spellSystem;

        public PlayerSystem(Level a_level)
        {
            m_player = new Player(a_level);
            m_spellSystem = new SpellSystem();
        }

        public void Update(float a_elapsedTime, List<Model.Enemy> a_enemies)
        {
            m_player.Update();

            RegenerateMana(a_elapsedTime, m_player);

            RegenerateHp(a_elapsedTime, m_player);

            m_spellSystem.Update(a_elapsedTime);

            DecreaseGlobalCD(m_player, a_elapsedTime);

            //Tar bort en spelares target om fienden dör eller evadar.
            if(m_player.Target != null && m_player.Target.GetType() == GameModel.ENEMY_NPC)
            {
                Enemy enemy = m_player.Target as Enemy;
                if(!m_player.Target.IsAlive() || enemy.IsEvading)
                {
                    m_player.Target = null;
                    m_player.IsAttacking = false;
                }
            }

            //Kollar om spelaren attackerar.
            if (m_player.IsAttacking)
            {
                m_player.InCombat = true;
                //Testar om spelaren och hans target kolliderar / är i range för hitboxen.
                if (m_player.Target.ThisUnit.Bounds.Intersects(m_player.PlayerArea))
                {
                    m_player.IsWithinMeleRange = true;
                    //Om spelarens hit cooldown är mindre än 0 så slår spelaren.
                    if (m_player.SwingTime <= 0)
                    {
                        //Om player target fienden lever så sänks hans hp.
                        if (m_player.Target.IsAlive() && m_player.IsAlive())
                        {
                            m_player.Target.CurrentHp -= (m_player.AutohitDamage - m_player.Target.Armor);
                        }
                        //Sätter cooldown på ett slag.
                        m_player.SwingTime = 50;
                    }
                    else if (!m_player.IsCastingSpell)
                    {
                        m_player.SwingTime -= 1;
                    }
                }
                else
                {
                    m_player.IsWithinMeleRange = false;
                }
            }
            else 
            {
                m_player.InCombat = false;
            }

            if(!m_player.IsAlive())
            {
                m_player.Target = null;
                m_player.IsAttacking = false;

                if (m_player.SpawnTimer < 0)
                {
                    m_player.Spawn();
                }
                else 
                {
                    m_player.SpawnTimer -= a_elapsedTime;
                }
            }
        }
    }
}
