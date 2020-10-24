using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace View
{
    ///<summary>
    ///Huvudklass för hantering och utritning av spelets samtliga grafiska komponenter</summary> 
    class GameView
    {
        #region Klass-instanser
        private View.AnimationSystem m_animationSystem;
        private View.Conversation m_conversation;
        private View.UIView m_UIView;
        private View.InputHandler m_inputHandler;
        private View.SoundHandler m_soundHandler;
        private View.Camera m_camera;
        public View.UnitView m_unitView;
        private Model.GameModel m_gameModel; 
        #endregion

        #region Variabler
        private bool m_showDebug;
        private string m_zoneText = "";
        private SpriteFont m_spriteFont;
        private float m_zoneTextFader = 1;
        private bool m_zoneTextWasDrawn = true;
        private SpriteBatch m_spriteBatch;
        private Texture2D m_fireball;
        private Stopwatch m_lootWatch = new Stopwatch(); 
        #endregion

        public GameView(GraphicsDevice a_graphicsDevice, SpriteBatch a_spriteBatch, Model.GameModel a_gameModel, View.AnimationSystem a_animationSystem, View.InputHandler a_inputHandler, View.SoundHandler a_soundHandler)
        {
            this.m_gameModel = a_gameModel;
            this.m_camera = new Camera(a_graphicsDevice, a_gameModel);
            this.m_spriteBatch = a_spriteBatch;
            this.m_soundHandler = a_soundHandler;
            this.m_inputHandler = a_inputHandler;//Hantering av samtliga anv-inputs
            this.m_animationSystem = a_animationSystem;//System för sprite-animationer
            this.m_conversation = new Conversation(m_spriteBatch, m_gameModel, m_camera, m_inputHandler);//Behandlar/ritar dialogtexter
            this.m_UIView = new UIView(m_spriteBatch, m_camera, m_inputHandler, a_gameModel, m_conversation);//Utritning av user-interface
            this.m_unitView = new UnitView(m_gameModel, m_spriteBatch, m_camera, m_inputHandler, m_animationSystem, m_conversation);//Utritning av samtliga units
        }

        internal void LoadContent(ContentManager a_content)
        {
            m_animationSystem.LoadContent(a_content);
            m_UIView.LoadContent(a_content); 
            m_conversation.LoadContent(a_content);
            m_unitView.LoadContent(a_content);
            m_fireball = a_content.Load<Texture2D>("Textures/Spells/fireball");
            m_spriteFont = a_content.Load<SpriteFont>(@"Fonts\Nyala");
        }

        //Huvudmetod för utritning av spelets grafiska/visuella komponenter
        internal void DrawAndUpdate(float a_elapsedTime)
        {
            //Hämtar spelarObjekt
            Model.Player player = m_gameModel.m_playerSystem.m_player;
            
            //Uppdaterar kamera  
            m_camera.UpdateCamera();

            //Påbörjar utritning
            m_spriteBatch.Begin();
            
            #region Draw

            //Ritar bakgrund
            m_gameModel.m_currentMap.DrawLayer(m_spriteBatch, m_gameModel.m_level.IndexBackgroundLayerOne, m_camera.GetScreenRectangle, 0f);
            m_gameModel.m_currentMap.DrawLayer(m_spriteBatch, m_gameModel.m_level.IndexBackgroundLayerTwo, m_camera.GetScreenRectangle, 0f);
            m_gameModel.m_currentMap.DrawLayer(m_spriteBatch, m_gameModel.m_level.IndexBackgroundLayerThree, m_camera.GetScreenRectangle, 0f);

            m_inputHandler.MouseIsOverLoot = false;

            //Ritar Items
            DrawItems(a_elapsedTime);
            
            //Ritar spelare, enemies, NPCs
            m_unitView.DrawAndUpdateUnits(a_elapsedTime);

            //Ritar spells
            DrawSpells(m_gameModel.m_playerSystem.m_spellSystem.ActiveSpells, a_elapsedTime);
            DrawSpells(m_gameModel.m_enemySystem.m_enemySpellSystem.ActiveSpells, a_elapsedTime);

            //Ritar förgrund
            if (m_gameModel.m_level.foregroundVisible)
            {
                m_gameModel.m_currentMap.DrawLayer(m_spriteBatch, m_gameModel.m_level.IndexForeground, m_camera.GetScreenRectangle, 0f);
            }

            #region Dialogrutor

            //Triggar utritning av quest meddelanden
            if (player.Target != null)
            {
                if (player.Target.GetType() == Model.GameModel.FRIENDLY_NPC)
                {
                    Model.Friend npc = player.Target as Model.Friend;

                    if (m_inputHandler.DidGetTargetedByLeftClick(m_camera.VisualizeRectangle(npc.ThisUnit.Bounds)) &&
                        npc.ThisUnit.Bounds.Intersects(player.CollisionArea) && npc.CanInterract)
                    {
                        m_conversation.DrawDialog = true;
                    }
                    else if (!npc.ThisUnit.Bounds.Intersects(player.CollisionArea))
                    {
                        m_conversation.DrawDialog = false;
                    }

                    bool isQuestNpc = false;

                    if (npc.UnitId == m_gameModel.m_questSystem.ActiveNpc)
                        isQuestNpc = true;

                    if (m_conversation.DrawDialog)
                    {
                        m_conversation.DrawNPCText(player.Target, isQuestNpc);
                    }
                }
            }

            #endregion

            //Ritar UserInterface
            m_UIView.DrawAndUpdate(a_elapsedTime);

            //Uppdaterar och ritar zonetexter
            DrawZoneText();

            #region Utritning av objektlager (DEBUG)

            //if (m_inputHandler.PressedAndReleased('V'))
            //{
            //    m_showDebug = !m_showDebug;
            //}

            //if (m_showDebug)
            //{
            //    m_gameModel.m_currentMap.DrawObjectLayer(m_spriteBatch, m_gameModel.m_level.IndexCollision, m_camera.GetScreenRectangle, 0f);
            //    m_gameModel.m_currentMap.DrawObjectLayer(m_spriteBatch, m_gameModel.m_level.IndexInteraction, m_camera.GetScreenRectangle, 0f);
            //    m_gameModel.m_currentMap.DrawObjectLayer(m_spriteBatch, m_gameModel.m_level.IndexFriendlyNPC, m_camera.GetScreenRectangle, 0f);
            //    m_gameModel.m_currentMap.DrawObjectLayer(m_spriteBatch, m_gameModel.m_level.IndexEnemyNPC, m_camera.GetScreenRectangle, 0f);
            //    m_gameModel.m_currentMap.DrawObjectLayer(m_spriteBatch, m_gameModel.m_level.IndexItems, m_camera.GetScreenRectangle, 0f);
            //    m_gameModel.m_currentMap.DrawObjectLayer(m_spriteBatch, m_gameModel.m_level.IndexPlayer, m_camera.GetScreenRectangle, 0f);
            //    m_gameModel.m_currentMap.DrawObjectLayer(m_spriteBatch, m_gameModel.m_level.IndexEnemyZone, m_camera.GetScreenRectangle, 0f);
            //    m_gameModel.m_currentMap.DrawObjectLayer(m_spriteBatch, m_gameModel.m_level.IndexGraveyard, m_camera.GetScreenRectangle, 0f);
            //}

            #endregion 

            #endregion           

            m_spriteBatch.End();

            //Repeterar uppdatering av kamera (Buggfix)  
            m_camera.UpdateCamera();

            //Uppdaterar och spelar spelmusik
            UpdateMusic();
        }

        //Metod för uppdatering av spelmusik
        private void UpdateMusic()
        {
            bool inTown = false;
            bool inDungeon = false;

            foreach (MapObject obj in m_gameModel.m_level.ZoneLayer.MapObjects)
            {
                if (obj.Name == "Town" && obj.Bounds.Intersects(m_gameModel.m_playerSystem.m_player.ThisUnit.Bounds))
                    inTown = true;
                else if (obj.Name == "Dungeon" && obj.Bounds.Intersects(m_gameModel.m_playerSystem.m_player.ThisUnit.Bounds))
                    inDungeon = true;
            }

            if (inTown)
                m_soundHandler.PlaySoundTrack(View.SoundHandler.TOWN);
            else if (inDungeon)
                m_soundHandler.PlaySoundTrack(View.SoundHandler.DUNGEON);
            else
                m_soundHandler.PlaySoundTrack(View.SoundHandler.WORLD);

        }

        //Metod för uppdatering av zontexter
        private void UpdateZoneText()
        {
            bool inZone = false;
            foreach (MapObject obj in m_gameModel.m_level.ZoneLayer.MapObjects)
            {
                //Testar om spelaren är i en zon.
                if (m_gameModel.m_playerSystem.m_player.ThisUnit.Bounds.Intersects(obj.Bounds))
                {
                    inZone = true;
                    //Testar om spelaren kommit in i en ny zon.
                    if (obj.Properties["Name"].Value != m_zoneText)
                    {
                        m_zoneTextFader = 1;
                        m_zoneText = Convert.ToString(obj.Properties["Name"].Value);
                        m_zoneTextWasDrawn = false;
                    }
                }
            }
            //Testar om man lämnat zonen.
            if (!inZone && m_zoneTextFader == 1)
            {
                m_zoneText = "";
            }
        }

        //Metod för uppdatering och utritning av Zontexter
        private void DrawZoneText()
        {
            UpdateZoneText();

            //Om zontexten inte redan har ritats ut.
            if (!m_zoneTextWasDrawn)
            {
                m_zoneTextFader = m_zoneTextFader - 0.005f;
                Color color = new Color(m_zoneTextFader, m_zoneTextFader, m_zoneTextFader, m_zoneTextFader);
                Vector2 stringSize = m_spriteFont.MeasureString(m_zoneText);
                m_spriteBatch.DrawString(m_spriteFont, m_zoneText, new Vector2((m_camera.GetScreenRectangle.Width / 2) - stringSize.X/2, m_camera.GetScreenRectangle.Height / 3), color);

                if (m_zoneTextFader <= 0)
                {
                    m_zoneTextFader = 1;
                    m_zoneTextWasDrawn = true;
                }
            }
        }

        //Metod för utritning av Items på kartan samt sättande spelarens Itemtarget
        private void DrawItems(float a_elapsedTime)
        {
            List<Model.Item> items = m_gameModel.m_itemSystem.m_items;
            Model.Player player = m_gameModel.m_playerSystem.m_player;

            foreach (Model.Item item in items)
            {
                if (m_inputHandler.MouseIsOver(m_camera.VisualizeRectangle(item.ThisItem.Bounds)))
                    m_inputHandler.MouseIsOverLoot = true;

               //Kontrollerar om spelaren har targetat ett item.
                if (m_inputHandler.DidGetTargetedByLeftClick(m_camera.VisualizeRectangle(item.ThisItem.Bounds)))
                {
                    player.ItemTarget = item;
                }

                if (player.ItemTarget == item)
                {
                    if (m_inputHandler.DidGetTargetedByLeftClick(m_camera.VisualizeRectangle(item.ThisItem.Bounds)) &&
                        player.ItemTarget.ThisItem.Bounds.Intersects(player.CollisionArea) && !m_lootWatch.IsRunning)
                    {
                        item.WasLooted = true;
                    }
                    else
                    {
                        item.WasLooted = false;
                    }
                }
               
                if (item.GetType() == Model.GameModel.ARMOR)
                {
                    Model.Armor Armor = item as Model.Armor;

                    if (Armor.Type == Model.Armor.HEAD_ARMOR)
                    {
                        Vector2 position = m_camera.VisualizeCordinates(Armor.ThisItem.Bounds.Location.X, Armor.ThisItem.Bounds.Location.Y);
                        int itemAnimation = 0; 

                        itemAnimation = AnimationSystem.FACING_CAMERA;

                        m_animationSystem.UpdateAndDraw(a_elapsedTime, Color.White, position, itemAnimation, AnimationSystem.ITEM_PURPLE_CHEST);
                    }
                }
            }
        }

        ///<summary>Utritning av samtliga aktiva spells</summary>
        ///<param name="a_activeSpells">Aktiva spells från ett Model.SpellSystem</param>
        private void DrawSpells(List<Model.Spell> a_activeSpells, float a_elapsedTime)
        {
            foreach (Model.Spell spell in a_activeSpells)
            {
                //Kontrollerar att spellen fortfarande är aktiv och att den har kastats.
                if(spell.Duration > 0 && spell.CastTime < 0)
                {
                    if (spell.GetType() == Model.SpellSystem.FIRE_BALL)
                    {
                        Vector2 fireballPos = m_camera.VisualizeCordinates((int)spell.Position.X, (int)spell.Position.Y);
                        m_animationSystem.UpdateAndDraw(a_elapsedTime, Color.White, fireballPos, AnimationSystem.SMITE, AnimationSystem.FIREBALL_TEXTURE);
                    }
                }
                if (spell.GetType() == Model.SpellSystem.SMITE && spell.CoolDown > 1.7 && spell.Caster.Target != null)
                {
                    Vector2 smitePos = m_camera.VisualizeCordinates(spell.Caster.Target.ThisUnit.Bounds.X, spell.Caster.Target.ThisUnit.Bounds.Y);
                    m_animationSystem.UpdateAndDraw(a_elapsedTime, Color.White, smitePos, AnimationSystem.SMITE, AnimationSystem.SMITE_TEXTURE);
                }
            }
        }

        //Metod för hämtning av alla objekt som kameran "ser"
        private List<MapObject> GetObjectsInRegion(Map a_currentMap)
        {
            List<MapObject> mapObjects = new List<MapObject>();

            foreach (MapObject mapObject in a_currentMap.GetObjectsInRegion(m_camera.GetScreenRectangle))
            {
                mapObjects.Add(mapObject);
            }

            return mapObjects;
        }

        //Hantering av anv-interaktion via InputHandler
        #region InputHantering

        internal bool DidPressKey(char a_key)
        {
            return m_inputHandler.IsKeyDown(a_key);
        }

        internal bool DidPressAndReleaseKey(char a_key)
        {
            return m_inputHandler.PressedAndReleased(a_key);
        }

        internal bool MouseIsOverInterface()
        {
            return m_inputHandler.MouseIsOverInterface;
        }

        internal bool DidActivateActionBar(char a_actionBar)
        {
            Rectangle actionBar = m_UIView.GetActionBarArea(a_actionBar);

            return (m_inputHandler.DidGetTargetedByLeftClick(actionBar) || DidPressAndReleaseKey(a_actionBar));
        } 

        internal Vector2 GetMapMousePosition()
        {
            if (m_inputHandler.RightButtonIsDown())
            {
                if (!m_inputHandler.MouseIsOverInterface)
                    return m_camera.LogicalizeCordinates(m_inputHandler.GetMouseState().X, m_inputHandler.GetMouseState().Y);
            }

            return Vector2.Zero;
        }

        internal bool UnTarget()
        {
            Model.Player player = m_gameModel.m_playerSystem.m_player;                
            
            return (m_inputHandler.DidLeftClick() && !m_inputHandler.MouseIsOverObject && !m_inputHandler.MouseIsOverInterface) && !m_conversation.DrawDialog ||
                    !player.Target.ThisUnit.Bounds.Intersects(m_camera.GetScreenRectangle);
        }
        
        #endregion

    }
}
