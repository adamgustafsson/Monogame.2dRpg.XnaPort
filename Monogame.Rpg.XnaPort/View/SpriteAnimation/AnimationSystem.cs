using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace View
{
    ///
    /// Systemklass för hantering av frameanimationer.
    ///
    class AnimationSystem
    {
        //Variabler
        private Animation[] m_spriteTextures;
        private SpriteBatch m_spriteBatch;
        private const float m_rotation = 0;
        private const float m_depth = 0.5f;
        private const int m_framesX = 3;
        private const int m_framesY = 4;

        //Publika konstanter för animerings id
        #region Animerings ID
        public const int FACING_CAMERA = 1;
        public const int FACING_LEFT = 2;
        public const int FACING_RIGHT = 3;
        public const int FACING_AWAY = 4;
        public const int MOVING_LEFT = 5;
        public const int MOVING_RIGHT = 6;
        public const int MOVING_UP = 7;
        public const int MOVING_DOWN = 8;
        public const int IS_DEAD = 9;
        public const int WAS_HEALED = 10;
        public const int IS_CASTING_FIREBALL = 11;
        public const int IS_CASTING_HEAL = 12;
        public const int VERTICAL_ANIMATION = 13;
        public const int SMITE = 14;
        public const int FIREBALL = 15;
        #endregion

        //Publika konstanter för textur id enligt array
        #region Textur ID
        public const int PLAYER = 0;
        public const int ENEMY_KNIGHT = 1;
        public const int NPC_OLD_MAN = 2;
        public const int WEAPON_SWORD = 3;
        public const int ITEM_PURPLE_CHEST = 4;
        public const int ENEMY_MAGE = 5;
        public const int ARMOR_HEAD = 6;
        public const int ENEMY_GOBLIN = 7;
        public const int BOSS_A = 8;
        public const int CLASS_SCREEN_TEMPLAR = 9;
        public const int CLASS_SCREEN_DESCENDANT = 10;
        public const int CLASS_SCREEN_PROPHET = 11;
        public const int CITY_GUARD = 12;
        public const int SMITE_TEXTURE = 13;
        public const int FIREBALL_TEXTURE = 14;
        #endregion

        //Konstruktor - skapar samtliga animationsobjekt
        public AnimationSystem(SpriteBatch a_spriteBatch)  
        {
            this.m_spriteBatch = a_spriteBatch;
            //@PARAM: (Texturname, Origin, Rotation, Scale, Depth, FramesX, FramesY, FPS)
            this.m_spriteTextures = new Animation[15] {   new Animation("player", Vector2.Zero, m_rotation, 1.5f, m_depth, m_framesX, 7, 6),
                                                    new Animation("enemy_knight", Vector2.Zero, m_rotation, 1.5f, m_depth, m_framesX, 6, 6),
                                                    new Animation("npc_old_man", Vector2.Zero, m_rotation, 1.5f, m_depth, m_framesX, m_framesY, 0),
                                                    new Animation("swordC", Vector2.Zero, m_rotation, 1.5f, m_depth, m_framesX, m_framesY, 6),
                                                    new Animation("PurpleChest", Vector2.Zero, m_rotation, 1.5f, m_depth, m_framesX, m_framesY, 1),
                                                    new Animation("enemyfiremage", Vector2.Zero, m_rotation, 1.5f, m_depth, m_framesX, 6, 4),
                                                    new Animation("headArmor3", Vector2.Zero, m_rotation, 1.5f, m_depth, m_framesX, m_framesY, 5),
                                                    new Animation("goblin", Vector2.Zero, m_rotation, 1.5f, m_depth, m_framesX, 6, 5),
                                                    new Animation("enemyboss", Vector2.Zero, m_rotation, 1.5f, m_depth, m_framesX, 7, 5),
                                                    new Animation("templar", Vector2.Zero, m_rotation, 2.0f, m_depth, m_framesX, m_framesY, 4),
                                                    new Animation("descendant", Vector2.Zero, m_rotation, 2.0f, m_depth, m_framesX, m_framesY, 4),
                                                    new Animation("prophet", Vector2.Zero, m_rotation, 2.0f, m_depth, m_framesX, m_framesY, 4),
                                                    new Animation("cityGuard", Vector2.Zero, m_rotation, 1.5f, m_depth, m_framesX, m_framesY, 4),
                                                    new Animation("smite", Vector2.Zero,  m_rotation, 2.0f, m_depth, m_framesX, 1, 4),
                                                    new Animation("fireball2", Vector2.Zero,  m_rotation, 1.5f, m_depth, 4, 1, 7)};
        }

        //Anropar "Load" i texturklassen
        internal void LoadContent(Microsoft.Xna.Framework.Content.ContentManager a_content)
        {
            foreach(Animation sprite in m_spriteTextures)
            {
                sprite.Load(a_content);
            }
        }

        //Uppdaterar och ritar via UpdateFrame & DrawFrame
        internal void UpdateAndDraw(float a_elapsedTime, Color a_color, Vector2 a_texturePos, int a_animation, int a_texture)
        {
            int frameY = -1;
            int frameX = -1;
            bool staticAnimation = false;
            bool verticalAnimation = false;

            switch (a_animation)
            {
                case MOVING_DOWN:
                    frameY = 0;
                    break;
                case SMITE:
                    frameY = 0;
                    break;
                case MOVING_UP:
                    frameY = 3;
                    break;
                case MOVING_RIGHT:
                    frameY = 2;
                    break;
                case MOVING_LEFT:
                    frameY = 1;
                    break;
                case FACING_CAMERA:
                    frameY = 0;
                    staticAnimation = true;
                    break;
                case FACING_LEFT:
                    frameY = 1;
                    staticAnimation = true;
                    break;
                case FACING_RIGHT:
                    frameY = 2;
                    staticAnimation = true;
                    break;
                case FACING_AWAY:
                    frameY = 3;
                    staticAnimation = true;
                    break;
                case IS_DEAD:
                    frameY = 5;
                    staticAnimation = true;
                    break;
                case WAS_HEALED:
                    break;
                case IS_CASTING_FIREBALL:
                    frameY = 4;
                    break;
                case IS_CASTING_HEAL:
                    frameY = 6;
                    break;
                case VERTICAL_ANIMATION:
                    frameX = 1;
                    verticalAnimation = true;
                    break;
            }

            if (staticAnimation)
            {
                m_spriteTextures[a_texture].StaticTexture(a_elapsedTime, frameY);
                
            }
            else if (verticalAnimation)
            {
                m_spriteTextures[a_texture].AnimateVerticalSprite(a_elapsedTime, frameX);
            }
            else
            {
                m_spriteTextures[a_texture].AnimateSprite(a_elapsedTime, frameY); 
            }

            m_spriteTextures[a_texture].DrawFrame(m_spriteBatch, a_texturePos, a_color);

        }

    }
}
