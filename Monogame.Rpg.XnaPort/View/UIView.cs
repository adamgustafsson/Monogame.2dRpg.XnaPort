using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using FuncWorks.XNA.XTiled;
using System.Diagnostics;
using LudosProduction.RpgLib;

namespace View
{
    /// <summary>
    /// Klass för utritning av samtligt user-interface
    /// </summary>
    class UIView
    {
        #region Variabler
        public Vector2[] m_keyPositions;
        private Texture2D[] m_textures;
        private Texture2D[] m_iconTextures;
        private Texture2D[] m_worldMapTextures;
        private SpriteBatch m_spriteBatch;
        private SpriteFont m_spriteFontBig;
        private SpriteFont m_spriteFontSegoeSmall;
        private SpriteFont m_spriteFontSegoe;
        private SpriteFont m_healFont;
        private Stopwatch m_rightClickWatch;
        private float m_dmgCounter;
        private float m_healingCounter;
        private float m_smiteCounter;
        private float m_randomNr = 0;
        private Vector2 m_moveToPos;
        private float m_moveToColor;

        private View.Camera m_camera;
        private View.InputHandler m_inputHandler;
        private Model.Player m_player;
        private Model.QuestSystem m_questSystem;
        private List<Model.Item> m_worldItems;
        private List<Model.Spell> m_activeSpells;
        private Map m_currentMap;
        Conversation m_conversation;

        #endregion

        #region TexturID
        private const int RED_HP = 0;
        private const int GREEN_HP = 1;
        private const int AVATAR_PLAYER = 2;
        private const int AVATAR_KNIGHT = 3;
        private const int AVATAR_OLD_MAN = 4;
        private const int BACKPACK = 5;
        private const int HEADARMOR = 6;
        private const int ACTION_BAR = 7;
        private const int CHARACTER_PANEL = 8;
        private const int MANA = 9;
        private const int BAR_BACKGROUND = 10;
        private const int AVATAR_GOBLIN = 11;
        private const int LOOT_BOX = 12;
        private const int CURSOR_NORMAL = 13;
        private const int CURSOR_HOVER_ENEMY = 14;
        private const int CURSOR_SELECT = 15;
        private const int AVATAR_FIREMAGE = 16;
        private const int BOSS_HEAD = 17;
        private const int ITEM_STATS_BACKGROUND = 18;
        private const int ITEM_STATS_BG_SMALL= 19;
        private const int MOVE_TO_CROSS = 20;
        private const int CHAR_PANEL_BG = 21;
        private const int AVATAR_BRYNOLF = 22;
        private const int QLOG = 23;
        private const int AVATAR_CITY_GUARD = 24;
        #endregion

        #region TexturID-Icons
        private const int ICON_INSTANT_HEAL = 0;
        private const int ICON_INSTANT_HEAL_CD = 1;
        public const int ICON_BAG = 2;
        public const int ICON_BAG_OPEN = 3;
        private const int ICON_SMITE = 4;
        private const int ICON_SMITE_CD = 5;
        private const int ICON_WORLD_MAP = 6;
        private const int ICON_WORLD_MAP_SELECTED = 7;
        private const int ICON_CHAR_PANEL = 8;
        private const int ICON_CHAR_PANEL_SELECTED = 9;
        private const int ICON_QUEST_LOG= 10;
        private const int ICON_QUEST_LOG_SELECTED = 11; 
        #endregion

        #region TexturIDWorldMap
        private const int PLAYER_POSITION = 0;
        private const int WORLD_MAP = 1;
        private const int WORLD_MAP_BG = 2;
        #endregion

        public UIView(SpriteBatch a_spriteBatch, Camera a_camera, InputHandler a_inputHandler, Model.GameModel a_gameModel, Conversation a_conversation)
        {
            this.m_spriteBatch = a_spriteBatch;
            this.m_keyPositions = new Vector2[7];
            this.m_camera = a_camera;

            this.m_inputHandler = a_inputHandler;
            this.m_activeSpells = a_gameModel.m_playerSystem.m_spellSystem.ActiveSpells;
            this.m_player = a_gameModel.m_playerSystem.m_player;
            this.m_questSystem = a_gameModel.m_questSystem;
            this.m_worldItems = a_gameModel.m_itemSystem.m_items;
            this.m_currentMap = a_gameModel.m_level.CurrentMap();

            this.m_rightClickWatch = new Stopwatch();
            this.m_conversation = a_conversation;
        }

