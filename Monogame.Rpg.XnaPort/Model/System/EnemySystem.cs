using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework;

namespace Model
{
    class EnemySystem : UnitSystem
    {
        public List<Model.Enemy> m_enemies;
        public List<Model.Enemy> m_spawnList;
        public SpellSystem m_enemySpellSystem;

        private Random m_random = new Random();
        private int m_enemyId;

        public EnemySystem(ObjectLayer a_enemyLayer, Level a_level, Map a_currentMap)
        {
            m_enemies = new List<Model.Enemy>();
            m_spawnList = new List<Enemy>();
            m_enemySpellSystem = new SpellSystem();
            m_enemyId = 0;
            LoadEnemies(a_enemyLayer, a_level, a_currentMap);
        }

        private void LoadEnemies(ObjectLayer a_enemyLayer, Level a_level, Map a_currentMap)
        {
            foreach (MapObject enemy in a_enemyLayer.MapObjects)
            {
                //TODO: Fördela roller via Objektnamn tex W, M 
                m_enemyId++;
                foreach (var enemyZone in a_level.EnemyZoneLayer.MapObjects)
                {
                    if(enemy.Bounds.Intersects(enemyZone.Bounds))
                    {
                        m_enemies.Add(new Enemy(enemy, Convert.ToInt32(enemy.Properties["Type"].AsInt32), m_enemyId, enemyZone.Bounds));
                    }
                }
            }
        }

        public void Update(Player a_player, float a_elapsedTime)
        {
            m_enemySpellSystem.Update(a_elapsedTime);
            EnemySpawnList(a_player);

            //Sätts till true om en fiende attackerar.
            a_player.InCombat = false;

            foreach (Model.Enemy enemy in m_enemies)
            {          
                //BUGGFIX måste ta dän enemy target för mages....nåt konstigt.
                if (enemy.Target != null)
                {
                 if (!enemy.Target.IsAlive())
                    {
                        enemy.IsEvading = true;
                        enemy.IsAttacking = false;
                        enemy.Target = null;
                        enemy.IsCastingSpell = false;
                        MoveToTarget(enemy, Vector2.Zero);
                    }
                }

                //Decrease GCD
                DecreaseGlobalCD(enemy, a_elapsedTime);

                //Kontrollerar om det är en mage eller boss, isf ska han regga mana.
                if (enemy.Type == Enemy.CLASS_MAGE || enemy.Type == Enemy.BOSS_A)
                {
                    RegenerateMana(a_elapsedTime, enemy);
                }

                //Om fienden är aktiv
                if (enemy.IsActive && enemy.ThisUnit.Bounds.Intersects(enemy.EnemyZone) && !enemy.IsEvading)
                {
                    //Testar om fienden redan attackerar.
                    if (enemy.IsAttacking)
                    {
                        EnemyAttack(enemy, a_player);
                    }
                    //Testar om fienden aggras av något.
                    else
                    {
                        CheckEnemyAggro(enemy, a_player);
                    }
                }
                //Fienden slutar attackera.
                else
                {
                    enemy.IsEvading = true;
                    enemy.IsAttacking = false;
                }

                if (enemy.IsEvading)
                {
                    EnemyEvade(enemy);
                }

                SpawnEnemy(enemy);
            }

            m_enemies.RemoveAll(Enemy => !Enemy.IsAlive());
            m_enemies.OrderByDescending(Enemy => Enemy.ThisUnit.Bounds.Y);
        }

        private void CheckEnemyAggro(Enemy a_enemy, Player a_player)
        {
            a_enemy.UnitState = Enemy.FACING_CAMERA;

            int aggroRange = SetAggroRange(a_enemy);

            //2 ifsatser som testar om spelaren är innanför fiendens "Aggro zon".
            if (a_player.ThisUnit.Bounds.Center.X > a_enemy.ThisUnit.Bounds.Center.X - aggroRange && a_player.ThisUnit.Bounds.Center.X < a_enemy.ThisUnit.Bounds.Center.X + aggroRange)
            {
                if ((a_player.ThisUnit.Bounds.Center.Y > a_enemy.ThisUnit.Bounds.Center.Y - aggroRange && a_player.ThisUnit.Bounds.Center.Y < a_enemy.ThisUnit.Bounds.Center.Y + aggroRange) && a_player.IsAlive())
                {
                    //Sätter fiendens target till spelaren.
                    a_enemy.TargetDisLocationX = m_random.Next(0, 49);
                    a_enemy.TargetDisLocationY = m_random.Next(0, 49);
                    a_enemy.Target = a_player;
                    a_enemy.IsAttacking = true;
                }
            }
        }

