using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace View
{
    /// <summary>
    /// Klass för inputhantering från användaren
    /// </summary>
    class InputHandler
    {
        #region Keyboard konstanter

        public const char UP = 'W';
        public const char DOWN = 'S';
        public const char LEFT = 'A';
        public const char RIGHT = 'D';
        public const char ACTION_BAR_ONE = '1';
        public const char ACTION_BAR_TWO = '2';
        public const char ACTION_BAR_THREE = '3';
        public const char ACTION_BAR_FOUR = '4';
        public const char BACKPACK = 'B';
        public const char CHARACTER_PANEL = 'C';
        public const char QUEST_LOG = 'L';
        public const char WORLD_MAP = 'M'; 

        #endregion

        #region Variabler

        //Keyboard/Mouse states
        private KeyboardState m_kbs;
        private KeyboardState m_prevKbs;
        private MouseState m_mouseState;
        private MouseState m_prevMoseState;

        //Mouse
        private bool m_isMouseOverEnemy;
        private bool m_isMouseOverNPC = false;
        private bool m_isMouseOverInterface;
        private bool m_isMouseOverLoot;

        //Meny inputs
        private bool m_soundDisabled;
        private bool m_musicDisabled; 

        #endregion

        //Metoder för uppdatering och sättande av keyboard & mouse states
        internal void SetKeyboardState()
        {
            m_prevKbs = m_kbs;
            m_kbs = Keyboard.GetState();
        }
        internal void SetMouseState()
        {
            m_prevMoseState = m_mouseState;
            m_mouseState = Mouse.GetState();
        }

        //Retunerar aktuell mouse state
        internal MouseState GetMouseState()
        {
            return m_mouseState;
        }

        //Booleans metoder för användar inputs

        internal bool IsKeyDown(char a_key)
        {
            return m_kbs.IsKeyDown((Keys)a_key);
        }

        internal bool PressedAndReleased(char a_key)
        {
            if (m_kbs.IsKeyUp((Keys)a_key) && m_prevKbs.IsKeyDown((Keys)a_key))
            {
                return true;
            }

            return false;
        }

        internal bool DidGetTargetedByLeftClick(Rectangle a_target)
        {
            //Om spelare har tryckt på vänster musknapp och släppt.
            if (m_prevMoseState.LeftButton == ButtonState.Pressed && m_mouseState.LeftButton == ButtonState.Released)
            {
                //Om musens Position/"Triangel" är innanför en motståndare så sätts target.
                if (a_target.Intersects(new Rectangle(m_mouseState.X, m_mouseState.Y, 1, 1)))
                {
                    return true;
                }
            }

            return false;
        }

        internal bool DidGetTargetedByRightClick(Rectangle a_target)
        {
            //Om spelare har tryckt på vänster musknapp och släppt.
            if (m_prevMoseState.RightButton == ButtonState.Pressed && m_mouseState.RightButton == ButtonState.Released)
            {
                //Om musens Position/"Triangel" är innanför en motståndare så sätts target.
                if (a_target.Intersects(new Rectangle(m_mouseState.X, m_mouseState.Y, 1, 1)))
                {
                    return true;
                }
            }

            return false;
        }

        internal bool DidRightClick()
        {
            return (m_prevMoseState.RightButton == ButtonState.Pressed && m_mouseState.RightButton == ButtonState.Released);
        }

        internal bool DidLeftClick()
        {
            return (m_prevMoseState.LeftButton == ButtonState.Pressed && m_mouseState.LeftButton == ButtonState.Released);
        }

        internal bool MouseIsOver(Rectangle a_area)
        {
            return (a_area.Intersects(new Rectangle(m_mouseState.X, m_mouseState.Y, 1, 1)));
        }

        internal bool MouseIsOverEnemy
        {
            get { return m_isMouseOverEnemy; }
            set { m_isMouseOverEnemy = value; }
        }

        internal bool MouseIsOverObject
        {
            get { return m_isMouseOverEnemy || m_isMouseOverNPC || m_isMouseOverLoot; }
        }

        internal bool MouseIsOverInterface
        {
            get { return m_isMouseOverInterface; }
            set { m_isMouseOverInterface = value; }
        }

        internal bool MouseIsOverLoot
        {
            get { return m_isMouseOverLoot; }
            set { m_isMouseOverLoot = value; }
        }

        internal bool RightButtonIsDown()
        {
            return m_mouseState.RightButton == ButtonState.Pressed;
        }

        internal bool LeftButtonIsDown()
        {
            return m_mouseState.LeftButton == ButtonState.Pressed;
        }

        internal bool PressedAndReleasedEsc()
        {
            return m_kbs.IsKeyUp(Keys.Escape) && m_prevKbs.IsKeyDown(Keys.Escape);
        }

        internal bool MusicDisabled
        {
            get { return m_musicDisabled; }
            set { m_musicDisabled = value; }
        }

        internal bool SoundDisabled
        {
            get { return m_soundDisabled; }
            set { m_soundDisabled = value; }
        }
    }
}
