using System.Collections.Generic;
using FBLibrary.Core;
using FBLibrary.Core.BaseEntities;
using FBClient.Core.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FBClient.TileEngine;

namespace FBClient.WorldEngine
{
    public class Map : BaseMap
    {
        #region Field Region

        // Textures
        Texture2D _mapTexture;
        Texture2D _wallTexture;

        private TileMap _tileMap;

        private readonly List<EdgeWall> _edgeWallList;
        private readonly List<UnbreakableWall> _unbreakableWallList;
        private List<Teleporter> _teleporterList;
        private readonly List<Arrow> _arrowList;

        #endregion

        #region Property Region

        public TileMap TileMap
        {
            get { return _tileMap; }
        }

        public List<Teleporter> TeleporterList
        {
            get { return _teleporterList; }
            set { _teleporterList = value; }
        }

        #endregion

        #region Constructor Region

        public Map()
        {
            _edgeWallList = new List<EdgeWall>();
            _unbreakableWallList = new List<UnbreakableWall>();
            _teleporterList = new List<Teleporter>();
            _arrowList = new List<Arrow>();
        }

        #endregion

        #region XNA Method Region

        public void LoadContent()
        {
            _wallTexture = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Sprites/edgeWall");
            _mapTexture = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Tilesets/tileset1");

            var tilesets = new List<Tileset>() { new Tileset(_mapTexture, 64, 32, 32, 32) };
            var layer = new MapLayer(Size.X, Size.Y);
            var mapLayers = new List<MapLayer> { layer };
            _tileMap = new TileMap(tilesets, mapLayers);
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera2D camera)
        {
            // Background
            var position = new Vector2(
                Engine.Origin.X - (int)(Engine.Origin.X / Engine.TileWidth) * Engine.TileWidth - Engine.TileWidth,
                Engine.Origin.Y - (int)(Engine.Origin.Y / Engine.TileHeight) * Engine.TileHeight - Engine.TileHeight);

            // Draw additional unbreakable walls to fill the resolution
            for (int i = 0; i < (FinalBomber.Instance.GraphicsDevice.Viewport.Width / Engine.TileWidth) + 2; i++)
            {
                for (int j = 0; j < (FinalBomber.Instance.GraphicsDevice.Viewport.Height / Engine.TileHeight) + 2; j++)
                {
                    if (!((position.X + i * Engine.TileWidth > Engine.Origin.X &&
                        position.X + i * Engine.TileWidth < Engine.Origin.X + Size.X * Engine.TileWidth - Engine.TileWidth) &&
                        (position.Y + j * Engine.TileHeight > Engine.Origin.Y &&
                        position.Y + j * Engine.TileHeight < Engine.Origin.Y + Size.Y * Engine.TileHeight - Engine.TileHeight)))
                    {
                        spriteBatch.Draw(_wallTexture, new Vector2(position.X + (i * Engine.TileWidth), position.Y + (j * Engine.TileHeight)), Color.White);
                    }
                }
            }

            if (_tileMap != null)
            _tileMap.Draw(spriteBatch, camera, CollisionLayer);

            // Draw entities
            foreach (var edgeWall in _edgeWallList)
                edgeWall.Draw(gameTime);

            foreach (var unbreakableWall in _unbreakableWallList)
                unbreakableWall.Draw(gameTime);

            foreach (var teleporter in TeleporterList)
                teleporter.Draw(gameTime);

            foreach (var arrow in _arrowList)
                arrow.Draw(gameTime);
        }

        protected override void AddUnbreakableWall(Point position)
        {
            var unbreakableWall = new UnbreakableWall(position);
            _unbreakableWallList.Add(unbreakableWall);

            base.AddUnbreakableWall(unbreakableWall);
        }

        protected override void AddEdgeWall(Point position)
        {
            var edgeWall = new EdgeWall(position);
            _edgeWallList.Add(edgeWall);

            base.AddEdgeWall(edgeWall);
        }

        #endregion
    }
}