        private void EnemyAttack(Enemy a_enemy, Player a_player)
        {
            a_player.InCombat = true;

            //Om fienden är en mage och kan kasta en fireball så gör han det.
            if (a_enemy.Type == Model.Enemy.CLASS_MAGE && !a_enemy.IsCastingSpell)
            {
                if (m_enemySpellSystem.CanCastSpell(SpellSystem.FIRE_BALL, a_enemy))
                {
                    m_enemySpellSystem.AddSpell(SpellSystem.FIRE_BALL, a_enemy);
                }
            }
            //Om fienden är bossA och kan kasta fireball eller heal så gör han det.
            if (a_enemy.Type == Model.Enemy.BOSS_A && !a_enemy.IsCastingSpell)
            {
                //Kastar heal om han har förlorat halva livet.
                if (a_enemy.CurrentHp < a_enemy.TotalHp / 2 && m_enemySpellSystem.CanCastSpell(SpellSystem.INSTANT_HEAL, a_enemy) && a_enemy.Type == Model.Enemy.BOSS_A)
                {
                    m_enemySpellSystem.AddSpell(SpellSystem.INSTANT_HEAL, a_enemy);
                }
                else if (m_enemySpellSystem.CanCastSpell(SpellSystem.FIRE_BALL, a_enemy))
                {
                    m_enemySpellSystem.AddSpell(SpellSystem.FIRE_BALL, a_enemy);
                }
            }
            //Om fienden inte kastar en spell så kan han röra sig.
            if (!a_enemy.IsCastingSpell)
            {
                //@Param: Enemy, Vector2 Where to move
                MoveToTarget(a_enemy, new Vector2(a_enemy.Target.ThisUnit.Bounds.X, a_enemy.Target.ThisUnit.Bounds.Y));

                //Bankar och slår
                if (a_enemy.ThisUnit.Bounds.Intersects(a_player.PlayerArea) && a_enemy.IsAlive())
                {
                    if (a_enemy.SwingTime < 0)
                    {
                        //När en fiende slå så minskar skadan med spelarens armor.
                        a_player.CurrentHp -= (a_enemy.AutohitDamage - a_player.Armor);

                        if (a_player.CurrentHp > a_player.TotalHp)
                        {
                            a_player.CurrentHp = a_player.TotalHp;
                        }

                        a_enemy.SwingTime = 20;
                    }
                    else
                    {
                        a_enemy.SwingTime -= 1;
                    }
                }
            }
        }

        private void EnemyEvade(Enemy a_enemy)
        {
            //Feinden går tillbaka till spawn om den inte attackerar
            //@Param: Enemy, Where to move
            if (a_enemy.ThisUnit.Bounds.Location != a_enemy.SpawnPosition && !a_enemy.IsAttacking)
            {
                //Evade :P
                a_enemy.CurrentHp = a_enemy.TotalHp;
                MoveToTarget(a_enemy, new Vector2(a_enemy.SpawnPosition.X, a_enemy.SpawnPosition.Y));
            }
            else if (!a_enemy.IsAttacking)
            {
                a_enemy.IsEvading = false;
            }
        }

        private void SpawnEnemy(Enemy a_enemy)
        {
            //Spawnar om fiender.
            if (!a_enemy.IsAlive())
            {
                if (a_enemy.Type == Model.Enemy.CLASS_GOBLIN)
                {
                    a_enemy.SpawnTimer = 2000;
                }
                else
                {
                    a_enemy.SpawnTimer = 10000;
                }

                m_spawnList.Add(a_enemy);
                a_enemy.UnitState = Enemy.IS_DEAD;
            }
        }

        public int SetAggroRange(Enemy a_enemy)
        {
            if (a_enemy.Type == Enemy.CLASS_WARRIOR)
            {
                return 200;
            }
            else if (a_enemy.Type == Enemy.CLASS_MAGE)
            {
                return 300;
            }
            else if (a_enemy.Type == Enemy.CLASS_GOBLIN)
            {
                return 200;
            }
            //Standard.
            else
            {
                return 200;
            }
        }
      
