using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace View
{
    /// <summary>
    /// Klass för ritning av menyer och skärmar
    /// </summary>
    class ScreenView
    {
        #region Variabler

        private SpriteFont m_segoeKeycaps;
        private Texture2D[] m_screenTextures;
        private Texture2D[] m_screenButtons;
        private Texture2D[] m_mouseTextures;
        private SpriteBatch m_spriteBatch;

        private Stopwatch m_stopWatch = new Stopwatch();
        private View.AnimationSystem m_animationSys;
        private View.InputHandler m_inputHandler;
        private View.SoundHandler m_soundHandler; 

        #endregion

        #region Variabler - Booleans

        private bool m_didPressNewGame;
        private bool m_didPressQuit;
        private bool m_didPressExit;
        private bool m_didHover;
        private bool m_didChooseClass;
        private bool m_fullScreen = false;

        private bool m_showOptions;
        private bool m_showCredits;
        private bool m_showTemplarSelection = true;
        private bool m_showProphetSelection;
        private bool m_showDescendantSelection;
 
        #endregion

        #region SCREEN IMG ID
        public const int SCREEN_START = 0;
        public const int SCREEN_ANIMATION_A = 1;
        public const int SCREEN_ANIMATION_B = 2;
        public const int SCREEN_CLASS_SELECT = 3;
        public const int SCREEN_SELECT_TEMPLAR = 4;
        public const int SCREEN_SELECT_PROPHET = 5;
        public const int SCREEN_SELECT_DESCENDANT = 6;
        public const int SCREEN_BORDER_BG = 7;
        public const int SCREEN_TRANSP_BG = 8;
        public const int SCREEN_PAUSE_BG = 9;
        public const int SCREEN_OPTION = 10;
        public const int SCREEN_END_BG = 11;
        public const int SCREEN_CREDITS = 12; 
        #endregion

        #region BUTTON IMG ID
        public const int BUTTONS_START_MENU = 0;
        public const int BUTTONS_NEWGAME_SELECTED = 1;
        public const int BUTTONS_CREDITS_SELECTED = 2;
        public const int BUTTONS_SELECT = 3;
        public const int BUTTONS_SELECT_GRAY = 4;
        public const int BUTTONS_SELECT_SELECTED = 5;
        public const int BUTTONS_OPTIONS_SELECTED = 6;
        public const int BUTTONS_PAUSE_MENU = 7;
        public const int BUTTONS_QUIT_SELECTED = 8;
        public const int BUTTONS_CHECK_BOX = 9;
        public const int BUTTONS_OK = 10;
        public const int BUTTONS_OK_SELECTED = 11;
        public const int BUTTONS_EXIT_SELECTED = 12; 
        #endregion

        //Konstruktor; laddar gränssnitt & bilder
        public ScreenView(SpriteBatch a_spriteBatch, View.AnimationSystem a_animationSystem, View.InputHandler a_inputHandler, View.SoundHandler a_soundHandler)
        {
            this.m_spriteBatch = a_spriteBatch;
            this.m_animationSys = a_animationSystem;
            this.m_inputHandler = a_inputHandler;
            this.m_soundHandler = a_soundHandler;
        }

        public void LoadContent(ContentManager a_content)
        {
            #region m_screenTextures
            this.m_screenTextures = new Texture2D[13] {a_content.Load<Texture2D>("Textures/Screens/Images/BG2"),
                                           a_content.Load<Texture2D>("Textures/Screens/Images/startanimation1"),
                                           a_content.Load<Texture2D>("Textures/Screens/Images/test9"),
                                           a_content.Load<Texture2D>("Textures/Screens/Images/bg3"),
                                           a_content.Load<Texture2D>("Textures/Screens/Images/selectTemplar"),
                                           a_content.Load<Texture2D>("Textures/Screens/Images/selectProphet"),
                                           a_content.Load<Texture2D>("Textures/Screens/Images/selectDescendant"),
                                           a_content.Load<Texture2D>("Textures/Screens/Images/borderBg"),
                                           a_content.Load<Texture2D>("Textures/Screens/Images/transpBg"),
                                           a_content.Load<Texture2D>("Textures/Screens/Images/pauseBg"),
                                           a_content.Load<Texture2D>("Textures/Screens/Images/optionScreen"),
                                           a_content.Load<Texture2D>("Textures/Screens/Images/endBG"),
                                           a_content.Load<Texture2D>("Textures/Screens/Images/credits")};
            #endregion

            #region m_screenButtons
            this.m_screenButtons = new Texture2D[13] {a_content.Load<Texture2D>("Textures/Screens/Buttons/menyFinal"),
                                           a_content.Load<Texture2D>("Textures/Screens/Buttons/newGameSelect"),
                                           a_content.Load<Texture2D>("Textures/Screens/Buttons/creditsSelect"),                                          
                                           a_content.Load<Texture2D>("Textures/Screens/Buttons/select"),
                                           a_content.Load<Texture2D>("Textures/Screens/Buttons/selectGray"),
                                           a_content.Load<Texture2D>("Textures/Screens/Buttons/selectSelected"),
                                           a_content.Load<Texture2D>("Textures/Screens/Buttons/optionsSelect"),
                                           a_content.Load<Texture2D>("Textures/Screens/Buttons/pauseMeny"),
                                           a_content.Load<Texture2D>("Textures/Screens/Buttons/quitSelected"),
                                           a_content.Load<Texture2D>("Textures/Screens/Buttons/checkBox"),
                                           a_content.Load<Texture2D>("Textures/Screens/Buttons/ok"),
                                           a_content.Load<Texture2D>("Textures/Screens/Buttons/okSelected"),
                                           a_content.Load<Texture2D>("Textures/Screens/Buttons/exitSelected")};
            #endregion

            this.m_mouseTextures = new Texture2D[2] { a_content.Load<Texture2D>("Textures/Interface/cursor"),
                                                      a_content.Load<Texture2D>("Textures/Interface/cursorSelect")};
            this.m_segoeKeycaps = a_content.Load<SpriteFont>("Fonts/Segoe");
            this.m_animationSys.LoadContent(a_content);
        }

        //Metod för uppspelning av Start musik
        internal void PlayStartTheme()
        {
            if (m_inputHandler.MusicDisabled)
                m_soundHandler.StopSong();
            else
                m_soundHandler.PlaySoundTrack(SoundHandler.THEME);
        }

        //Metod för uppspelande av meny-ljud
        internal void PlayMenuSound(int a_sound)
        {
            if (a_sound == SoundHandler.MENU_BUTTON_HOVER && !m_didHover)
            {
                m_didHover = true;
                m_soundHandler.PlaySound(SoundHandler.MENU_BUTTON_HOVER, 0.2f);
            }
            else if (a_sound == SoundHandler.MENU_BUTTON_SELECT)
                m_soundHandler.PlaySound(SoundHandler.MENU_BUTTON_SELECT, 0.2f);
            else if (a_sound == SoundHandler.MENU_BUTTON_SELECT_B)
                m_soundHandler.PlaySound(SoundHandler.MENU_BUTTON_SELECT_B, 0.4f);

        }

        //Metod för utritning av startskärm
        internal void DrawStartScreen()
        {
            m_stopWatch.Start();

            PlayStartTheme();

            //Ritar start BG
            DrawImage(SCREEN_START, Vector2.Zero, new Point(1280, 720), 1.0f);

            #region Animation
            int y = 0;
            int x = 10;

            if (m_stopWatch.ElapsedMilliseconds > 400)
            {
                y = 59;
                x = 192;
            }
            if (m_stopWatch.ElapsedMilliseconds > 800)
            {
                y = 118;
                x = 374;
            }
            if (m_stopWatch.ElapsedMilliseconds > 1200)
            {
                m_stopWatch.Stop();
                m_stopWatch.Reset();
            } 
            #endregion

            //Ritar animation bilder
            m_spriteBatch.Draw(m_screenTextures[SCREEN_ANIMATION_A], new Vector2(23, 260), new Rectangle(0, y, 265, 59), Color.White);
            m_spriteBatch.Draw(m_screenTextures[SCREEN_ANIMATION_B], new Vector2(154, 352), new Rectangle(x, 0, 170, 247), Color.White);

            if (!m_showOptions && !m_showCredits)
            {
                //Ritar starmeny
                DrawImage(SCREEN_BORDER_BG, new Vector2(727, 0), new Point(300, 720), 1.0f);
                DrawButton(BUTTONS_START_MENU, new Vector2(750, 325), new Point(512, 512), 0.5f);

                //Skapar knapp area för startmeny
                Rectangle newGameArea = new Rectangle(750, 325, 250, 40);
                Rectangle optionsArea = new Rectangle(750, 404, 250, 40);
                Rectangle creditsArea = new Rectangle(750, 450, 250, 30);
                Rectangle exitArea = new Rectangle(750, 485, 250, 30);

                #region Inputhantering för startmeny
                if (m_inputHandler.MouseIsOver(newGameArea))
                {
                    DrawButton(BUTTONS_NEWGAME_SELECTED, new Vector2(750, 319), new Point(512, 100), 0.5f);
                    PlayMenuSound(SoundHandler.MENU_BUTTON_HOVER);

                    if (m_inputHandler.DidGetTargetedByLeftClick(newGameArea))
                    {
                        PlayMenuSound(SoundHandler.MENU_BUTTON_SELECT_B);
                        m_didPressNewGame = true;
                    }
                }
                else if (m_inputHandler.MouseIsOver(optionsArea))
                {
                    DrawButton(BUTTONS_OPTIONS_SELECTED, new Vector2(750, 396), new Point(512, 100), 0.5f);
                    PlayMenuSound(SoundHandler.MENU_BUTTON_HOVER);

                    if (m_inputHandler.DidGetTargetedByLeftClick(optionsArea))
                    {
                        PlayMenuSound(SoundHandler.MENU_BUTTON_SELECT);
                        m_showOptions = true;
                    }
                }
                else if (m_inputHandler.MouseIsOver(creditsArea))
                {
                    DrawButton(BUTTONS_CREDITS_SELECTED, new Vector2(750, 434), new Point(512, 100), 0.5f);
                    PlayMenuSound(SoundHandler.MENU_BUTTON_HOVER);

                    if (m_inputHandler.DidGetTargetedByLeftClick(creditsArea))
                    {
                        PlayMenuSound(SoundHandler.MENU_BUTTON_SELECT);
                        m_showCredits = true;
                    }
                }
                else if (m_inputHandler.MouseIsOver(exitArea))
                {
                    DrawButton(BUTTONS_EXIT_SELECTED, new Vector2(750, 473), new Point(512, 100), 0.5f);
                    PlayMenuSound(SoundHandler.MENU_BUTTON_HOVER);

                    if (m_inputHandler.DidGetTargetedByLeftClick(exitArea))
                    {
                        m_didPressExit = true;
                    }
                }
                else
                    m_didHover = false;

                #endregion
            }
            else if (m_showOptions)
            {
                DrawOptionScreen(new Vector2(727, 0));
            }
            else
            {
                DrawCreditsScreen();
            }
        }

        //Metod för utritning av alternativ skärm
        internal void DrawOptionScreen(Vector2 a_position)
        {
            DrawImage(SCREEN_OPTION, a_position, new Point(300, 720), 1.0f);

            #region Graphics
            //Grafik knappar
            Rectangle defaultGraphics = new Rectangle((int)a_position.X + 17, 192, 22, 20);
            Rectangle fullscreenGraphics = new Rectangle((int)a_position.X + 17, 211, 22, 20);

            if (m_inputHandler.DidGetTargetedByLeftClick(defaultGraphics))
            {
                m_fullScreen = false;
            }
            if (m_inputHandler.DidGetTargetedByLeftClick(fullscreenGraphics))
            {
                m_fullScreen = true;
            }
            if (!m_fullScreen)
            {
                DrawButton(BUTTONS_CHECK_BOX, new Vector2((int)a_position.X + 17, 192), new Point(22, 20), 1f);
            }
            else
            {
                DrawButton(BUTTONS_CHECK_BOX, new Vector2((int)a_position.X + 17, 211), new Point(22, 20), 1f);
            } 
            #endregion

            #region Sound
            Rectangle disableSound = new Rectangle((int)a_position.X + 17, 263, 22, 20);
            Rectangle disableMusic = new Rectangle((int)a_position.X + 17, 281, 22, 20);


            if (m_inputHandler.DidGetTargetedByLeftClick(disableSound) && !m_inputHandler.SoundDisabled)
            {
                m_inputHandler.SoundDisabled = true;
            }
            else if (m_inputHandler.DidGetTargetedByLeftClick(disableSound) && m_inputHandler.SoundDisabled)
            {
                m_inputHandler.SoundDisabled = false;
            }
            if (m_inputHandler.DidGetTargetedByLeftClick(disableMusic) && !m_inputHandler.MusicDisabled)
            {
                m_inputHandler.MusicDisabled = true;
            }
            else if (m_inputHandler.DidGetTargetedByLeftClick(disableMusic) && m_inputHandler.MusicDisabled)
            {
                m_inputHandler.MusicDisabled = false;
            }

            if (m_inputHandler.SoundDisabled)
            {
                DrawButton(BUTTONS_CHECK_BOX, new Vector2((int)a_position.X + 17, 263), new Point(22, 20), 1f);
            }

            if (m_inputHandler.MusicDisabled)
            {
                DrawButton(BUTTONS_CHECK_BOX, new Vector2((int)a_position.X + 17, 281), new Point(22, 20), 1f);
            } 
            #endregion

            #region OkButton
            Rectangle okButton = new Rectangle((int)a_position.X + 21, 550, 256, 30);
            DrawButton(BUTTONS_OK, new Vector2((int)a_position.X + 21, 550), new Point(512, 83), 0.5f);

            if (m_inputHandler.MouseIsOver(okButton))
            {
                DrawButton(BUTTONS_OK_SELECTED, new Vector2((int)a_position.X + 21, 544), new Point(512, 83), 0.5f);
                PlayMenuSound(SoundHandler.MENU_BUTTON_HOVER);

                if (m_inputHandler.DidGetTargetedByLeftClick(okButton))
                {
                    m_showOptions = false;
                    PlayMenuSound(SoundHandler.MENU_BUTTON_SELECT);
                }
            }
            else
                m_didHover = false;
            #endregion
        }

        //Metod för utritning av credit-skärm
        internal void DrawCreditsScreen()
        {
            //m_didHover = false;
            DrawImage(SCREEN_BORDER_BG, new Vector2(727, 0), new Point(300, 720), 1.0f);
            DrawImage(SCREEN_CREDITS, Vector2.Zero, new Point(1280, 720), 1.0f);

            #region OkButton
            Rectangle okButton = new Rectangle(750, 560, 256, 30);
            DrawButton(BUTTONS_OK, new Vector2(750, 560), new Point(512, 83), 0.5f);

            if (m_inputHandler.MouseIsOver(okButton))
            {
                DrawButton(BUTTONS_OK_SELECTED, new Vector2(750, 554), new Point(512, 83), 0.5f);
                PlayMenuSound(SoundHandler.MENU_BUTTON_HOVER);

                if (m_inputHandler.DidGetTargetedByLeftClick(okButton))
                {
                    m_showCredits = false;
                    PlayMenuSound(SoundHandler.MENU_BUTTON_SELECT);
                }
            }
            else
                m_didHover = false;
            #endregion
        }

        //Metod för utritning av skärm för klassval
        internal void DrawClassSelectionScreen(float a_elapsedTime)
        {
            PlayStartTheme();

            //Ritar bakgrund
            DrawImage(SCREEN_CLASS_SELECT, Vector2.Zero, new Point(1280, 720), 1f);

            //Positioner för animationer
            Vector2 templarPosition = new Vector2(300, 170);
            Vector2 prophetPosition = new Vector2(616, 170);
            Vector2 descendantPosition = new Vector2(916, 170);

            //Rektanglar för select-knappar
            Rectangle selectTemplar = new Rectangle(209, 600, 256, 50);
            Rectangle selectProphet = new Rectangle(0,0, 512, 100);
            Rectangle selectDescendant = new Rectangle(0, 0, 512, 100);

            #region Templar
            if (m_inputHandler.MouseIsOver(new Rectangle((int)templarPosition.X, (int)templarPosition.Y, 64, 64)) || m_showTemplarSelection)
            {
                if (m_showTemplarSelection)
                {
                    m_spriteBatch.Draw(m_screenTextures[SCREEN_SELECT_TEMPLAR], new Vector2(185, 0), new Rectangle(0, 0, 300, 720), Color.White, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
                    m_animationSys.UpdateAndDraw(a_elapsedTime, Color.White, templarPosition, AnimationSystem.MOVING_DOWN, AnimationSystem.CLASS_SCREEN_TEMPLAR);
                    m_spriteBatch.Draw(m_screenButtons[BUTTONS_SELECT], new Vector2(203, 601), selectProphet, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                    #region SelectButton
                    if (m_inputHandler.MouseIsOver(selectTemplar))
                    {
                        m_spriteBatch.Draw(m_screenButtons[BUTTONS_SELECT_SELECTED], new Vector2(209, 600), selectProphet, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                        PlayMenuSound(SoundHandler.MENU_BUTTON_HOVER);

                        if (m_inputHandler.DidGetTargetedByLeftClick(selectTemplar))
                        {
                            PlayMenuSound(SoundHandler.MENU_BUTTON_SELECT_B);
                            m_didChooseClass = true;
                        }
                    }
                    else
                        m_didHover = false; 
                    #endregion
                }
                else
                {
                    m_animationSys.UpdateAndDraw(a_elapsedTime, Color.White, templarPosition, AnimationSystem.FACING_CAMERA, AnimationSystem.CLASS_SCREEN_TEMPLAR);
                }

                if (m_inputHandler.DidGetTargetedByLeftClick(new Rectangle((int)templarPosition.X, (int)templarPosition.Y, 64, 64)))
                {
                    PlayMenuSound(SoundHandler.MENU_BUTTON_SELECT);
                    m_showTemplarSelection = true;
                    m_showProphetSelection = false;
                    m_showDescendantSelection = false;
                }

            }
            else if (!m_showTemplarSelection)
            {
                m_animationSys.UpdateAndDraw(a_elapsedTime, Color.White, templarPosition, AnimationSystem.FACING_AWAY, AnimationSystem.CLASS_SCREEN_TEMPLAR);
            }

            #endregion

            #region Prophet
            if (m_inputHandler.MouseIsOver(new Rectangle((int)prophetPosition.X, (int)prophetPosition.Y, 64, 64)) || m_showProphetSelection)
            {
                if (m_showProphetSelection)
                {
                    m_spriteBatch.Draw(m_screenTextures[SCREEN_SELECT_PROPHET], new Vector2(501, 0), new Rectangle(0, 0, 300, 720), Color.White, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
                    m_animationSys.UpdateAndDraw(a_elapsedTime, Color.White, prophetPosition, AnimationSystem.MOVING_DOWN, AnimationSystem.CLASS_SCREEN_PROPHET);

                    m_spriteBatch.Draw(m_screenButtons[BUTTONS_SELECT_GRAY], new Vector2(525, 600), selectProphet, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                }
                else
                {
                    m_animationSys.UpdateAndDraw(a_elapsedTime, Color.White, prophetPosition, AnimationSystem.FACING_CAMERA, AnimationSystem.CLASS_SCREEN_PROPHET);
                }

                if (m_inputHandler.DidGetTargetedByLeftClick(new Rectangle((int)prophetPosition.X, (int)prophetPosition.Y, 64, 64)))
                {
                    PlayMenuSound(SoundHandler.MENU_BUTTON_SELECT);
                    m_showTemplarSelection = false;
                    m_showProphetSelection = true;
                    m_showDescendantSelection = false;
                }
            }
            else if (!m_showProphetSelection)
            {
                m_animationSys.UpdateAndDraw(a_elapsedTime, Color.White, prophetPosition, AnimationSystem.FACING_AWAY, AnimationSystem.CLASS_SCREEN_PROPHET);
            } 
            #endregion

            #region Descendant
            if (m_inputHandler.MouseIsOver(new Rectangle((int)descendantPosition.X, (int)descendantPosition.Y, 64, 64)) || m_showDescendantSelection)
            {
                if (m_showDescendantSelection)
                {
                    m_spriteBatch.Draw(m_screenTextures[SCREEN_SELECT_DESCENDANT], new Vector2(801, 0), new Rectangle(0, 0, 300, 720), Color.White, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
                    m_animationSys.UpdateAndDraw(a_elapsedTime, Color.White, descendantPosition, AnimationSystem.MOVING_DOWN, AnimationSystem.CLASS_SCREEN_DESCENDANT);
                    m_spriteBatch.Draw(m_screenButtons[BUTTONS_SELECT_GRAY], new Vector2(825, 600), selectProphet, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                }
                else
                {
                    m_animationSys.UpdateAndDraw(a_elapsedTime, Color.White, descendantPosition, AnimationSystem.FACING_CAMERA, AnimationSystem.CLASS_SCREEN_DESCENDANT);
                }

                if (m_inputHandler.DidGetTargetedByLeftClick(new Rectangle((int)descendantPosition.X, (int)descendantPosition.Y, 64, 64)))
                {
                    PlayMenuSound(SoundHandler.MENU_BUTTON_SELECT);
                    m_showTemplarSelection = false;
                    m_showProphetSelection = false;
                    m_showDescendantSelection = true;
                }
            }
            else if (!m_showDescendantSelection)
            {
                m_animationSys.UpdateAndDraw(a_elapsedTime, Color.White, descendantPosition, AnimationSystem.FACING_AWAY, AnimationSystem.CLASS_SCREEN_DESCENDANT);
            } 
            #endregion


        }

        //Metod för utritning av pause skärm
        internal void DrawPauseScreen(float a_elapsedTime)
        {
            Rectangle optionsArea = new Rectangle(850, 325, 250, 30);
            Rectangle quitArea = new Rectangle(850, 364, 250, 30);

            DrawImage(SCREEN_TRANSP_BG, new Vector2(0, 0), new Point(1280, 720),1.0f);

            if (!m_showOptions)
            {
                DrawImage(SCREEN_PAUSE_BG, new Vector2(827, 0), new Point(300, 720), 1.0f);

                DrawButton(BUTTONS_PAUSE_MENU, new Vector2(850, 250), new Point(512, 330), 0.5f);

                if (m_inputHandler.MouseIsOver(optionsArea))
                {
                    DrawButton(BUTTONS_OPTIONS_SELECTED, new Vector2(850, 321), new Point(512, 100), 0.5f);
                    PlayMenuSound(SoundHandler.MENU_BUTTON_HOVER);

                    if (m_inputHandler.DidGetTargetedByLeftClick(optionsArea))
                    {
                        PlayMenuSound(SoundHandler.MENU_BUTTON_SELECT);
                        m_showOptions = true;
                    }
                }
                else if (m_inputHandler.MouseIsOver(quitArea))
                {
                    DrawButton(BUTTONS_QUIT_SELECTED, new Vector2(850, 359), new Point(512, 100), 0.5f);
                    PlayMenuSound(SoundHandler.MENU_BUTTON_HOVER);

                    if (m_inputHandler.DidGetTargetedByLeftClick(quitArea))
                    {
                        m_didPressQuit = true;
                    }
                    else
                        m_didPressQuit = false;
                }
                else
                    m_didHover = false;
            }
            else
            {
                DrawOptionScreen(new Vector2(827, 0));
            }
        }

        //Metod för utritning av slut-skärm
        internal void DrawGameOverScreen(float a_elapsedTime)
        {
            if (!m_showCredits)
            {
                //Ritar endmeny
                DrawImage(SCREEN_END_BG, new Vector2(0, 0), new Point(1280, 720), 1.0f);
                DrawImage(SCREEN_BORDER_BG, new Vector2(727, 0), new Point(300, 720), 1.0f);
                m_spriteBatch.Draw(m_screenButtons[BUTTONS_START_MENU], new Vector2(750, 440), new Rectangle(0, 230, 512, 250), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

                //Skapar knapp area för startmeny
                Rectangle creditsArea = new Rectangle(750, 450, 250, 30);
                Rectangle exitArea = new Rectangle(750, 485, 250, 30);

                #region Inputhantering för endmeny

                if (m_inputHandler.MouseIsOver(creditsArea))
                {
                    DrawButton(BUTTONS_CREDITS_SELECTED, new Vector2(750, 434), new Point(512, 100), 0.5f);
                    PlayMenuSound(SoundHandler.MENU_BUTTON_HOVER);

                    if (m_inputHandler.DidGetTargetedByLeftClick(creditsArea))
                    {
                        m_showCredits = true;
                    }
                }
                else if (m_inputHandler.MouseIsOver(exitArea))
                {
                    DrawButton(BUTTONS_EXIT_SELECTED, new Vector2(750, 473), new Point(512, 100), 0.5f);
                    PlayMenuSound(SoundHandler.MENU_BUTTON_HOVER);

                    if (m_inputHandler.DidGetTargetedByLeftClick(exitArea))
                    {
                        m_didPressExit = true;
                    }
                }
                else
                    m_didHover = false;

                #endregion
            }
            else
                DrawCreditsScreen();
        }

        //Metod för utritning knappar
        internal void DrawButton(int a_texture, Vector2 a_pos, Point a_size, float a_scale)
        {
            m_spriteBatch.Draw(m_screenButtons[a_texture], a_pos, new Rectangle(0, 0, a_size.X, a_size.Y), Color.White, 0f, Vector2.Zero, a_scale, SpriteEffects.None, 0f);
        }

        //Metod för utritning av bilder
        internal void DrawImage(int a_texture, Vector2 a_pos, Point a_size, float a_scale)
        {
            m_spriteBatch.Draw(m_screenTextures[a_texture], a_pos, new Rectangle(0, 0, a_size.X, a_size.Y), Color.White, 0f, Vector2.Zero, a_scale, SpriteEffects.None, 0f);
        }

        //Metod för utritning av muspekare
        internal void DrawMouse()
        {
            Vector2 mousePosition = new Vector2(m_inputHandler.GetMouseState().X, m_inputHandler.GetMouseState().Y);

            if (!m_inputHandler.MouseIsOverEnemy)
            {
                if (m_inputHandler.LeftButtonIsDown())
                {
                    m_spriteBatch.Draw(m_mouseTextures[1], mousePosition, Color.White);
                }
                else
                {
                    m_spriteBatch.Draw(m_mouseTextures[0], mousePosition, Color.White);
                }
            }
        }

        #region Get/Set/Booleans

        internal bool PressedAndReleasedEsc()
        {
            return m_inputHandler.PressedAndReleasedEsc();
        }

        internal bool DidPressNewGame
        {
            get { return m_didPressNewGame; }
        }

        internal bool DidPressQuit
        {
            get { return m_didPressQuit; }
            set { m_didPressQuit = value; }
        }

        internal bool DidPressExit
        {
            get { return m_didPressExit; }
        }

        internal bool DidChooseClass
        {
            get { return m_didChooseClass; }
        }

        internal bool DidPressOptions
        {
            get { return m_showOptions; }
            set { m_showOptions = value; }
        }

        internal bool DidShowCredits
        {
            get { return m_showCredits; }
            set { m_showCredits = value; }
        }

        internal bool FullScreen
        {
            get { return m_fullScreen; }
            set { m_fullScreen = value; }
        } 

        #endregion
    }
}