        internal void LoadContent(ContentManager a_content)
        {
            #region TexturesArray
            m_textures = new Texture2D[25] {    a_content.Load<Texture2D>("Textures/Interface/red_hp"),
                                                a_content.Load<Texture2D>("Textures/Interface/green_hp"),
                                                a_content.Load<Texture2D>("Textures/Interface/Avatars/test"),
                                                a_content.Load<Texture2D>("Textures/Interface/Avatars/avatar_knight"),
                                                a_content.Load<Texture2D>("Textures/Interface/Avatars/avatar_old_man"),
                                                a_content.Load<Texture2D>("Textures/Interface/backpack"),
                                                a_content.Load<Texture2D>("Textures/Interface/headArmor2"),
                                                a_content.Load<Texture2D>("Textures/Interface/spellBar2"),
                                                a_content.Load<Texture2D>("Textures/Interface/characterpanel"),
                                                a_content.Load<Texture2D>("Textures/Interface/mana"),
                                                a_content.Load<Texture2D>("Textures/Interface/barbackground"),
                                                a_content.Load<Texture2D>("Textures/Interface/Avatars/goblinAvatar"),
                                                a_content.Load<Texture2D>("Textures/Interface/lootbox"),
                                                a_content.Load<Texture2D>("Textures/Interface/cursor"),
                                                a_content.Load<Texture2D>("Textures/Interface/cursorAttack"),
                                                a_content.Load<Texture2D>("Textures/Interface/cursorSelect"),
                                                a_content.Load<Texture2D>("Textures/Interface/Avatars/fireMageAvatar"),
                                                a_content.Load<Texture2D>("Textures/Items/QuestItems/bossHead"),
                                                a_content.Load<Texture2D>("Textures/Interface/itemStatsBackG"),
                                                a_content.Load<Texture2D>("Textures/Interface/itemStatsBGsmall"),
                                                a_content.Load<Texture2D>("Textures/Interface/moveToCross"),
                                                a_content.Load<Texture2D>("Textures/Interface/charPanel"),
                                                a_content.Load<Texture2D>("Textures/Interface/Avatars/brynolf"),
                                                a_content.Load<Texture2D>("Textures/Interface/qlog"),
                                                a_content.Load<Texture2D>("Textures/Interface/Avatars/cityGuardavatar")}; 
            #endregion

            #region Icon Textures Array
            m_iconTextures = new Texture2D[12] { a_content.Load<Texture2D>("Textures/Interface/Icons/instantHeal"),
                                                a_content.Load<Texture2D>("Textures/Interface/Icons/instantHealCD"),
                                                a_content.Load<Texture2D>("Textures/Interface/Icons/bagIcon"),
                                                a_content.Load<Texture2D>("Textures/Interface/Icons/bagIconOpen"),
                                                a_content.Load<Texture2D>("Textures/Interface/Icons/smite"),
                                                a_content.Load<Texture2D>("Textures/Interface/Icons/smiteCD"),
                                                a_content.Load<Texture2D>("Textures/Interface/Icons/worldMap"),
                                                a_content.Load<Texture2D>("Textures/Interface/Icons/worldMapSelected"),
                                                a_content.Load<Texture2D>("Textures/Interface/Icons/charPanel2"),
                                                a_content.Load<Texture2D>("Textures/Interface/Icons/charPanelSelected2"),
                                                a_content.Load<Texture2D>("Textures/Interface/Icons/qLog"),
                                                a_content.Load<Texture2D>("Textures/Interface/Icons/qLogSelected")}; 
            #endregion

            #region WorldMap Textures Array
            m_worldMapTextures = new Texture2D[3] { a_content.Load<Texture2D>("WorldMaps/playerPos"), 
                                                    a_content.Load<Texture2D>("WorldMaps/theworldFG"),
                                                     a_content.Load<Texture2D>("WorldMaps/theworldBG")}; 
            #endregion

            //Fonts
            m_spriteFontBig = a_content.Load<SpriteFont>(@"Fonts\SegoeBig");
            m_spriteFontSegoe = a_content.Load<SpriteFont>(@"Fonts\Segoe");
            m_spriteFontSegoeSmall = a_content.Load<SpriteFont>(@"Fonts\SegoeSmall");
            m_healFont = a_content.Load<SpriteFont>(@"Fonts\HealFont");
        }

        //Huvud metod för uppdatering och utritning av user interface
        internal void DrawAndUpdate(float a_elapsedTime)
        {
            m_inputHandler.MouseIsOverInterface = false;

            DrawTargetAvatars();

            DrawActionBar();

            DrawBackpack();

            DrawDamageAndHealing(a_elapsedTime);

            DrawCharacterPanel();

            DrawWorldMap();

            if (m_questSystem.IsWatchingQuestLog)
            {
                DrawQuestLog();
            }
            if(m_player.IsCastingSpell)
            {
                DrawCastBar();
            }
            if (m_player.LootTarget != null)
            {
                DrawLootBox(m_player.LootTarget);
            }

            //Kollar om musen befinner sig över actionbarsen
            foreach (Vector2 actionBarPos in m_keyPositions)
            {
                if (m_inputHandler.MouseIsOver(new Rectangle((int)actionBarPos.X, (int)actionBarPos.Y, 48, 48)))
                    m_inputHandler.MouseIsOverInterface = true;
            }

            DrawMoveToCross();

            DrawMouse();
        }

        //Metod för utritning av Move-to krysset
        private void DrawMoveToCross()
        {
            if (m_inputHandler.DidRightClick() && !m_inputHandler.MouseIsOverInterface)
            {
                m_rightClickWatch.Reset();
                m_rightClickWatch.Start();
                m_moveToPos = m_player.MoveToPosition;
                m_moveToColor = 1f;
            }
            if (m_rightClickWatch.IsRunning && m_rightClickWatch.ElapsedMilliseconds < 1000)
            {
                m_moveToColor = m_moveToColor - 0.015f;
                Color color = new Color(m_moveToColor, m_moveToColor, m_moveToColor, m_moveToColor);
                Vector2 pos = m_camera.VisualizeCordinates((int)m_moveToPos.X - 16, (int)m_moveToPos.Y);
                m_spriteBatch.Draw(m_textures[MOVE_TO_CROSS], pos, color);
            }

            else
            {
                m_rightClickWatch.Stop();
                m_rightClickWatch.Reset();
            }
        }

        //Metod för utritning av muspekaren
        private void DrawMouse()
        {
            Vector2 mousePosition = new Vector2(m_inputHandler.GetMouseState().X, m_inputHandler.GetMouseState().Y);
            Color mouseColor = Color.White;

            if (m_inputHandler.MouseIsOverLoot && !m_inputHandler.MouseIsOverEnemy)
                mouseColor = Color.LightGreen;

            if (m_inputHandler.MouseIsOverEnemy)
            {
                m_spriteBatch.Draw(m_textures[CURSOR_HOVER_ENEMY], mousePosition, mouseColor);
            }
            else if (m_inputHandler.LeftButtonIsDown())
            {
                m_spriteBatch.Draw(m_textures[CURSOR_SELECT], mousePosition, mouseColor);
            }
            else
            {
                m_spriteBatch.Draw(m_textures[CURSOR_NORMAL], mousePosition, mouseColor);
            }

        }