        internal void MoveToTarget(Model.Enemy a_enemy, Vector2 a_target)
        {
            //
            Vector2 moveTo = a_target;
            Vector2 newCords = Vector2.Zero;

            float xSpeed = 0;
            float ySpeed = 0;

            if (a_enemy.IsAttacking)
            {
                moveTo.X = a_enemy.Target.ThisUnit.Bounds.X + a_enemy.TargetDisLocationX;
                moveTo.Y = a_enemy.Target.ThisUnit.Bounds.Y + a_enemy.TargetDisLocationY;
            }
            if (a_enemy.IsEvading)
            {
                moveTo.X = a_enemy.SpawnPosition.X;
                moveTo.Y = a_enemy.SpawnPosition.Y;
            }

            a_enemy.Direction = new Vector2(moveTo.X - a_enemy.ThisUnit.Bounds.Center.X, moveTo.Y - a_enemy.ThisUnit.Bounds.Center.Y);

            xSpeed = Math.Abs(a_enemy.Direction.X);
            ySpeed = Math.Abs(a_enemy.Direction.Y);

            if (!ArrivedToPosition(a_enemy.ThisUnit.Bounds, moveTo, 5) &&  (a_enemy.Target != null && !a_enemy.ThisUnit.Bounds.Intersects(a_enemy.Target.ThisUnit.Bounds) || a_enemy.IsEvading))
            {
                newCords = a_enemy.Direction;
                newCords.Normalize();
                newCords.X = newCords.X * a_enemy.MoveSpeed;
                newCords.Y = newCords.Y * a_enemy.MoveSpeed;
                xSpeed = Math.Abs(a_enemy.Direction.X);
                ySpeed = Math.Abs(a_enemy.Direction.Y);

                a_enemy.ThisUnit.Bounds.X += (int)newCords.X;
                a_enemy.ThisUnit.Bounds.Y += (int)newCords.Y;

                if (xSpeed > ySpeed)
                {
                    if (a_enemy.Direction.X > 0f)
                    {
                        a_enemy.UnitState = View.AnimationSystem.MOVING_RIGHT;
                        a_enemy.WeaponState = a_enemy.UnitState;
                    }
                    else
                    {
                        a_enemy.UnitState = View.AnimationSystem.MOVING_LEFT;
                        a_enemy.WeaponState = a_enemy.UnitState;
                    }
                }
                else
                {
                    if (a_enemy.Direction.Y > 0f)
                    {
                        a_enemy.UnitState = View.AnimationSystem.MOVING_DOWN;
                        a_enemy.WeaponState = a_enemy.UnitState;
                    }
                    else
                    {
                        a_enemy.UnitState = View.AnimationSystem.MOVING_UP;
                        a_enemy.WeaponState = a_enemy.UnitState;
                    }
                }

            }
            else
            {
                if (xSpeed > ySpeed)
                {
                    if (a_enemy.Direction.X > 0f)
                    {
                        a_enemy.UnitState = View.AnimationSystem.FACING_RIGHT;
                        a_enemy.WeaponState = View.AnimationSystem.MOVING_RIGHT;
                    }
                    else
                    {
                        a_enemy.UnitState = View.AnimationSystem.FACING_LEFT;
                        a_enemy.WeaponState = View.AnimationSystem.MOVING_LEFT;
                    }
                }
                else
                {
                    if (a_enemy.Direction.Y > 0f)
                    {
                        a_enemy.UnitState = View.AnimationSystem.FACING_CAMERA;
                        a_enemy.WeaponState = View.AnimationSystem.MOVING_DOWN;
                    }
                    else
                    {
                        a_enemy.UnitState = View.AnimationSystem.FACING_AWAY;
                        a_enemy.WeaponState = View.AnimationSystem.MOVING_UP;
                    }
                }

                a_enemy.IsEvading = false;
            }

        }

        public void EnemySpawnList(Model.Player a_player)
        {
            if (m_spawnList.Count > 0)
            {
                for (int i = 0; i < m_spawnList.Count; i++)
                {
                    if (m_spawnList[i].WaitingToSpawn())
                    {
                        m_spawnList[i].SpawnTimer --;
                    }
                    else
                    {
                        //BUGGFIX FÖR LOOTRUTOR EFTER SPAWN
                        if (m_spawnList[i] == a_player.LootTarget)
                        {
                            a_player.LootTarget = null;
                        }

                        //Skapar den nya fienden här fär att spawnen ska bli rätt.
                        MapObject mapObj = m_spawnList[i].ThisUnit;
                        Rectangle spawnPos = new Rectangle(m_spawnList[i].SpawnPosition.X, m_spawnList[i].SpawnPosition.Y, m_spawnList[i].ThisUnit.Bounds.Width, m_spawnList[i].ThisUnit.Bounds.Height);
                        mapObj.Bounds = spawnPos;
                        Enemy enemy = new Enemy(mapObj, m_spawnList[i].Type, m_spawnList[i].UnitId, m_spawnList[i].EnemyZone);
                        m_enemies.Add(enemy);
                        m_spawnList.Remove(m_spawnList[i]);
                    }
                }
            }
        }
    }
}
