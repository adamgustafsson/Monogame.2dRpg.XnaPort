using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FuncWorks.XNA.XTiled;

namespace View
{
    /// <summary>
    /// Klass för hantering och uppdatering av spelarkamera
    /// </summary>
    class Camera
    {
        //Variabler
        private Rectangle m_mapView;
        private Viewport m_viewPort;
        private Map m_currentMap;
        private Model.Player m_player;

        //Konstruktor
        public Camera(GraphicsDevice a_graphicsDevice, Model.GameModel a_gameModel)
        {
            this.m_viewPort = a_graphicsDevice.Viewport;
            this.m_mapView = a_graphicsDevice.Viewport.Bounds;
            this.m_currentMap = a_gameModel.m_currentMap;
            this.m_player = a_gameModel.m_playerSystem.m_player;
        }

        //Metod för uppdatering av kameraregler
        internal void UpdateCamera()
        {
            Rectangle delta = m_viewPort.Bounds;

            if (delta.X != m_player.ThisUnit.Bounds.Center.X && m_player.ThisUnit.Bounds.Center.X > m_viewPort.Width / 2 && m_player.ThisUnit.Bounds.Center.X < (m_currentMap.Bounds.Width - m_viewPort.Width / 2))
            {
                delta.X = m_player.ThisUnit.Bounds.Center.X - m_viewPort.Width / 2;
            }
            if (delta.Y != m_player.ThisUnit.Bounds.Center.Y && m_player.ThisUnit.Bounds.Center.Y > m_viewPort.Height / 2 && m_player.ThisUnit.Bounds.Center.Y < (m_currentMap.Bounds.Height - m_viewPort.Height / 2))
            {
                delta.Y = m_player.ThisUnit.Bounds.Center.Y - m_viewPort.Height / 2;
            }
            if (m_currentMap.Bounds.Contains(delta))
            {
                m_mapView = delta;
            }
        }

        //Metod för visualisering av map-koordinater
        internal Vector2 VisualizeCordinates(int xCordinate, int yCordinate)
        {
            return new Vector2(xCordinate - m_mapView.X, yCordinate - m_mapView.Y);
        }

        //Metod för logistering av skärm koordinater
        internal Vector2 LogicalizeCordinates(int xCordinate, int yCordinate)
        {
            return new Vector2(xCordinate + m_mapView.X, yCordinate + m_mapView.Y);
        }

        //Metod för visualisering av map-rektanglar
        internal Rectangle VisualizeRectangle(Rectangle a_mapObject)
        {
            Vector2 cords = VisualizeCordinates(a_mapObject.X, a_mapObject.Y);
            return new Rectangle((int)cords.X, (int)cords.Y, a_mapObject.Width, a_mapObject.Height);
        }

        //Retunerar spelarkamerans aktuella koordenater
        public Rectangle GetScreenRectangle
        {
            get { return m_mapView; }
        }
    }
}