        //Metod för utritning av QuestLog
        private void DrawQuestLog()
        {
            List<Objective> progress = m_questSystem.ObjectiveList;
            List<Objective> quest = m_questSystem.CurrentQuest.Objectives;
            Vector2 position = new Vector2(405, 150);
            Rectangle textRect = m_camera.VisualizeRectangle(new Rectangle((int)position.X + 8, (int)position.Y + 53, 225, 350));
            Rectangle closeCross = GetCloseButton(position.X, position.Y, QLOG);

            m_spriteBatch.Draw(m_textures[QLOG], position, Color.White);
            
            if (m_questSystem.CurrentQuest.Status != Model.QuestSystem.PRE)
            {
                m_spriteBatch.DrawString(m_spriteFontSegoeSmall, m_conversation.GetLogMessage(textRect), m_camera.LogicalizeCordinates(textRect.X, textRect.Y), Color.White);

                int changeRow = 150;
                for (int i = 0; i < progress.Count; i++)
                {
                    m_spriteBatch.DrawString(m_spriteFontSegoeSmall, progress[i].Amount + "/" + quest[i].Amount + " - " + quest[i].Name, m_camera.LogicalizeCordinates(textRect.X, textRect.Y + changeRow), Color.White);
                    changeRow += 18;
                }
            }

            if (m_inputHandler.MouseIsOver(new Rectangle((int)position.X, (int)position.Y, m_textures[QLOG].Bounds.Width, m_textures[QLOG].Bounds.Height)))
                m_inputHandler.MouseIsOverInterface = true;
            if (m_inputHandler.DidGetTargetedByLeftClick(closeCross))
                m_questSystem.IsWatchingQuestLog = false;
        }

