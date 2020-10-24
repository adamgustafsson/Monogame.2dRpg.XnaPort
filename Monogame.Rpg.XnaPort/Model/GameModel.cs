using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Model
{
    class GameModel
    {
        public Map m_currentMap;
        public Level m_level;
        public PlayerSystem m_playerSystem;
        public EnemySystem m_enemySystem;
        public FriendSystem m_friendSystem;
        public ItemSystem m_itemSystem;
        public QuestSystem m_questSystem;

        //Typ-referenser till modellklasser
        public static Type ENEMY_NPC = typeof(Model.Enemy);
        public static Type FRIENDLY_NPC = typeof(Model.Friend);      
        public static Type ARMOR = typeof(Model.Armor);
        public static Type PLAYER = typeof(Model.Player);
        public static Type QUEST_ITEM = typeof(Model.QuestItem);


        //Konstruktor
        public GameModel(ContentManager a_content)
        {
            m_level = new Level(a_content);
            m_currentMap = m_level.CurrentMap();
            m_playerSystem = new PlayerSystem(m_level);
            m_enemySystem = new EnemySystem(m_level.EnemyNPCLayer, m_level, m_currentMap);
            m_friendSystem = new FriendSystem(m_level.FriendlyNPCLayer);
            m_itemSystem = new ItemSystem(m_level.ItemLayer);
            m_questSystem = new QuestSystem(a_content);
        }

        internal void UpdateSimulation(float a_elapsedTime)
        {
            m_itemSystem.UpdateItemSystem(a_elapsedTime, m_playerSystem.m_player);

            m_playerSystem.Update(a_elapsedTime, m_enemySystem.m_enemies);

            m_enemySystem.Update(m_playerSystem.m_player, a_elapsedTime);

            //m_friendSystem.Update(m_playerSystem.m_player);

            m_questSystem.UpdateActiveQuest(m_enemySystem.m_spawnList, m_friendSystem.m_friends, m_playerSystem.m_player.BackPack.BackpackItems, m_level);

            if (PlayerEnters(m_playerSystem.m_player))
            {
                m_level.foregroundVisible = false;
            }
            else
            {
                m_level.foregroundVisible = true;
            }

            if (PlayerAndTileCollide())
            {
                m_playerSystem.m_player.ThisUnit.Bounds.X = m_playerSystem.m_player.LastPosition.X;
                m_playerSystem.m_player.ThisUnit.Bounds.Y = m_playerSystem.m_player.LastPosition.Y;
                m_playerSystem.m_player.MoveToPosition = new Vector2(m_playerSystem.m_player.ThisUnit.Bounds.Center.X, m_playerSystem.m_player.ThisUnit.Bounds.Center.Y);
                m_playerSystem.m_player.Direction = new Vector2(m_playerSystem.m_player.ThisUnit.Bounds.Center.X, m_playerSystem.m_player.ThisUnit.Bounds.Center.Y);

            }
            else
            {
                if (!(m_playerSystem.m_player.ThisUnit.Bounds.X == m_playerSystem.m_player.LastPosition.X
                    && m_playerSystem.m_player.ThisUnit.Bounds.Y == m_playerSystem.m_player.LastPosition.Y))
                {
                    m_playerSystem.m_player.CanMoveDown = true;
                    m_playerSystem.m_player.CanMoveLeft = true;
                    m_playerSystem.m_player.CanMoveRight = true;
                    m_playerSystem.m_player.CanMoveUp = true;
                }

                m_playerSystem.m_player.LastPosition = m_playerSystem.m_player.ThisUnit.Bounds.Location;
            }
        }

        public bool PlayerAndTileCollide()
        {
            foreach (var obj in m_currentMap.GetObjectsInRegion(m_level.IndexCollision, m_playerSystem.m_player.CollisionArea))
            {
                if(obj.Name != "Open")
                {
                    if (obj.Bounds.Intersects(m_playerSystem.m_player.PlayerArea))
                    {
                        if (m_playerSystem.m_player.ThisUnit.Bounds.X > m_playerSystem.m_player.LastPosition.X)
                        {
                            m_playerSystem.m_player.CanMoveRight = false;
                            m_playerSystem.m_player.CanMoveLeft = true;
                            m_playerSystem.m_player.CanMoveDown = true;
                            m_playerSystem.m_player.CanMoveUp = true;
                        }
                        else if (m_playerSystem.m_player.ThisUnit.Bounds.X < m_playerSystem.m_player.LastPosition.X)
                        {
                            m_playerSystem.m_player.CanMoveRight = true;
                            m_playerSystem.m_player.CanMoveLeft = false;
                            m_playerSystem.m_player.CanMoveDown = true;
                            m_playerSystem.m_player.CanMoveUp = true;
                        }
                        else if (m_playerSystem.m_player.ThisUnit.Bounds.Y > m_playerSystem.m_player.LastPosition.Y)
                        {
                            m_playerSystem.m_player.CanMoveRight = true;
                            m_playerSystem.m_player.CanMoveLeft = true;
                            m_playerSystem.m_player.CanMoveDown = false;
                            m_playerSystem.m_player.CanMoveUp = true;
                        }
                        else if (m_playerSystem.m_player.ThisUnit.Bounds.Y < m_playerSystem.m_player.LastPosition.Y)
                        {
                            m_playerSystem.m_player.CanMoveRight = true;
                            m_playerSystem.m_player.CanMoveLeft = true;
                            m_playerSystem.m_player.CanMoveUp = false;
                            m_playerSystem.m_player.CanMoveDown = true;
                        }
                        return true;
                    }
                }
            }

            return false;
        }

        public bool PlayerEnters(Player a_player)
        {
            foreach (var obj in m_currentMap.GetObjectsInRegion(m_level.IndexInteraction, m_playerSystem.m_player.CollisionArea))
            {
                if (obj.Bounds.Intersects(a_player.PlayerArea))
                {
                    return true;
                }
            }
            return false;
        }

        internal bool GameIsOver()
        {
            return m_questSystem.AllQuestsCompleted;
        }
    }
}
