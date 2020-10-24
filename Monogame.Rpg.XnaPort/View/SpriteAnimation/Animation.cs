using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace View
{
    /// <summary>
    /// Klass med animeringsmetoder för utritning av spelets samtliga animationer
    /// </summary>
    public class Animation
    {
        #region Variebler

        private string m_name;
        private int m_framecountX;
        private int m_framecountY;
        private Texture2D m_myTexture;
        private float m_timePerFrame;
        private int m_previousFrameX;
        private int m_frameX;
        private int m_frameY;
        private float m_totalElapsed;

        public float m_rotation, m_scale, m_depth;
        public Vector2 m_origin;
 
        #endregion

        //Konstruktor
        public Animation(string a_textureName, Vector2 a_origin, float a_rotation, float a_scale, float a_depth, int a_frameX, int a_frameY, int a_framesPerSec)
        {
            this.m_name = a_textureName;
            this.m_origin = a_origin;
            this.m_rotation = a_rotation;
            this.m_scale = a_scale;
            this.m_depth = a_depth;
            this.m_framecountX = a_frameX;
            this.m_framecountY = a_frameY;
            this.m_timePerFrame = (float)0.5 / a_framesPerSec;
            this.m_previousFrameX = 1;
            this.m_frameX = 1;
            this.m_frameY = 0;
            this.m_totalElapsed = 0;
        }
        
        //Laddar textur, sätter framecount
        public void Load(ContentManager a_content)
        {
            m_myTexture = a_content.Load<Texture2D>("Sprites/" + m_name);
        }

        //Metod för animering av sprites i x-led
        public void AnimateSprite(float a_elapsed, int a_frameY)
        {
            m_frameY = a_frameY;

            m_frameX = m_previousFrameX;

            m_totalElapsed += a_elapsed;

            if (m_totalElapsed > m_timePerFrame)
            {
                m_frameX++;                 //Ökar position i x-led

                if (m_frameX == 3)          //Om x har flyttat 4 gånger..
                {
                    m_frameX = 0;           //nollställ x..
                }

                m_previousFrameX = m_frameX;
                m_totalElapsed -= m_timePerFrame;
            }
        }

        //Metod för animering av sprites i y-led
        public void AnimateVerticalSprite(float a_elapsed, int a_frameX)
        {
            m_frameX = a_frameX;

            m_totalElapsed += a_elapsed;

            if (m_totalElapsed > m_timePerFrame)
            {
                m_frameY++;                

                if (m_frameY == 4)         
                {
                    m_frameY = 3;           
                }

                m_totalElapsed -= m_timePerFrame;
            }
        }

        //Metod för visning av en spriteruta
        public void StaticTexture(float a_elapsed, int a_frameY)
        {
            m_frameY = a_frameY;
            m_frameX = 1;
        }

        public void DrawFrame(SpriteBatch a_batch, Vector2 a_screenPos, Color a_color)
        {
            DrawFrame(a_batch, m_frameX, m_frameY, a_screenPos, a_color);
        }

        //Ritar ut animationen
        public void DrawFrame(SpriteBatch a_batch, int a_frameX, int a_frameY, Vector2 a_screenPos, Color a_color)
        {
            int FrameWidth = m_myTexture.Width / m_framecountX;
            int FrameHeight = m_myTexture.Height / m_framecountY;

            Rectangle sourcerect = new Rectangle(FrameWidth * a_frameX, FrameHeight * a_frameY, FrameWidth, FrameHeight);

            a_batch.Draw(m_myTexture, a_screenPos, sourcerect, a_color, m_rotation, m_origin, m_scale, SpriteEffects.None, m_depth);
        }

        //Resetmetod
        public void Reset()
        {
            m_frameX = 0;
            m_totalElapsed = 0f;
        }
    }
}