        //Metod för utritning av loot ruta
        private void DrawLootBox(Model.Unit a_unit)
        {
            //displacement för texturen på lootboxen.
            int displacementX = 10;
            int displacementY = 20;

            //Används även i backpack. ändra till global.
            Vector2 unitPos = m_camera.VisualizeCordinates(a_unit.ThisUnit.Bounds.X, a_unit.ThisUnit.Bounds.Y);
            Rectangle itemRect = new Rectangle(0, 0, 32, 32);
            Rectangle lootBoxOuter = new Rectangle((int)unitPos.X, (int)unitPos.Y + 100,  m_textures[LOOT_BOX].Width, m_textures[LOOT_BOX].Height);
            Rectangle lootBoxInner = new Rectangle(lootBoxOuter.X + displacementX, lootBoxOuter.Y + displacementY, m_textures[LOOT_BOX].Width - displacementX, m_textures[LOOT_BOX].Height - displacementY);
            Rectangle closeCross = GetCloseButton(lootBoxOuter.X, lootBoxOuter.Y, LOOT_BOX);
            m_spriteBatch.Draw(m_textures[LOOT_BOX], lootBoxOuter, Color.White);

            int x = -5;
            int y = 6;

            foreach (Model.Item item in a_unit.BackPack.BackpackItems)
            {
                ////Om inte detta körs så blir det lite buggat. inget man ser om man inte ritar ut objektlagret items.
                item.ThisItem.Bounds.X = lootBoxInner.X + m_camera.GetScreenRectangle.X + x;
                item.ThisItem.Bounds.Y = lootBoxInner.Y + m_camera.GetScreenRectangle.Y + y;

                Vector2 itemPosition = m_camera.VisualizeCordinates(item.ThisItem.Bounds.X, item.ThisItem.Bounds.Y);
                
                //Om typen är en armor.
                if(item.GetType() == Model.GameModel.ARMOR)
                {
                    if(item.Type == Model.Armor.HEAD_ARMOR)
                    {
                        m_spriteBatch.Draw(m_textures[HEADARMOR], itemPosition, itemRect, Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
                    }
                }
                //Om typen är ett questitem.
                else if (item.GetType() == Model.GameModel.QUEST_ITEM)
                {
                    if (item.Type == Model.QuestItem.ENEMY_HEAD)
                    {
                        m_spriteBatch.Draw(m_textures[BOSS_HEAD], itemPosition, itemRect, Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
                    }
                }

                if (m_inputHandler.DidGetTargetedByLeftClick(new Rectangle((int)itemPosition.X, (int)itemPosition.Y, item.ThisItem.Bounds.Width, item.ThisItem.Bounds.Height)))
                {
                    m_player.ItemTarget = item;

                    if(m_player.ItemTarget.GetType() != Model.GameModel.QUEST_ITEM)
                    {
                        item.WasLooted = true;
                    }
                    if(!item.WasLooted)
                    {
                        //Kollar om man redan har ett fiendehuvud i backpacken.
                        if (!m_player.BackPack.BackpackItems.Exists(Item => Item.Type == Model.QuestItem.ENEMY_HEAD && Item.GetType() == Model.GameModel.QUEST_ITEM))
                        {
                            item.WasLooted = true;
                        }
                        else 
                        {
                            m_player.ItemTarget = null;
                        }
                    }
                }

                x++;
                if (x >= m_textures[LOOT_BOX].Width - displacementX * 2)
                {
                    y += 48;
                    x = 0;
                }

                //Kollar om spelaren har musen över ett item. isf så ska stats visas.
                if (m_inputHandler.MouseIsOver(m_camera.VisualizeRectangle(item.ThisItem.Bounds)))
                {
                    DrawItemStats(item, itemPosition);
                }
            }

            //Kontrollerar att man inte stängt lootrutan eller gått ifrån kroppen.
            if (m_inputHandler.DidGetTargetedByLeftClick(closeCross) || !m_player.ThisUnit.Bounds.Intersects(a_unit.ThisUnit.Bounds))
            {
                m_player.LootTarget = null;
            }

            

            a_unit.BackPack.BackpackItems.RemoveAll(Item => Item.WasLooted == true);
        }

        //Metod för utritning av skada och healing utfärdat av spelaren
        private void DrawDamageAndHealing(float a_elapsedTime)
        {
            #region AoutoHitDMG

            m_dmgCounter = m_dmgCounter + (a_elapsedTime * 75);

            if (m_player.IsWithinMeleRange && m_player.Target != null && m_player.SwingTime < 50 && m_player.SwingTime > 25)
            {
                Vector2 position = m_camera.VisualizeCordinates(m_player.Target.ThisUnit.Bounds.X + 20, m_player.Target.ThisUnit.Bounds.Y);

                Color color = Color.White;
                float scale = 1;

                if (m_player.AutohitDamage > 10)
                {
                    color = Color.Yellow;
                    scale = scale + m_dmgCounter / 50;
                }

                //Random: för att dmg ska slumpas lite i sidled, scale för critsen
                m_spriteBatch.DrawString(m_healFont, m_player.AutohitDamage.ToString(), position - new Vector2(m_randomNr + (scale * 5), m_dmgCounter), color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            else
            {
                m_dmgCounter = 0;
                Random rdm = new Random();
                m_randomNr = rdm.Next(-10, 10);
            } 
            #endregion

            #region Spells
            if (m_activeSpells.Exists(Spell => Spell.GetType() == Model.SpellSystem.INSTANT_HEAL && Spell.Duration == 0 && Spell.CoolDown > 4))
                DrawSpellOutput(a_elapsedTime, Model.SpellSystem.INSTANT_HEAL);
            else
                m_healingCounter = 0;  

            if (m_activeSpells.Exists(Spell => Spell.GetType() == Model.SpellSystem.SMITE && Spell.Duration == 0 && Spell.CoolDown > 1))
                DrawSpellOutput(a_elapsedTime, Model.SpellSystem.SMITE);
            else
                m_smiteCounter = 0;

            #endregion
        }

        //Metod för utritning av spells skada/healing
        private void DrawSpellOutput(float a_elapsedTime, Type a_spellType)
        {
            Model.Spell spell = m_activeSpells.Find(Spell => Spell.GetType() == a_spellType);
            string output = null;
            float counter = 0;
            Color color = Color.White;
            Vector2 position = Vector2.Zero;

            if (a_spellType == Model.SpellSystem.SMITE && m_player.Target != null)
            {
                Model.Smite smite = spell as Model.Smite;
                output = ((int)smite.Damage).ToString();
                m_smiteCounter = m_smiteCounter + (a_elapsedTime * 75);
                counter = m_smiteCounter;
                position = m_camera.VisualizeCordinates(m_player.Target.ThisUnit.Bounds.X, m_player.Target.ThisUnit.Bounds.Y);
            }
            else if (a_spellType == Model.SpellSystem.INSTANT_HEAL)
            {
                Model.InstantHeal heal = spell as Model.InstantHeal;
                output = ((int)heal.Heal).ToString();
                m_healingCounter = m_healingCounter + (a_elapsedTime * 75);
                counter = m_healingCounter;
                color = Color.LightGreen;
                position = m_camera.VisualizeCordinates(m_player.ThisUnit.Bounds.X, m_player.ThisUnit.Bounds.Y);
            }    
        
            if(output != null)
                 m_spriteBatch.DrawString(m_healFont, output, position - new Vector2(-7, counter), color);

        }

        //Metod för utritning av samtliga actionbars
        private void DrawActionBar()
        {
            Vector2 position = new Vector2(m_camera.GetScreenRectangle.Width / 2 - m_textures[ACTION_BAR].Bounds.Width / 2, m_camera.GetScreenRectangle.Height - m_textures[ACTION_BAR].Bounds.Height * 1.5f);
            m_spriteBatch.Draw(m_textures[ACTION_BAR], position, Color.White);

            m_keyPositions[0] = position + new Vector2(40,51);
            m_keyPositions[1] = position + new Vector2(88, 51);
            m_keyPositions[2] = position + new Vector2(150, 51);
            m_keyPositions[3] = position + new Vector2(189, 51);
            m_keyPositions[4] = position + new Vector2(239, 51);
            m_keyPositions[5] = position + new Vector2(288, 51);
            m_keyPositions[6] = position + new Vector2(334, 49);


            #region InstantHeal

            m_spriteBatch.Draw(m_iconTextures[ICON_INSTANT_HEAL], m_keyPositions[1], Color.White);

            if (m_activeSpells.Exists(Spell => Spell.GetType() == Model.SpellSystem.INSTANT_HEAL && Spell.CoolDown > 0))
            {
                m_spriteBatch.Draw(m_iconTextures[ICON_INSTANT_HEAL_CD], m_keyPositions[1], Color.White);
                string cd = ((int)m_activeSpells.Find(Spell => Spell.GetType() == Model.SpellSystem.INSTANT_HEAL && Spell.CoolDown > 0).CoolDown).ToString();
                m_spriteBatch.DrawString(m_spriteFontBig, cd, m_keyPositions[1] + new Vector2(14, 5), Color.White);
            } 

            #endregion

            #region Smite

            m_spriteBatch.Draw(m_iconTextures[ICON_SMITE], m_keyPositions[0], Color.White);

            if (m_activeSpells.Exists(Spell => Spell.GetType() == Model.SpellSystem.SMITE && Spell.CoolDown > 0))
            {
                m_spriteBatch.Draw(m_iconTextures[ICON_SMITE_CD], m_keyPositions[0], Color.White);
                string cd = ((int)m_activeSpells.Find(Spell => Spell.GetType() == Model.SpellSystem.SMITE && Spell.CoolDown > 0).CoolDown).ToString();
                m_spriteBatch.DrawString(m_spriteFontBig, cd, m_keyPositions[0] + new Vector2(14, 5), Color.White);
            }

            #endregion

            #region Bag

            m_spriteBatch.Draw(m_iconTextures[ICON_BAG], m_keyPositions[6], Color.White);

            if (m_player.BackPack.IsOpen)
            {
                m_spriteBatch.Draw(m_iconTextures[ICON_BAG_OPEN], m_keyPositions[6], Color.White);
            } 

            #endregion

            #region Map

            m_spriteBatch.Draw(m_iconTextures[ICON_WORLD_MAP], m_keyPositions[5], Color.White);
            
            if (m_player.IsLookingAtMap)
            {
                m_spriteBatch.Draw(m_iconTextures[ICON_WORLD_MAP_SELECTED], m_keyPositions[5], Color.White);
            } 
            
            #endregion

            #region CharacterPanel

            m_spriteBatch.Draw(m_iconTextures[ICON_CHAR_PANEL], m_keyPositions[4], Color.White);

            if (m_player.CharPanel.IsOpen)
            {
                m_spriteBatch.Draw(m_iconTextures[ICON_CHAR_PANEL_SELECTED], m_keyPositions[4], Color.White);
            }

            #endregion

            #region QuestLog

            m_spriteBatch.Draw(m_iconTextures[ICON_QUEST_LOG], m_keyPositions[3], Color.White);

            if (m_questSystem.IsWatchingQuestLog)
            {
                m_spriteBatch.Draw(m_iconTextures[ICON_QUEST_LOG_SELECTED], m_keyPositions[3], Color.White);
            }

            #endregion
        }

        //Metod för utritning av NPCs avatars samt deras mana/hp
        private void DrawTargetAvatars()
        {
            //Ritar bakgrunden till hp/mana.
            Rectangle pBackgroundRect = new Rectangle(110, 30, 169, 36);

            //RITAR SPELARENS HP.
            int playerHpWidth = CalculateHp(m_player);
            Rectangle playerHpRect = new Rectangle(105, 47, playerHpWidth, 16);
            m_spriteBatch.Draw(m_textures[GREEN_HP], playerHpRect, Color.White);

            //RITAR SPELARENS MANA.
            int playerManaWidth = CalculateMana(m_player);
            Rectangle playerManaRect = new Rectangle(105, 63, playerManaWidth, 16);
            m_spriteBatch.Draw(m_textures[MANA], playerManaRect, Color.White);

            //healthbar PROFILBILD.
            m_spriteBatch.Draw(m_textures[AVATAR_PLAYER], new Vector2(0, 0), Color.White);


            if (m_player.Target != null)
            {
                Texture2D target;
                Texture2D hp;

                if (m_player.Target.GetType() == Model.GameModel.ENEMY_NPC)
                {
                    hp = m_textures[RED_HP];

                    if (m_player.Target.Type == Model.Enemy.CLASS_WARRIOR)
                    {
                        target = m_textures[AVATAR_KNIGHT];
                    }
                    else if(m_player.Target.Type == Model.Enemy.CLASS_GOBLIN)
                    {
                        target = m_textures[AVATAR_GOBLIN];
                    }
                    else if (m_player.Target.Type == Model.Enemy.CLASS_MAGE)
                    {
                        target = m_textures[AVATAR_FIREMAGE];
                    }
                    else if (m_player.Target.Type == Model.Enemy.BOSS_A)
                    {
                        target = m_textures[AVATAR_BRYNOLF];
                    }
                    else 
                    {
                        target = m_textures[AVATAR_GOBLIN];
                    }
                }
                else if (m_player.Target.Type == Model.Friend.CITY_GUARD)
                {
                    target = m_textures[AVATAR_CITY_GUARD];
                    hp = m_textures[GREEN_HP];
                }
                else
                {
                    target = m_textures[AVATAR_OLD_MAN];
                    hp = m_textures[GREEN_HP];
                }

                //Målar ut targeten.
                //Ritar bakgrunden till hp/mana.
                Rectangle eBackgroundRect = new Rectangle(410, 30, 169, 36);

                //RITAR FIENDENS HP.
                int enemyHpWidth = CalculateHp(m_player.Target);
                Rectangle enemyHpRect = new Rectangle(407, 47, enemyHpWidth, 16);
                m_spriteBatch.Draw(hp, enemyHpRect, Color.White);

                //RITAR FIENDENS MANA.
                if(m_player.Target.GetType() == Model.GameModel.ENEMY_NPC)
                {
                    Model.Enemy enemy = m_player.Target as Model.Enemy;
                    if(enemy.Type == Model.Enemy.CLASS_MAGE || enemy.Type == Model.Enemy.BOSS_A)
                    {
                        int enemyManaWidth = CalculateMana(m_player.Target);
                        Rectangle enemyManaRect = new Rectangle(407, 63, enemyManaWidth, 16);
                        m_spriteBatch.Draw(m_textures[MANA], enemyManaRect, Color.White);
                    }
                }

                m_spriteBatch.Draw(target, new Vector2(300, 0), Color.White);
            }
        }

        //Metod för utritning av castbaren
        private void DrawCastBar()
        {
            foreach (Model.Spell spell in m_activeSpells)
            {
                //Kontrollerar att spellen kastas av spelaren.
                if(spell.Caster == m_player)
                {
                    //Kontrollerar att det är casttime kvar.
                    if(spell.CastTime > 0)
                    {
                    //Sätter width på castbaren.
                    float castBarWidth = m_player.ThisUnit.Bounds.Width;

                    float casted = spell.CastTime / spell.FullCastTime;
                    casted = 1 - casted;
                    castBarWidth = casted * castBarWidth;
                    Vector2 castBarVector = m_camera.VisualizeCordinates(m_player.ThisUnit.Bounds.X, (m_player.ThisUnit.Bounds.Y + m_player.ThisUnit.Bounds.Height));
                    Rectangle castBarRect = new Rectangle((int)castBarVector.X, (int)castBarVector.Y, (int)castBarWidth, 6);
                    m_spriteBatch.Draw(m_textures[MANA], castBarRect, Color.White);
                    }
                }
            }
        }

        //Metod för uträkning av hp-pool och dess förhållande till hp-grafiken
        private int CalculateHp(Model.Unit a_unit)
        {
            //Sätter hPwidth till så bred texturen är som håller hpt.
            float hpWidth = 169;

            //Kontrollerar om uniten inte har fullt liv.
            if (a_unit.CurrentHp < a_unit.TotalHp)
            {
                //Räknar ut hur många % hp som är kvar.
                float percentHpLeft = (a_unit.CurrentHp / a_unit.TotalHp);
                //Bestämmer många procent som ska vara kvar av hp baren.
                hpWidth = percentHpLeft * hpWidth;

                //KAPAR JUSTNU BORT 0,5 eftersom det blir en int i rektangeln....men skitsamma?
                return (int)hpWidth;
            }
            return (int)hpWidth;
        }

        //Metod för uträkning av mana-pool och dess förhållande till mana-grafiken
        private int CalculateMana(Model.Unit a_unit)
        {
            //Sätter manawidth till så bred texturen är som håller manan.
            float manaWidth = 169;

            //Kontrollerar om uniten inte har fullt mana.
            if (a_unit.CurrentMana < a_unit.TotalMana)
            {
                //Räknar ut hur många % mana som är kvar.
                float percentManaLeft = (a_unit.CurrentMana / a_unit.TotalMana);
                //Bestämmer många procent som ska vara kvar av mana baren.
                manaWidth = percentManaLeft * manaWidth;

                //KAPAR JUSTNU BORT 0,5 eftersom det blir en int i rektangeln....men skitsamma?
                return (int)manaWidth;
            }
            return (int)manaWidth;
        }

        //Metod för utritning av karaktärspanelen
        private void DrawCharacterPanel()
        {
            Vector2 pos = new Vector2();
            Vector2 panelPos = new Vector2(m_player.CharPanel.Position.X, m_player.CharPanel.Position.Y);
            Model.Item statsItem = null;

            Rectangle charPanelRect = new Rectangle(0, 0, m_textures[CHAR_PANEL_BG].Width, m_textures[CHAR_PANEL_BG].Height);
            Rectangle closeCross = GetCloseButton(m_player.CharPanel.Position.X, m_player.CharPanel.Position.Y, CHAR_PANEL_BG);

            if(m_player.CharPanel.IsOpen)
            {
                m_spriteBatch.Draw(m_textures[CHAR_PANEL_BG], panelPos, charPanelRect, Color.White);
                m_spriteBatch.DrawString(m_spriteFontSegoe, "Health: +" + m_player.TotalHp.ToString(), panelPos + new Vector2(80, 70), Color.White);
                m_spriteBatch.DrawString(m_spriteFontSegoe, "Mana: +" + m_player.TotalMana.ToString(), panelPos + new Vector2(80, 85), Color.White);
                m_spriteBatch.DrawString(m_spriteFontSegoe, "Damage: +10", panelPos + new Vector2(80, 100), Color.White);
                m_spriteBatch.DrawString(m_spriteFontSegoe, "Crit: 10%", panelPos + new Vector2(80, 115), Color.White);
                m_spriteBatch.DrawString(m_spriteFontSegoe, "Armor: +" + m_player.Armor.ToString(), panelPos + new Vector2(80, 130), Color.White);
                m_spriteBatch.DrawString(m_spriteFontSegoe, "Resist: +" + m_player.Resist.ToString(), panelPos + new Vector2(80, 145), Color.White);

                if (m_inputHandler.MouseIsOver(new Rectangle((int)m_player.CharPanel.Position.X, (int)m_player.CharPanel.Position.Y, m_textures[CHAR_PANEL_BG].Bounds.Width, m_textures[CHAR_PANEL_BG].Bounds.Height)))
                    m_inputHandler.MouseIsOverInterface = true;
                if (m_inputHandler.DidGetTargetedByLeftClick(closeCross))
                {
                    m_player.CharPanel.IsOpen = false;
                }
            }

            Vector2 position = new Vector2(m_player.CharPanel.Position.X +13, m_player.CharPanel.Position.Y+39);
            Rectangle itemRect = new Rectangle(0, 0, 32, 32);

            #region Ritar Equippade Items
            foreach (Model.Item item in m_player.CharPanel.EquipedItems)
            {
                item.ThisItem.Bounds.X = (int)m_player.CharPanel.Position.X + m_camera.GetScreenRectangle.X +10;
                item.ThisItem.Bounds.Y = (int)m_player.CharPanel.Position.Y + m_camera.GetScreenRectangle.Y +25;

                if (m_player.CharPanel.IsOpen)
                {
                    if (m_inputHandler.DidGetTargetedByLeftClick(m_camera.VisualizeRectangle(item.ThisItem.Bounds)))
                    {
                        m_player.CharPanelTarget = item;
                    }
                    if (item.GetType() == Model.GameModel.ARMOR)
                    {
                        Model.Armor Armor = item as Model.Armor;

                        if (Armor.Type == Model.Armor.HEAD_ARMOR)
                        {
                            m_spriteBatch.Draw(m_textures[HEADARMOR], position, itemRect, Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
                        }
                    }
                }
                //Kollar om spelaren har musen över ett item. isf så ska stats visas.
                if (m_inputHandler.MouseIsOver(m_camera.VisualizeRectangle(item.ThisItem.Bounds)))
                {
                    pos = position;
                    statsItem = item;
                }
            } 
            #endregion

            if (statsItem != null && m_player.CharPanel.IsOpen)
            {
                DrawItemStats(statsItem, pos);
            }
        }

        //Metod för utritning av backpack
        private void DrawBackpack()
        {
            //För Itemstats
            Vector2 pos = new Vector2();
            Model.Item statsItem = null;

            //Skapar en rektangel för backpacken.
            Rectangle backpackRect = new Rectangle(0, 0, m_textures[BACKPACK].Width, m_textures[BACKPACK].Height);
            Rectangle closeCross = GetCloseButton(m_player.BackPack.Position.X, m_player.BackPack.Position.Y, BACKPACK);

            //Kollar det här så objektens position hela tiden uppdateras.
            if (m_player.BackPack.IsOpen)
            {
                m_spriteBatch.Draw(m_textures[BACKPACK], new Vector2(m_player.BackPack.Position.X, m_player.BackPack.Position.Y), backpackRect, Color.White);

                if (m_inputHandler.MouseIsOver(new Rectangle((int)m_player.BackPack.Position.X, (int)m_player.BackPack.Position.Y, m_textures[BACKPACK].Bounds.Width, m_textures[BACKPACK].Bounds.Height)))
                    m_inputHandler.MouseIsOverInterface = true;
                if (m_inputHandler.DidGetTargetedByLeftClick(closeCross))
                {
                    m_player.BackPack.IsOpen = false;
                }

            }

            //Skapar en rektangel för item i backpacken.
            Rectangle itemRect = new Rectangle(0, 0, 32, 32);
            int x = 12;
            int y = 45;

            foreach (Model.Item item in m_player.BackPack.BackpackItems)
            {
                ////Om inte detta körs så blir det lite buggat. inget man ser om man inte ritar ut objektlagret items.
                item.ThisItem.Bounds.X = (int)m_player.BackPack.Position.X + m_camera.GetScreenRectangle.X + x;
                item.ThisItem.Bounds.Y = (int)m_player.BackPack.Position.Y + m_camera.GetScreenRectangle.Y + y;
                
                Vector2 itemPosition = new Vector2(m_player.BackPack.Position.X + x, m_player.BackPack.Position.Y + y);
                
                //Kontrollerar att backpacken är öppen. 
                if (m_player.BackPack.IsOpen)
                {
                    if (m_inputHandler.DidGetTargetedByLeftClick(m_camera.VisualizeRectangle(item.ThisItem.Bounds)))
                    {
                        m_player.BackpackTarget = item;
                    }

                    //Om typen är en armor.        
                    if (item.GetType() == Model.GameModel.ARMOR)
                    {
                        if (item.Type == Model.Armor.HEAD_ARMOR)
                        {
                            m_spriteBatch.Draw(m_textures[HEADARMOR], itemPosition, itemRect, Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
                        }
                    }
                    //Om typen är ett questitem.
                    else if (item.GetType() == Model.GameModel.QUEST_ITEM)
                    {
                        if (item.Type == Model.QuestItem.ENEMY_HEAD)
                        {
                            m_spriteBatch.Draw(m_textures[BOSS_HEAD], itemPosition, itemRect, Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
                        }
                    }
                }

                //Itemplacering i bagen
                x += 50;

                if (x >= m_textures[BACKPACK].Width)
                {
                    y += 50;
                    x = 0;
                }

                //Kollar om spelaren har musen över ett item. isf så ska stats visas.
                if (m_inputHandler.MouseIsOver(m_camera.VisualizeRectangle(item.ThisItem.Bounds)))
                {
                    pos = itemPosition;
                    statsItem = item;
                }
            }

            //Om spelaren håller över ett item (kollas i foreachloopen) och backpacken är öppen ritas stats ut.
            if(statsItem != null && m_player.BackPack.IsOpen)
            {
                DrawItemStats(statsItem, pos);
            }
            
            //Försöker ta bort ett objekt från backpacken och lägga på marken om spealren högerklickar på item i backpacken.
            if(m_player.BackPack.IsOpen)
            {
                for (int i = 0; i < m_player.BackPack.BackpackItems.Count; i++)
                {
                    if (m_inputHandler.DidGetTargetedByRightClick(m_camera.VisualizeRectangle(m_player.BackPack.BackpackItems[i].ThisItem.Bounds)) && m_player.BackPack.BackpackItems[i].GetType() != Model.GameModel.QUEST_ITEM)
                    {
                        m_player.BackPack.BackpackItems[i].ThisItem.Bounds = m_player.ThisUnit.Bounds;
                        m_worldItems.Add(m_player.BackPack.BackpackItems[i]);
                        m_player.BackPack.BackpackItems.Remove(m_player.BackPack.BackpackItems[i]);
                    }
                }
            }
        }

        //Metod för utritning av världskartan och spelarens position
        private void DrawWorldMap()
        { 
            if(m_player.IsLookingAtMap)
            {
                Vector2 position = new Vector2(m_camera.GetScreenRectangle.Width / 2 - m_worldMapTextures[WORLD_MAP_BG].Bounds.Width / 2, m_camera.GetScreenRectangle.Height - (m_worldMapTextures[WORLD_MAP_BG].Bounds.Height * 1.25f));
                Rectangle mapRect = new Rectangle((int)position.X + 15, (int)position.Y + 30, 500, 500);
                m_spriteBatch.Draw(m_worldMapTextures[WORLD_MAP_BG], position, Color.White);
                m_spriteBatch.Draw(m_worldMapTextures[WORLD_MAP], mapRect, Color.White);
                m_spriteBatch.Draw(m_worldMapTextures[PLAYER_POSITION], new Vector2(mapRect.X + m_player.ThisUnit.Bounds.X / 20, mapRect.Y + m_player.ThisUnit.Bounds.Y / 20), Color.White);

                //Fixa med anrop till GeatCloseRec
                Rectangle closeCross = new Rectangle((int)position.X + m_worldMapTextures[WORLD_MAP_BG].Width - 25, (int)position.Y, 25, 25);

                if (m_inputHandler.MouseIsOver(new Rectangle((int)position.X, (int)position.Y, m_worldMapTextures[WORLD_MAP_BG].Bounds.Width, m_worldMapTextures[WORLD_MAP_BG].Bounds.Height)))
                    m_inputHandler.MouseIsOverInterface = true;
                if (m_inputHandler.DidGetTargetedByLeftClick(closeCross))
                {
                    m_player.IsLookingAtMap = false;
                }
            }
        }

        //Metod för utritning av item information när man har musen över dess position
        internal void DrawItemStats(Model.Item a_item, Vector2 a_itemPosition)
        {
            if (a_item.GetType() == Model.GameModel.QUEST_ITEM)
            {
                m_spriteBatch.Draw(m_textures[ITEM_STATS_BG_SMALL], new Vector2(a_itemPosition.X + a_item.ThisItem.Bounds.Width - 10, a_itemPosition.Y), Color.White);
                m_spriteBatch.DrawString(m_spriteFontSegoe, "Quest Item", new Vector2(a_itemPosition.X + a_item.ThisItem.Bounds.Width, a_itemPosition.Y + 8), Color.White);
            }
            else
            {
                m_spriteBatch.Draw(m_textures[ITEM_STATS_BACKGROUND], new Vector2(a_itemPosition.X + a_item.ThisItem.Bounds.Width - 10, a_itemPosition.Y), Color.White);

                if (a_item.GetType() == Model.GameModel.ARMOR)
                {
                    Model.Armor armor = a_item as Model.Armor;
                    //Kod som visar itemets stats när man håller musen över.
                    if (armor.Type == Model.Armor.HEAD_ARMOR)
                    {
                        m_spriteBatch.DrawString(m_spriteFontSegoe, "Slot: Head", new Vector2(a_itemPosition.X + a_item.ThisItem.Bounds.Width, a_itemPosition.Y + 4), Color.White);
                    }

                    m_spriteBatch.DrawString(m_spriteFontSegoe, "Armor: +" + armor.ArmorValue.ToString(), new Vector2(a_itemPosition.X + a_item.ThisItem.Bounds.Width, a_itemPosition.Y + 19), Color.White);
                    m_spriteBatch.DrawString(m_spriteFontSegoe, "Resist: +" + armor.MagicResistValue.ToString(), new Vector2(a_itemPosition.X + a_item.ThisItem.Bounds.Width, a_itemPosition.Y + 34), Color.White);
                    m_spriteBatch.DrawString(m_spriteFontSegoe, "Health: +" + armor.HealthValue.ToString(), new Vector2(a_itemPosition.X + a_item.ThisItem.Bounds.Width, a_itemPosition.Y + 49), Color.White);
                    m_spriteBatch.DrawString(m_spriteFontSegoe, "Mana: +" + armor.ManaValue.ToString(), new Vector2(a_itemPosition.X + a_item.ThisItem.Bounds.Width, a_itemPosition.Y + 64), Color.White);
                }
            }
        }

        //Metod som retunerar den rektangel area som motsvarar medskickad actionbar
        internal Rectangle GetActionBarArea(char a_input)
        {
            int keyArrayIndex = 0;

            switch (a_input)
            {
                case View.InputHandler.ACTION_BAR_ONE:
                    keyArrayIndex = 0;
                    break;
                case View.InputHandler.ACTION_BAR_TWO:
                    keyArrayIndex = 1;
                    break;
                case View.InputHandler.ACTION_BAR_THREE:
                    keyArrayIndex = 2;
                    break;
                case View.InputHandler.QUEST_LOG:
                    keyArrayIndex = 3;
                    break;
                case View.InputHandler.CHARACTER_PANEL:
                    keyArrayIndex = 4;
                    break;
                case View.InputHandler.WORLD_MAP:
                    keyArrayIndex = 5;
                    break;
                case View.InputHandler.BACKPACK:
                    keyArrayIndex = 6;
                    break;
            }

            return new Rectangle((int)m_keyPositions[keyArrayIndex].X, (int)m_keyPositions[keyArrayIndex].Y, 48, 48);
        }

        //Metod som retunerar den rektangel area som motsvarar den yta som skall räknas som kryssruta för ett fönster
        internal Rectangle GetCloseButton(float a_panelLocationX, float a_panelLocationY, int a_textureId)
        {
            return new Rectangle((int)a_panelLocationX + m_textures[a_textureId].Width - 25, (int)a_panelLocationY, 25, 25);
        }
    }
}
