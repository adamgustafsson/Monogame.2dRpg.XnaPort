using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Model
{
    class Level
    {
        /* AssignLayerIndex - Denna metoden måste köras före AssignObjectLayers (Bör vara det första som görs direkt efter att man IndexLevel++)
         * AssignObjectLayers - Ska köras efter AssignLayerIndexes
         * LoadMaps - Laddar alla banor/världar och lägger till dem i listan
         * IndexLevel - Indexet i Map-listan på banan/världen som körs för tillfället.
         */

        //Index's tillhörande lager
        public int IndexBackgroundLayerOne;
        public int IndexBackgroundLayerTwo;
        public int IndexBackgroundLayerThree;
        public int IndexForeground;
        public int IndexInteraction;
        public int IndexCollision;
        public int IndexFriendlyNPC;
        public int IndexEnemyNPC;
        public int IndexPlayer;
        public int IndexItems;
        public int IndexEnemyZone;
        public int IndexGraveyard;
        public int IndexZones;
          
        //Objektlager
        private ObjectLayer m_backgroundLayer;
        private ObjectLayer m_foregroundLayer;
        private ObjectLayer m_interactionLayer;
        private ObjectLayer m_collisionLayer;
        private ObjectLayer m_friendlyNPCLayer;
        private ObjectLayer m_enemyNPCLayer;
        private ObjectLayer m_playerLayer;
        private ObjectLayer m_itemLayer;
        private ObjectLayer m_enemyZoneLayer;
        private ObjectLayer m_graveyardLayer;
        private ObjectLayer m_zoneLayer;
        
        public bool foregroundVisible = true;

        public Level(ContentManager a_content)
        {
            m_mapList = new List<Map>();
            LoadMaps(a_content);
            AssignObjectLayerIndexes();
            AssignObjectLayers();
            AssignTileLayerIndexes();
        }

        #region Objektlager Get/Set

            public ObjectLayer ZoneLayer
            {
                get { return m_zoneLayer; }
                set { m_zoneLayer = value; }
            }    

            public ObjectLayer EnemyZoneLayer
            {
                get { return m_enemyZoneLayer; }
                set { m_enemyZoneLayer = value; }
            }
            public ObjectLayer BackgroundLayer
            {
                get { return m_backgroundLayer; }
                set { m_backgroundLayer = value; }
            }
            public ObjectLayer ForegroundLayer
            {
                get { return m_foregroundLayer; }
                set { m_foregroundLayer = value; }
            }
            public ObjectLayer InteractionLayer
            {
                get { return m_interactionLayer; }
                set { m_interactionLayer = value; }
            }
            public ObjectLayer CollisionLayer
            {
                get { return m_collisionLayer; }
                set { m_collisionLayer = value; }
            }
            public ObjectLayer FriendlyNPCLayer
            {
                get { return m_friendlyNPCLayer; }
                set { m_friendlyNPCLayer = value; }
            }
            public ObjectLayer EnemyNPCLayer
            {
                get { return m_enemyNPCLayer; }
                set { m_enemyNPCLayer = value; }
            }
            public ObjectLayer PlayerLayer
            {
                get { return m_playerLayer; }
                set { m_playerLayer = value; }
            }
            public ObjectLayer ItemLayer
            {
                get { return m_itemLayer; }
                set { m_itemLayer = value; }
            }
            public ObjectLayer GraveyardLayer
            {
                get { return m_graveyardLayer; }
                set { m_graveyardLayer = value; }
            }
        #endregion

        //Lista innehållande världar
        private List<Map> m_mapList;

        //Aktiv värld
        private int m_indexLevel;
        
        //index på aktuell bana i listan
        public int IndexLevel
        {
            get { return m_indexLevel; }
            set { m_indexLevel = value; }
        }

        //Retunerar aktuell bana
        public Map CurrentMap()
        {
            return m_mapList[m_indexLevel];
        }

        //Laddar all världar
        public void LoadMaps(ContentManager a_content)
        {
            m_mapList.Add(TMXContentProcessor.LoadTMX("world.tmx", "TileTextures", a_content));
        }

        //Hämtar Index's till samtliga objektlager från TMX filen
        public void AssignObjectLayerIndexes()
        {
            for (int i = 0; i < m_mapList[IndexLevel].ObjectLayers.Count; i++)
            {
                switch (m_mapList[IndexLevel].ObjectLayers[i].Name)
                {
                    case "Interaction":
                        IndexInteraction = i;
                        break;
                    case "Collision":
                        IndexCollision = i;
                        break;
                    case "FriendlyNPC":
                        IndexFriendlyNPC = i;
                        break;
                    case "EnemyNPC":
                        IndexEnemyNPC = i;
                        break;
                    case "Player":
                        IndexPlayer = i;
                        break;
                    case "Items":
                        IndexItems = i;
                        break;
                    case "EnemyZone":
                        IndexEnemyZone = i;
                        break;
                    case "Graveyard":
                        IndexGraveyard = i;
                        break;
                    case "Zones":
                        IndexZones = i;
                        break;
                }
            }
        }

        //Hämtar Index's till samtliga tile-lager från TMX filen
        public void AssignTileLayerIndexes()
        {
            for (int i = 0; i < m_mapList[IndexLevel].TileLayers.Count; i++)
            {
                switch (m_mapList[IndexLevel].TileLayers[i].Name)
                {
                    case "BackgroundLayer1":
                        IndexBackgroundLayerOne = i;
                        break;
                    case "BackgroundLayer2":
                        IndexBackgroundLayerTwo = i;
                        break;
                    case "BackgroundLayer3":
                        IndexBackgroundLayerThree = i;
                        break;
                    case "Foreground":
                        IndexForeground = i;
                        break;
                }
            }
        }

        //Instancierar objekt med tillhörande lager
        public void AssignObjectLayers()
        {
            m_backgroundLayer = m_mapList[IndexLevel].ObjectLayers[IndexBackgroundLayerOne];
            m_foregroundLayer = m_mapList[IndexLevel].ObjectLayers[IndexForeground];
            m_interactionLayer = m_mapList[IndexLevel].ObjectLayers[IndexInteraction];
            m_collisionLayer = m_mapList[IndexLevel].ObjectLayers[IndexCollision];
            m_friendlyNPCLayer = m_mapList[IndexLevel].ObjectLayers[IndexFriendlyNPC];
            m_enemyNPCLayer = m_mapList[IndexLevel].ObjectLayers[IndexEnemyNPC];
            m_playerLayer = m_mapList[IndexLevel].ObjectLayers[IndexPlayer];
            m_itemLayer = m_mapList[IndexLevel].ObjectLayers[IndexItems];
            m_enemyZoneLayer = m_mapList[IndexLevel].ObjectLayers[IndexEnemyZone];
            m_graveyardLayer = m_mapList[IndexLevel].ObjectLayers[IndexGraveyard];
            m_zoneLayer = m_mapList[IndexLevel].ObjectLayers[IndexZones];
        }

    }
}
