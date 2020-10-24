using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace View
{
    class UnitView
    {
        #region Variabler
        //Modell instanser
        private Model.Player m_player;
        private List<Model.Enemy> m_enemies;
        private List<Model.Enemy> m_deadEnemies;
        private List<Model.Friend> m_friends;
        private Model.QuestSystem m_questSystem;

        //Svärd fix
        private float m_swordTime;

        //Vy instanser
        private View.InputHandler m_inputHandler;
        private View.AnimationSystem m_animationSystem;
        private View.Conversation m_conversation;

        //Övriga privata variabler
        private SpriteBatch m_spriteBatch;
        private Camera m_camera;
        private Texture2D[] m_textures;

        //Konstanta textur id
        private const int RED_CIRCLE = 0;
        private const int GREEN_CIRCLE = 1;
        private const int INTERACT = 2;
        private const int INTERACT_Q = 3;
        private const int INTERACT_Q_COMPLETE = 4; 
        #endregion

        public UnitView(Model.GameModel a_gameModel, SpriteBatch a_spriteBatch, Camera a_camera, InputHandler a_inputHandler, AnimationSystem a_animationSystem, View.Conversation a_conversation)
        {
            this.m_player = a_gameModel.m_playerSystem.m_player;
            this.m_enemies = a_gameModel.m_enemySystem.m_enemies;
            this.m_deadEnemies = a_gameModel.m_enemySystem.m_spawnList;
            this.m_friends = a_gameModel.m_friendSystem.m_friends;
            this.m_questSystem = a_gameModel.m_questSystem;
            this.m_camera = a_camera;
            this.m_spriteBatch = a_spriteBatch;
            this.m_inputHandler = a_inputHandler;
            this.m_animationSystem = a_animationSystem;
            this.m_conversation = a_conversation;

        }

        //Laddar klass-specifikt innehåll
        internal void LoadContent(ContentManager a_content)
        {
            m_textures = new Texture2D[5] {  a_content.Load<Texture2D>("Textures/Graphic/red_circle"),
                                             a_content.Load<Texture2D>("Textures/Graphic/green_circle"),
                                             a_content.Load<Texture2D>("Textures/Graphic/interact"),
                                             a_content.Load<Texture2D>("Textures/Graphic/interactQ"),
                                             a_content.Load<Texture2D>("Textures/Graphic/interactQcomplete")};
        }
        
        //Huvudmetod för uppdatering och utritning av Units (spelare/NPCs) 
        internal void DrawAndUpdateUnits(float a_elapsedTime)
        {
            m_swordTime = a_elapsedTime;
            DrawEnemies(a_elapsedTime);
            DrawFriendlyNPC(a_elapsedTime);
            DrawPlayer(a_elapsedTime);
        }

        //Metod för utritning av spelare
        private  void DrawPlayer(float a_elapsedTime)
        {
            if (m_player.IsAlive())
            {
                DrawWeapon(a_elapsedTime, m_player.UnitState, m_player.WeaponState, m_player);
            }
            else
            {
                m_player.UnitState = Model.Unit.IS_DEAD;
            }            
            //Ritar spelare
            Vector2 playerPosition = m_camera.VisualizeCordinates(m_player.ThisUnit.Bounds.X, m_player.ThisUnit.Bounds.Y);
            m_animationSystem.UpdateAndDraw(a_elapsedTime, Color.White, playerPosition, m_player.UnitState, AnimationSystem.PLAYER);

            //Börjat leka med armor.
            if (m_player.HasHelm)
            {
                DrawArmor(a_elapsedTime, m_player, m_player.UnitState, AnimationSystem.ARMOR_HEAD);
            }
        }

        //Metod för utritning av vapen
        private void DrawWeapon(float a_elapsedTime, int a_unitAnimation, int a_weaponAnimation, Model.Unit a_unit)
        {
            //Ritar vapen
            if (a_unit.IsAttacking && !a_unit.IsCastingSpell)// && m_player.IsAlive())//Player is alive: buggfix, skall ej behövas
            {
                if (a_weaponAnimation > 0)
                {
                    #region Displacement

                    int displacementX = 0;
                    int displacementY = 0;

                    if (a_weaponAnimation == AnimationSystem.FACING_CAMERA ||
                        a_weaponAnimation == AnimationSystem.MOVING_DOWN)
                    {
                        displacementX = -11;
                        displacementY = 25;
                    
                        if (a_unit.GetType() == Model.GameModel.ENEMY_NPC)
                        {
                            displacementX = -5;
                        }
                    }
                    else if (a_weaponAnimation == AnimationSystem.MOVING_UP)
                    {
                        displacementX = +11;
                        displacementY = -10;
                    }
                    else if (a_weaponAnimation == AnimationSystem.MOVING_LEFT)
                    {
                        displacementX = -20;
                        displacementY = +5;

                        if (a_unit.GetType() == Model.GameModel.ENEMY_NPC)
                        {
                            displacementX = -10;
                            displacementY = +12;
                        }
                    }
                    else
                    {
                        displacementX = +20;
                        displacementY = +5;
                    
                        if (a_unit.GetType() == Model.GameModel.ENEMY_NPC)
                        {
                            displacementX = +22;
                            displacementY = +15;
                        }
                    }
                    #endregion

                    Vector2 swordPosition = m_camera.VisualizeCordinates(a_unit.ThisUnit.Bounds.X + displacementX, a_unit.ThisUnit.Bounds.Y + displacementY);
                    m_animationSystem.UpdateAndDraw(m_swordTime, Color.White, swordPosition, a_weaponAnimation, AnimationSystem.WEAPON_SWORD);
                }
            }
            m_swordTime = 0;
        }

        //Metod för utritning av armor
        private void DrawArmor(float a_elapsedTime, Model.Player a_player, int a_playerAnimation, int a_armorId)
        {
            Vector2 armorPlacement = Vector2.Zero;

            switch (a_armorId)
            {
                case AnimationSystem.ARMOR_HEAD:
                    armorPlacement = m_camera.VisualizeCordinates(a_player.ThisUnit.Bounds.X, a_player.ThisUnit.Bounds.Y);
                    break;
            }

            int armorAnimation;

            if (a_playerAnimation == AnimationSystem.MOVING_DOWN)
            {
                armorAnimation = AnimationSystem.FACING_CAMERA;
            }
            else if (a_playerAnimation == AnimationSystem.MOVING_UP)
            {
                armorAnimation = AnimationSystem.FACING_AWAY;
            }
            else if (a_playerAnimation == AnimationSystem.MOVING_LEFT)
            {
                armorAnimation = AnimationSystem.FACING_LEFT;
            }
            else if (a_playerAnimation == AnimationSystem.MOVING_RIGHT)
            {
                armorAnimation = AnimationSystem.FACING_RIGHT;
            }
            else if (a_playerAnimation == AnimationSystem.IS_CASTING_HEAL)
            {
                armorAnimation = AnimationSystem.FACING_CAMERA;
            }
            else
            {
                armorAnimation = a_playerAnimation;
            }

            m_animationSystem.UpdateAndDraw(a_elapsedTime, Color.White, armorPlacement, armorAnimation, AnimationSystem.ARMOR_HEAD);
        }

        //Metod för utritning av friendly NPCs
        private void DrawFriendlyNPC(float a_elapsedTime)
        {
            foreach (Model.Friend friend in m_friends)
            {
                //Kollar att npcn är med på skärmen
                if (friend.ThisUnit.Bounds.Intersects(m_camera.GetScreenRectangle))
                {
                    friend.CanAddToQuest = true;

                    //Om man klickar på FNPC sätts den till spelarens target
                    if (m_inputHandler.DidGetTargetedByLeftClick(m_camera.VisualizeRectangle(friend.ThisUnit.Bounds)) ||
                        m_inputHandler.DidGetTargetedByRightClick(m_camera.VisualizeRectangle(friend.ThisUnit.Bounds)))
                    {
                        m_player.Target = friend;
                    }

                    Vector2 interactPosition = m_camera.VisualizeCordinates(friend.ThisUnit.Bounds.X + 10, friend.ThisUnit.Bounds.Y - 24);
                    int bubble = -1;

                    //Ritar ut pratbubbla om NPCns ID ingår i XML filen för konversationer
                    if (m_conversation.m_dialogueList.Exists(Message => Message.id == friend.UnitId))
                    {
                        friend.CanInterract = true;
                        bubble = INTERACT;
                    }
                    else if ((m_questSystem.QuestList.Exists(Quest => m_questSystem.ActiveNpc == friend.UnitId && Quest.Id == m_questSystem.CurrentQuest.Id)))
                    {
                        friend.CanInterract = true;

                        if (m_questSystem.CurrentQuest.Status == Model.QuestSystem.END)
                            bubble = INTERACT_Q_COMPLETE;
                        else if (m_questSystem.CurrentQuest.Status == Model.QuestSystem.PRE)
                            bubble = INTERACT_Q;
                        else
                            bubble = INTERACT;
                    }
                    else
                    {
                        friend.CanInterract = false;
                    }

                    if (friend.CanInterract)
                    {
                        m_spriteBatch.Draw(m_textures[bubble], new Rectangle((int)interactPosition.X, (int)interactPosition.Y, 30, 30), Color.White);
                    }

                    //Ritar NPC
                    Vector2 npcPosition = m_camera.VisualizeCordinates(friend.ThisUnit.Bounds.X + 8, friend.ThisUnit.Bounds.Y + 8);
                    int animation = 0;

                    if (friend.Type == Model.Friend.OLD_MAN)
                        animation = AnimationSystem.NPC_OLD_MAN;
                    else
                    {
                        animation = AnimationSystem.CITY_GUARD;
                    }
                  
                    //Ritar target ringen.
                    DrawTargetCircle(m_player, friend);
                    m_animationSystem.UpdateAndDraw(a_elapsedTime, Color.White, npcPosition, friend.UnitState, animation);
                }
                else
                {
                    friend.CanAddToQuest = false;
                }
            }

        }

        //Metod för utritning av enemy NPCs
        private void DrawEnemies(float a_elapsedTime)
        {
            float mageTime = a_elapsedTime;
            float warriorTime = a_elapsedTime;
            float swordTime = a_elapsedTime;
            float goblinTime = a_elapsedTime;
            float bossATime = a_elapsedTime;

            m_inputHandler.MouseIsOverEnemy = false;

            //Riter ut döda fiender.
            foreach (Model.Enemy deadEnemy in m_deadEnemies)
            {
                Vector2 enemyPosition = m_camera.VisualizeCordinates(deadEnemy.ThisUnit.Bounds.X + 8, deadEnemy.ThisUnit.Bounds.Y + 8);

                if (m_inputHandler.MouseIsOver(new Rectangle((int)enemyPosition.X - 8, (int)enemyPosition.Y - 8, deadEnemy.ThisUnit.Bounds.Width, deadEnemy.ThisUnit.Bounds.Height))
                    && !m_inputHandler.MouseIsOverInterface)
                {
                    m_inputHandler.MouseIsOverLoot= true;
                }

                if (deadEnemy.Type == Model.Enemy.BOSS_A)
                {
                    m_animationSystem.UpdateAndDraw(bossATime, Color.White, enemyPosition, deadEnemy.UnitState, AnimationSystem.BOSS_A);
                }
                else if (deadEnemy.Type == Model.Enemy.CLASS_GOBLIN)
                {
                    m_animationSystem.UpdateAndDraw(goblinTime, Color.White, enemyPosition, deadEnemy.UnitState, AnimationSystem.ENEMY_GOBLIN);
                }
                else if (deadEnemy.Type == Model.Enemy.CLASS_MAGE)
                {
                    m_animationSystem.UpdateAndDraw(mageTime, Color.White, enemyPosition, deadEnemy.UnitState, AnimationSystem.ENEMY_MAGE);
                }
                else if (deadEnemy.Type == Model.Enemy.CLASS_WARRIOR)
                {
                    m_animationSystem.UpdateAndDraw(warriorTime, Color.White, enemyPosition, deadEnemy.UnitState, AnimationSystem.ENEMY_KNIGHT);
                }

                if (m_inputHandler.DidGetTargetedByLeftClick(new Rectangle((int)enemyPosition.X, (int)enemyPosition.Y, deadEnemy.ThisUnit.Bounds.Width, deadEnemy.ThisUnit.Bounds.Height)) & !m_inputHandler.MouseIsOverInterface)
                {
                    //Kontrollerar så att man är tillräckligt nära för att loota en fiende.
                    if (m_player.ThisUnit.Bounds.Intersects(deadEnemy.ThisUnit.Bounds))
                    {
                        m_player.LootTarget = deadEnemy;
                    }
                }
            }

           // m_enemies.OrderByDescending(Enemy => Enemy.ThisUnit.Bounds.Y);
            //Försök på att måla ut fiender och tillbehör.
            foreach (Model.Enemy enemy in m_enemies)
            {
                //LAGT DEN HÄR UTE FÖR ATT HA DÖDS Animation också.
                //TODO: Klass för visualisering av position
                Vector2 enemyPosition = m_camera.VisualizeCordinates(enemy.ThisUnit.Bounds.X + 8, enemy.ThisUnit.Bounds.Y + 8);
               
                //Testar om musen är över en fiende.
                if(m_inputHandler.MouseIsOver(new Rectangle((int)enemyPosition.X - 8, (int)enemyPosition.Y - 8, enemy.ThisUnit.Bounds.Width, enemy.ThisUnit.Bounds.Height))
                    && !m_inputHandler.MouseIsOverInterface)
                {
                    m_inputHandler.MouseIsOverEnemy = true;
                }

                if(m_player.Target == enemy)
                {
                    //Ritar target ring
                    DrawTargetCircle(m_player, enemy);
                }

                if (enemy.IsAttacking && (enemy.Type == Model.Enemy.CLASS_WARRIOR)) //|| enemy.Type == Model.Enemy.BOSS_A))
                {
                    DrawWeapon(swordTime, enemy.UnitState, enemy.WeaponState, enemy);
                    //BUGGFIXXX.
                    swordTime = 0;
                }
                if (enemy.ThisUnit.Bounds.Intersects(m_camera.GetScreenRectangle))
                {
                    //Om fienden syns på skärmen är den aktiv
                    enemy.IsActive = true;

                    //Testar om spelaren har satt target på en fiende.
                    if (m_inputHandler.DidGetTargetedByLeftClick(m_camera.VisualizeRectangle(enemy.ThisUnit.Bounds)) && !m_inputHandler.MouseIsOverInterface)
                    {
                        //Sätter en fiende i form av mapobjekt som spelarens target.
                        m_player.Target = enemy;
                        m_player.IsAttacking = true;
                    }

                    //Är inte fienden levande och aktiv ritas han ut.
                    if (enemy.IsAlive())
                    {
                        //Ritar Enemy animation
                        if (enemy.Type == Model.Enemy.CLASS_WARRIOR)
                        {
                            m_animationSystem.UpdateAndDraw(warriorTime, Color.White, enemyPosition, enemy.UnitState, AnimationSystem.ENEMY_KNIGHT);

                            if (enemy.IsAttacking || enemy.IsEvading)
                            {
                                warriorTime = 0;
                            }
                        }

                        else if (enemy.Type == Model.Enemy.CLASS_MAGE)
                        {
                            m_animationSystem.UpdateAndDraw(mageTime, Color.White, enemyPosition, enemy.UnitState, AnimationSystem.ENEMY_MAGE);

                            if (enemy.IsAttacking || enemy.IsEvading)
                            {
                                mageTime = 0;
                            }
                        }

                        else if (enemy.Type == Model.Enemy.CLASS_GOBLIN)
                        {
                            m_animationSystem.UpdateAndDraw(goblinTime, Color.White, enemyPosition, enemy.UnitState, AnimationSystem.ENEMY_GOBLIN);

                            if (enemy.IsAttacking || enemy.IsEvading)
                            {
                                goblinTime = 0;
                            }
                        }

                        else if (enemy.Type == Model.Enemy.BOSS_A)
                        {
                            m_animationSystem.UpdateAndDraw(bossATime, Color.White, enemyPosition, enemy.UnitState, AnimationSystem.BOSS_A);

                            if (enemy.IsAttacking || enemy.IsEvading)
                            {
                                bossATime = 0;
                            }
                        }
                    }
                }
                else
                {
                    enemy.IsActive = false;
                }

            }
        }

        //Metod för utritning av target cirkel kring NPCs
        private void DrawTargetCircle(Model.Player a_player, Model.Unit a_unit)
        {
            Vector2 position = m_camera.VisualizeCordinates(a_unit.ThisUnit.Bounds.X + 1, a_unit.ThisUnit.Bounds.Y + 28);

            if (a_player.Target == a_unit)
            {
                //Enemy NPC
                if (a_unit.GetType() == Model.GameModel.ENEMY_NPC)
                {
                    m_spriteBatch.Draw(m_textures[RED_CIRCLE], position, Color.White);
                }
                //Friendly NPC
                if (a_unit.GetType() == Model.GameModel.FRIENDLY_NPC)
                {
                    m_spriteBatch.Draw(m_textures[GREEN_CIRCLE], position, Color.White);
                }
            }
        }
    }
}
