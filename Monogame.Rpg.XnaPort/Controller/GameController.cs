using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Controller
{
    class GameController
    {
        //Variabler
        private Model.GameModel m_gameModel;
        public View.GameView m_gameView;

        //Konstruktor
        public GameController(GraphicsDevice a_graphicsDevice, SpriteBatch a_spriteBatch, Model.GameModel a_gameModel, View.AnimationSystem a_animationSystem, View.InputHandler a_inputHandler, View.SoundHandler a_soundHandler)
        {
            m_gameModel = a_gameModel;
            m_gameView = new View.GameView(a_graphicsDevice, a_spriteBatch, m_gameModel, a_animationSystem, a_inputHandler, a_soundHandler);
        }

        internal void LoadContent(ContentManager a_content)
        {      
            m_gameView.LoadContent(a_content);
        }

        internal void UpdateSimulation(float a_elapsedTime)
        {
            Model.Player player = m_gameModel.m_playerSystem.m_player;
            Vector2 moveTo = m_gameView.GetMapMousePosition();

            #region MouseMoveInteraktion


            float xSpeed = 0;
            float ySpeed = 0;

            Rectangle mouseRect = new Rectangle((int)moveTo.X, (int)moveTo.Y, 1, 1);
            if (!player.IsCastingSpell && player.IsAlive())
            {
                if (moveTo != Vector2.Zero)    //Om man håller inne (Right-mouseclick)
                {
                    player.MoveToPosition = moveTo;
                    player.Direction = new Vector2(moveTo.X - player.ThisUnit.Bounds.Center.X, moveTo.Y - player.ThisUnit.Bounds.Center.Y);
                }
                else if (player.MoveToPosition != Vector2.Zero) //Click 2 move
                {
                    player.Direction = new Vector2(player.MoveToPosition.X - player.ThisUnit.Bounds.Center.X, player.MoveToPosition.Y - player.ThisUnit.Bounds.Center.Y);
                }

                Vector2 newCords = new Vector2();
                Vector2 facing = new Vector2();

                if (player.Direction != Vector2.Zero && !m_gameModel.m_playerSystem.ArrivedToPosition(player.ThisUnit.Bounds, player.MoveToPosition, 5)) //Om jag rör på mig
                {
                    newCords = player.Direction;
                    newCords.Normalize();
                    newCords.X = newCords.X * 4;
                    newCords.Y = newCords.Y * 4;

                    player.ThisUnit.Bounds.X += (int)newCords.X;
                    player.ThisUnit.Bounds.Y += (int)newCords.Y;
                }
                if (player.Target != null && player.Target.ThisUnit.Bounds.Intersects(player.MaxRangeArea))  // om jag har targetat en unit så sätter jag vilket håll han ska titta åt
                {
                    facing.X = player.Target.ThisUnit.Bounds.Center.X - player.ThisUnit.Bounds.Center.X;
                    facing.Y = player.Target.ThisUnit.Bounds.Center.Y - player.ThisUnit.Bounds.Center.Y;
                }
                else
                {
                    facing = newCords;
                }

                xSpeed = Math.Abs(facing.X);
                ySpeed = Math.Abs(facing.Y);

                if (facing == new Vector2())
                {
                    player.UnitState = View.AnimationSystem.FACING_CAMERA;
                    player.WeaponState = View.AnimationSystem.MOVING_DOWN;
                }
                if (player.Target != null && player.Target.ThisUnit.Bounds.Intersects(player.MaxRangeArea))
                {
                    if (xSpeed > ySpeed)
                    {
                        if (facing.X > 0f)
                        {
                            player.UnitState = View.AnimationSystem.FACING_RIGHT;
                            player.WeaponState = View.AnimationSystem.MOVING_RIGHT;
                        }
                        else
                        {
                            player.UnitState = View.AnimationSystem.FACING_LEFT;
                            player.WeaponState = View.AnimationSystem.MOVING_LEFT;
                        }
                    }
                    else
                    {
                        if (facing.Y > 0f)
                        {
                            player.UnitState = View.AnimationSystem.FACING_CAMERA;
                            player.WeaponState = View.AnimationSystem.MOVING_DOWN;
                        }
                        else
                        {
                            player.UnitState = View.AnimationSystem.FACING_AWAY;
                            player.WeaponState = View.AnimationSystem.MOVING_UP;
                        }
                    }
                }
                if (newCords != new Vector2() && !m_gameModel.m_playerSystem.ArrivedToPosition(player.ThisUnit.Bounds, player.MoveToPosition, 5))
                {
                    xSpeed = Math.Abs(newCords.X);
                    ySpeed = Math.Abs(newCords.Y);
                    if (xSpeed > ySpeed)
                    {
                        if (facing.X > 0f)
                        {
                            player.UnitState = View.AnimationSystem.MOVING_RIGHT;
                            player.WeaponState = player.UnitState;
                        }
                        else
                        {
                            player.UnitState = View.AnimationSystem.MOVING_LEFT;
                            player.WeaponState = player.UnitState;
                        }
                    }
                    else
                    {
                        if (facing.Y > 0f)
                        {
                            player.UnitState = View.AnimationSystem.MOVING_DOWN;
                            player.WeaponState = player.UnitState;
                        }
                        else
                        {
                            player.UnitState = View.AnimationSystem.MOVING_UP;
                            player.WeaponState = player.UnitState;
                        }
                    }
                }
            }
            else if(player.IsCastingSpell)
            {
                player.UnitState = View.AnimationSystem.IS_CASTING_HEAL;
            } 
            else
            {
                player.UnitState = View.AnimationSystem.FACING_CAMERA;
            }
            #endregion

            #region ActionBarInteraktion

            if (m_gameView.DidActivateActionBar(View.InputHandler.ACTION_BAR_TWO))
            {
                m_gameModel.m_playerSystem.m_spellSystem.AddSpell(Model.SpellSystem.INSTANT_HEAL, player);
            }

            if (m_gameView.DidActivateActionBar(View.InputHandler.ACTION_BAR_ONE) && m_gameModel.m_playerSystem.m_player.Target != null)
            {
                if (m_gameModel.m_playerSystem.m_player.Target.GetType() == Model.GameModel.ENEMY_NPC)
                    m_gameModel.m_playerSystem.m_spellSystem.AddSpell(Model.SpellSystem.SMITE, player);
            }

            if (m_gameView.DidActivateActionBar(View.InputHandler.BACKPACK))
            {
                if (!player.BackPack.IsOpen)
                {
                    player.BackPack.IsOpen = true;
                }
                else
                {
                    player.BackPack.IsOpen = false;
                }
            }

            if (m_gameView.DidActivateActionBar(View.InputHandler.QUEST_LOG))
            {
                if (!m_gameModel.m_questSystem.IsWatchingQuestLog)
                {
                    m_gameModel.m_questSystem.IsWatchingQuestLog = true;
                }
                else
                {
                    m_gameModel.m_questSystem.IsWatchingQuestLog = false;
                }
            }

            //Öppna stäng worldmap.
            if (m_gameView.DidActivateActionBar(View.InputHandler.WORLD_MAP))
            {
                if (!player.IsLookingAtMap)
                {
                    player.IsLookingAtMap = true;
                }
                else
                {
                    player.IsLookingAtMap = false;
                }
            }

            //Öppna stäng character panel.
            if (m_gameView.DidActivateActionBar(View.InputHandler.CHARACTER_PANEL))
            {
                if (!player.CharPanel.IsOpen)
                {
                    player.CharPanel.IsOpen = true;
                }
                else
                {
                    player.CharPanel.IsOpen = false;
                }
            }
            #endregion

            if (player.Target != null)
            {
                //Gör att spelaren slutar att attackera.
                if (m_gameView.UnTarget())
                {
                    player.IsAttacking = false;
                    player.Target = null;
                }
            }

            #region FUSK KOD
            ////bli typ odödlig.
            //if (m_gameView.DidPressAndReleaseKey('R'))
            //{
            //    if (m_gameModel.m_playerSystem.m_player.Armor < 90)
            //    {
            //        m_gameModel.m_playerSystem.m_player.Armor = 100;
            //    }
            //}
            ////FULL MANA OCH LIV.
            //if (m_gameView.DidPressAndReleaseKey('F'))
            //{
            //    m_gameModel.m_playerSystem.m_player.CurrentHp = m_gameModel.m_playerSystem.m_player.TotalHp;
            //    m_gameModel.m_playerSystem.m_player.CurrentMana = m_gameModel.m_playerSystem.m_player.TotalMana;
            //}
            ////stänga på och av hinder.
            //if (m_gameView.DidPressAndReleaseKey('T'))
            //{
            //    if (m_gameModel.m_questSystem.CurrentQuestIndex != 2)
            //    {
            //        m_gameModel.m_questSystem.CurrentQuestIndex = 2;
            //        m_gameModel.m_questSystem.CurrentQuest.Status = Model.QuestSystem.END;
            //    }
            //}

            //if (m_gameView.DidPressAndReleaseKey('G'))
            //{
            //    foreach (Model.Enemy e in m_gameModel.m_enemySystem.m_enemies)
            //    {
            //        if (e.Type == Model.Enemy.BOSS_A)
            //        {
            //            m_gameModel.m_playerSystem.m_player.ThisUnit.Bounds.Location = e.ThisUnit.Bounds.Location;
            //        }
            //    }
            //}

            //if (m_gameView.DidPressAndReleaseKey('H'))
            //{
            //    foreach (Model.Friend f in m_gameModel.m_friendSystem.m_friends)
            //    {
            //        if (f.Type == Model.Friend.CITY_GUARD)
            //        {
            //            m_gameModel.m_playerSystem.m_player.ThisUnit.Bounds.Location = f.ThisUnit.Bounds.Location;
            //        }
            //    }
            //}

            //#region GammalKeyboardMove
            ////Flyttar spelaren samt bestämmer animation
            //if (m_gameView.DidPressKey(View.InputHandler.DOWN) && player.CanMoveDown)
            //{
            //    player.ThisUnit.Bounds.Y += Convert.ToInt32(a_elapsedTime * 200);
            //    player.UnitState = View.AnimationSystem.MOVING_DOWN;
            //}
            //if (m_gameView.DidPressKey(View.InputHandler.UP) && player.CanMoveUp)
            //{
            //    player.ThisUnit.Bounds.Y -= Convert.ToInt32(a_elapsedTime * 200);
            //    player.UnitState = View.AnimationSystem.MOVING_UP;
            //}
            //if (m_gameView.DidPressKey(View.InputHandler.RIGHT) && player.CanMoveRight)
            //{
            //    player.ThisUnit.Bounds.X += Convert.ToInt32(a_elapsedTime * 200);
            //    player.UnitState = View.AnimationSystem.MOVING_RIGHT;

            //}
            //if (m_gameView.DidPressKey(View.InputHandler.LEFT) && player.CanMoveLeft)
            //{
            //    player.ThisUnit.Bounds.X -= Convert.ToInt32(a_elapsedTime * 200);
            //    player.UnitState = View.AnimationSystem.MOVING_LEFT;
            //}
            //#endregion
            #endregion

            //Uppdaterar spelmotor
            m_gameModel.UpdateSimulation(a_elapsedTime);
        }

        internal void Draw(float a_elapsedTime)
        {
            //Uppdaterar och ritar grafik
            m_gameView.DrawAndUpdate(a_elapsedTime);
        }
    }
}
