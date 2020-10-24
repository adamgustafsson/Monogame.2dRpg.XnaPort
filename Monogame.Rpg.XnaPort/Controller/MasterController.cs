using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework.Media;

namespace Controller
{
    #region TODO
    /// <summary>
    /// TODO: Grafik: Sound-system, soundeffects på knapp tryckningar, fights osv
    /// </summary> 
    #endregion
    /// <summary>
    /// Huvud klass för hantering och körning av spelets olika komponenter
    /// </summary>
    public class MasterController : Microsoft.Xna.Framework.Game
    {
        #region Variabler

        private SpriteBatch m_spriteBatch;
        private GraphicsDeviceManager m_graphics;

        private GameController m_gameController;
        private ScreenController m_screenController;

        private View.InputHandler m_inputHandler;
        private View.AnimationSystem m_animationSystem;
        private View.SoundHandler m_soundHandler;
        private Model.GameModel m_gameModel; 

        #endregion

        public MasterController()
        {
            this.m_graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.m_graphics.PreferredBackBufferHeight = 720;
            this.m_graphics.PreferredBackBufferWidth = 1280;
            this.m_graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            m_spriteBatch = new SpriteBatch(GraphicsDevice);
            m_gameModel = new Model.GameModel(Content);
            
            //Klasser som medskickas till samtliga controller-klasser
            m_inputHandler = new View.InputHandler();
            m_animationSystem = new View.AnimationSystem(m_spriteBatch);
            m_soundHandler = new View.SoundHandler(m_inputHandler);
            m_soundHandler.LoadContent(Content);

            //Controllers
            m_screenController = new ScreenController(m_gameModel, m_spriteBatch, m_animationSystem, m_inputHandler, m_soundHandler);
            m_gameController = new GameController(GraphicsDevice, m_spriteBatch, m_gameModel, m_animationSystem, m_inputHandler, m_soundHandler);

            m_gameController.LoadContent(Content);
            m_screenController.LoadScreenContent(Content);

            //Initsierar renderingsobjekt för TMX filer(map-filer)
            Map.InitObjectDrawing(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            m_soundHandler = null;
            MediaPlayer.Stop();
            this.Content.Unload();
        }

        protected override void Update(GameTime a_gameTime)
        {
            //Uppdaterar keyboar & mousestate
            m_inputHandler.SetKeyboardState();
            m_inputHandler.SetMouseState();
            
            //Uppdaterar ScreenController
            m_screenController.UpdateScreenSimulation((float)a_gameTime.ElapsedGameTime.TotalSeconds);

            if (m_screenController.DoQuit)
            {
                UnloadContent();
                LoadContent();
            }

            if (m_screenController.DoExit)
                this.Exit();

            //Om anv begär fullskärm
            if (m_screenController.FullScreen != m_graphics.IsFullScreen && !m_graphics.IsFullScreen)
            {
                m_graphics.PreferMultiSampling = false;
                m_graphics.IsFullScreen = true;
                m_graphics.ApplyChanges();
            }
            else if (m_screenController.FullScreen != m_graphics.IsFullScreen && m_graphics.IsFullScreen)
            {
                m_graphics.IsFullScreen = false;
                m_graphics.ApplyChanges();
            }
            //Uppdaterar spelmotorn via GameController om ingen extern skärm skall visas
            if (!m_screenController.IsShowingExternalScreen() && !m_screenController.IsShowingPauseScreen)
            {
                m_gameController.UpdateSimulation((float)a_gameTime.ElapsedGameTime.TotalSeconds);
            }

            base.Update(a_gameTime);
        }

        protected override void Draw(GameTime a_gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (m_screenController.IsShowingExternalScreen())
                m_screenController.DrawScreens((float)a_gameTime.ElapsedGameTime.TotalSeconds);
            //Ritar spelet via GameController om ingen extern skärm skall visas
            else
            {
                m_gameController.Draw((float)a_gameTime.ElapsedGameTime.TotalSeconds);

                //Pause skärm ritas över spelet vars motor är pausad
                if(m_screenController.IsShowingPauseScreen)
                    m_screenController.DrawScreens((float)a_gameTime.ElapsedGameTime.TotalSeconds);
            }

            base.Draw(a_gameTime);
        }
    }
}