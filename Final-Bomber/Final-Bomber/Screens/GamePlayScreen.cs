using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FBLibrary;
using Final_Bomber.Core;
using Final_Bomber.Core.Entities;
using Final_Bomber.Core.Players;
using Final_Bomber.Entities.AI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;

using Final_Bomber.Controls;
using Final_Bomber.Entities;
using Final_Bomber.TileEngine;
using Final_Bomber.WorldEngine;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using FBLibrary.Core;

namespace Final_Bomber.Screens
{
    public class GamePlayScreen : BaseGameState
    {
        #region Field Region
        private bool _pause;

        Engine _engine;

        // Map
        Point _mapSize;
        Texture2D _mapTexture;

        // List
        private List<Wall> _wallList;
        private List<PowerUp> _itemList;
        private List<EdgeWall> _edgeWallList;

        // Sound Effects & Musics
        Song[] _mapSong;
        Song[] _mapSongHurry;
        int _songNumber;

        // HUD
        Texture2D _itemInfoIcon;
        TimeSpan _timer;
        SpriteFont _gameFont;
        SpriteFont _smallFont;
        Point _hudOrigin;
        int _hudTopSpace;
        int _hudMarginLeft;
        Texture2D _cross;
        Texture2D _badItemTimerBar;

        // Window box
        Texture2D _windowSkin;
        WindowBox _scoresWindowBox;
        WindowBox _timerWindowBox;

        Texture2D _wallTexture;

        // Dead players number
        int _deadPlayersNumber;

        // FPS => Performance
        float _elapseTime;
        int _frameCounter;
        int _fps;

        // Random

        #endregion

        #region Property Region

        public World World { get; set; }

        public List<Player> PlayerList { get; private set; }

        public List<Bomb> BombList { get; private set; }

        public List<UnbreakableWall> UnbreakableWallList { get; private set; }

        public List<Teleporter> TeleporterList { get; private set; }

        private List<Arrow> ArrowList { get; set; }

        // Sudden Death
        public SuddenDeath SuddenDeath { get; private set; }

        // Sound Effects
        public Song MapSongHurry
        {
            get { return _mapSongHurry[_songNumber]; }
            set { _mapSongHurry[_songNumber] = value; }
        }

        public SoundEffect BombExplosionSound { get; private set; }

        public SoundEffect ItemPickUpSound { get; private set; }

        public SoundEffect PlayerDeathSound { get; private set; }

        // Random
        public static Random Random { get; private set; }

        #endregion

        #region Constructor Region

        public GamePlayScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
        }

        #endregion

        #region XNA Method Region

        public override void Initialize()
        {
            Random = new Random();

            // FPS
            _elapseTime = 0f;
            _frameCounter = 0;
            _fps = 0;
            _pause = false;
            _hudOrigin = new Point(FinalBomber.Instance.GraphicsDevice.Viewport.Width - 234, 0);
            _hudTopSpace = 15;
            _hudMarginLeft = 15;

            base.Initialize();
            Reset();
            _scoresWindowBox = new WindowBox(_windowSkin, new Vector2(_hudOrigin.X, _hudOrigin.Y),
                new Point(GraphicsDevice.Viewport.Width - (_hudOrigin.X),
                    _hudTopSpace + Config.PlayersNumber * Config.HUDPlayerInfoSpace + 15));

            _timerWindowBox = new WindowBox(_windowSkin, new Vector2(_hudOrigin.X, _scoresWindowBox.Size.Y),
                new Point(GraphicsDevice.Viewport.Width - _hudOrigin.X, 40));
        }

        protected override void LoadContent()
        {
            // Pictures      
            _mapTexture = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Tilesets/tileset1");
            _itemInfoIcon = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Pictures/ItemInfo");
            _cross = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Pictures/Cross");
            _badItemTimerBar = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Pictures/BadItemTimerCross");
            _wallTexture = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Characters/edgeWall");
            _windowSkin = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Windowskins/Windowskin1");

            // Sound Effects & Musics
            _mapSong = new Song[]
            {
                FinalBomber.Instance.Content.Load<Song>("Audio/Musics/map1")
            };
            _mapSongHurry = new Song[]
            {
                FinalBomber.Instance.Content.Load<Song>("Audio/Musics/map1_hurry")
            };
            BombExplosionSound = FinalBomber.Instance.Content.Load<SoundEffect>("Audio/Sounds/boom");
            ItemPickUpSound = FinalBomber.Instance.Content.Load<SoundEffect>("Audio/Sounds/item");
            PlayerDeathSound = FinalBomber.Instance.Content.Load<SoundEffect>("Audio/Sounds/playerDeath");

            // Fonts
            _gameFont = FinalBomber.Instance.Content.Load<SpriteFont>("Graphics/Fonts/GameFont");
            _smallFont = FinalBomber.Instance.Content.Load<SpriteFont>("Graphics/Fonts/SmallFont");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            /*
            if (!_pause)
            {
                #region FPS
                _elapseTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                _frameCounter++;

                if (_elapseTime >= 1)
                {
                    _fps = _frameCounter;
                    _frameCounter = 0;
                    _elapseTime = 0;
                }
                #endregion

                _timer += gameTime.ElapsedGameTime;

                #region Controls
                ControlManager.Update(gameTime, PlayerIndex.One);

                if (InputHandler.KeyPressed(Keys.P))
                {
                    MediaPlayer.Pause();
                    _pause = true;
                }

                if (InputHandler.KeyDown(Keys.Escape))
                {
                    MediaPlayer.Stop();
                    StateManager.ChangeState(FinalBomber.Instance.BattleMenuScreen);
                }

                if (Config.Debug)
                {
                    if (InputHandler.KeyPressed(Keys.R))
                        Reset();
                    else if (InputHandler.KeyPressed(Keys.A))
                    {
                        foreach (Wall w in _wallList)
                            w.Destroy();
                    }
                    else if (InputHandler.KeyPressed(Keys.I))
                    {
                        foreach (PowerUp i in _itemList)
                            i.Destroy();
                    }
                    else if (InputHandler.KeyDown(Keys.B))
                    {
                        var emptyCells = World.Levels[World.CurrentLevel].FindEmptyCells();

                        if (emptyCells.Count > 0)
                        {
                            int randomPos = Random.Next(0, emptyCells.Count - 1);
                            var pos = emptyCells[randomPos];
                            var bomb = new Bomb(-42, pos, Random.Next(Config.MinBombPower, Config.MaxBombPower), Config.BaseBombTimer, 0f);
                            BombList.Add(bomb);
                            World.Levels[World.CurrentLevel].Board[pos.X, pos.Y] = bomb;
                            World.Levels[World.CurrentLevel].CollisionLayer[pos.X, pos.Y] = true;
                        }
                    }

                    else if (InputHandler.KeyDown(Keys.F1))
                    {
                        PlayerList[0].IncreasePower(1);
                        PlayerList[0].IncreaseTotalBombNumber(1);
                        PlayerList[0].IncreaseSpeed(Config.PlayerSpeedIncrementeur);
                    }
                    else if (InputHandler.KeyDown(Keys.F2))
                    {
                        PlayerList[0].IncreasePower(-1);
                        PlayerList[0].IncreaseTotalBombNumber(-1);
                        PlayerList[0].IncreaseSpeed(-Config.PlayerSpeedIncrementeur);
                    }

                }
                #endregion

                #region Walls

                for (int i = 0; i < _wallList.Count; i++)
                {
                    _wallList[i].Update(gameTime);

                    // Is it die ?
                    if (World.Levels[World.CurrentLevel].
                        HazardMap[_wallList[i].CellPosition.X, _wallList[i].CellPosition.Y] == 3)
                    {
                        World.Levels[World.CurrentLevel].
                        HazardMap[_wallList[i].CellPosition.X, _wallList[i].CellPosition.Y] = 0;
                        _wallList[i].Destroy();
                    }

                    // We clean the obsolete elements
                    if (!_wallList[i].IsAlive)
                    {
                        if (!Config.ActiveSuddenDeath ||
                            (Config.ActiveSuddenDeath && !SuddenDeath.Visited[_wallList[i].CellPosition.X, _wallList[i].CellPosition.Y]))
                        {
                            World.Levels[World.CurrentLevel].CollisionLayer[_wallList[i].CellPosition.X, _wallList[i].CellPosition.Y] = false;
                            if (Random.Next(0, 100) < MathHelper.Clamp(Config.ItemNumber, 0, 100))
                            {
                                var item = new PowerUp(_wallList[i].CellPosition);
                                _itemList.Add(item);
                                World.Levels[World.CurrentLevel].Board[_wallList[i].CellPosition.X, _wallList[i].CellPosition.Y] = item;
                            }
                            else
                            {
                                World.Levels[World.CurrentLevel].Board[_wallList[i].CellPosition.X, _wallList[i].CellPosition.Y] = null;
                            }
                        }
                        _wallList.Remove(_wallList[i]);
                    }
                }
                #endregion

                #region Bombs
                for (int i = 0; i < BombList.Count; i++)
                {
                    BombList[i].Update(gameTime);

                    // Is it die ?
                    if (World.Levels[World.CurrentLevel].
                        HazardMap[BombList[i].CellPosition.X, BombList[i].CellPosition.Y] == 3 &&
                        !BombList[i].InDestruction)
                    {
                        BombList[i].Destroy();
                    }

                    // We clean the obsolete elements
                    if (!BombList[i].IsAlive)
                    {
                        if (World.Levels[World.CurrentLevel].Board[BombList[i].CellPosition.X, BombList[i].CellPosition.Y] is Bomb)
                            World.Levels[World.CurrentLevel].Board[BombList[i].CellPosition.X, BombList[i].CellPosition.Y] = null;
                        World.Levels[World.CurrentLevel].CollisionLayer[BombList[i].CellPosition.X, BombList[i].CellPosition.Y] = false;

                        // We don't forget to give it back to its owner
                        if (BombList[i].Id > 0)
                        {
                            Player pl = PlayerList.Find(p => p.Id == BombList[i].Id);
                            if (pl != null && pl.CurrentBombAmount < pl.TotalBombAmount)
                                pl.CurrentBombAmount++;
                        }

                        // Update the hazard map
                        List<Bomb> bL = BombList.FindAll(b => !b.InDestruction);
                        foreach (Point p in BombList[i].ActionField)
                        {
                            bool sameCellThanAnOther = false;
                            if (BombList.Where(b => !(b.CellPosition.X == BombList[i].CellPosition.X &&
                                                      b.CellPosition.Y == BombList[i].CellPosition.Y)).Any(b => b.ActionField.Find(c => c.X == p.X && c.Y == p.Y) != Point.Zero))
                            {
                                this.World.Levels[this.World.CurrentLevel].HazardMap[p.X, p.Y] = 2;
                                sameCellThanAnOther = true;
                            }
                            if (!sameCellThanAnOther)
                                this.World.Levels[this.World.CurrentLevel].HazardMap[p.X, p.Y] = 0;
                        }
                        BombList.Remove(BombList[i]);
                    }
                }
                #endregion

                #region Items
                for (int i = 0; i < _itemList.Count; i++)
                {
                    _itemList[i].Update(gameTime);

                    // Is it die ?
                    if (World.Levels[World.CurrentLevel].
                        HazardMap[_itemList[i].CellPosition.X, _itemList[i].CellPosition.Y] == 3)
                        _itemList[i].Destroy();

                    // We clean the obsolete elements
                    if (!_itemList[i].IsAlive)
                    {
                        if (World.Levels[World.CurrentLevel].Board[_itemList[i].CellPosition.X, _itemList[i].CellPosition.Y] is PowerUp)
                            World.Levels[World.CurrentLevel].Board[_itemList[i].CellPosition.X, _itemList[i].CellPosition.Y] = null;
                        _itemList.Remove(_itemList[i]);
                    }
                }
                #endregion

                #region Player
                for (int i = 0; i < PlayerList.Count; i++)
                {
                    PlayerList[i].Update(gameTime);

                    // Is it die ?
                    if (!PlayerList[i].InDestruction && !PlayerList[i].IsInvincible && World.Levels[World.CurrentLevel].
                        HazardMap[PlayerList[i].CellPosition.X, PlayerList[i].CellPosition.Y] == 3)
                    {
                        int bombId = -42;
                        List<Bomb> bl = BombList.FindAll(b => b.InDestruction);
                        foreach (Bomb b in bl)
                        {
                            if (b.ActionField.Any(po => po == PlayerList[i].CellPosition))
                            {
                                bombId = b.Id;
                            }
                        }
                        // Suicide
                        if (bombId == PlayerList[i].Id)
                            Config.PlayersScores[bombId - 1]--;
                        else if (bombId >= 0 && bombId < Config.PlayersNumber)
                        {
                            Config.PlayersScores[bombId - 1]++;
                            Player player = PlayerList.Find(p => p.Id == bombId);
                            if (player.OnEdge)
                            {
                                player.Rebirth(PlayerList[i].Position);
                                _deadPlayersNumber--;
                            }
                        }
                        PlayerList[i].Destroy();
                    }

                    // We clean the obsolete players
                    if (!PlayerList[i].IsAlive)
                    {
                        if (!PlayerList[i].OnEdge)
                        {
                            if (World.Levels[World.CurrentLevel].Board[PlayerList[i].CellPosition.X, PlayerList[i].CellPosition.Y] is Player)
                            {
                                var p = (Player)World.Levels[World.CurrentLevel].Board[PlayerList[i].CellPosition.X, PlayerList[i].CellPosition.Y];
                                if (p.Id == PlayerList[i].Id)
                                    World.Levels[World.CurrentLevel].Board[PlayerList[i].CellPosition.X, PlayerList[i].CellPosition.Y] = null;
                            }
                            _deadPlayersNumber++;
                        }

                        if ((Config.ActiveSuddenDeath && SuddenDeath.HasStarted))
                            PlayerList.Remove(PlayerList[i]);
                        else
                            PlayerList[i].OnEdge = true;
                    }
                }
                #endregion

                #region Teleporters

                for (int i = 0; i < TeleporterList.Count; i++)
                {
                    TeleporterList[i].Update(gameTime);

                    if (!TeleporterList[i].IsAlive)
                    {
                        TeleporterList.Remove(TeleporterList[i]);
                    }
                }

                #endregion

                #region Arrows

                for (int i = 0; i < ArrowList.Count; i++)
                {
                    ArrowList[i].Update(gameTime);

                    if (!ArrowList[i].IsAlive)
                    {
                        ArrowList.Remove(ArrowList[i]);
                    }
                }

                #endregion

                #region Sudden Death
                // Sudden Death
                if (Config.ActiveSuddenDeath)
                    SuddenDeath.Update(gameTime);
                #endregion

                // World Update
                World.Update(gameTime);

                // End of the round
                if ((Config.PlayersNumber > 1 && _deadPlayersNumber >= Config.PlayersNumber - 1) ||
                    (Config.PlayersNumber == 1 && _deadPlayersNumber == 1))
                {
                    foreach (Player p in PlayerList)
                    {
                        if (p.IsAlive)
                            Config.PlayersScores[p.Id] += 5;
                    }
                    Reset();
                }

                base.Update(gameTime);
            }
            else if (InputHandler.KeyPressed(Keys.P))
            {
                MediaPlayer.Resume();
                _pause = false;
            }
            */
        }

        public override void Draw(GameTime gameTime)
        {
            if (SuddenDeath != null && SuddenDeath.HasStarted)
            {
                Matrix translate = Matrix.CreateTranslation(Functions.Shake(Random));

                // Displace everything that gets drawn in this batch 
                FinalBomber.Instance.SpriteBatch.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    null,
                    null,
                    null,
                    translate);
            }
            else
            {
                FinalBomber.Instance.SpriteBatch.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    null,
                    null,
                    null,
                    Matrix.Identity);
            }

            base.Draw(gameTime);

            ControlManager.Draw(FinalBomber.Instance.SpriteBatch);

            #region Debug Region
            if (Config.Debug && InputHandler.KeyDown(Keys.H))
            {
                for (int x = 0; x < World.Levels[World.CurrentLevel].Size.X; x++)
                {
                    for (int y = 0; y < World.Levels[World.CurrentLevel].Size.Y; y++)
                    {
                        /*
                        FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, World.Levels[World.CurrentLevel].HazardMap[x, y].ToString(),
                            new Vector2(x * 30, 80 + 20 * y), Color.Black);
                        */
                    }
                }
            }
            else if (Config.Debug && InputHandler.KeyDown(Keys.C))
            {
                for (int x = 0; x < World.Levels[World.CurrentLevel].Size.X; x++)
                {
                    for (int y = 0; y < World.Levels[World.CurrentLevel].Size.Y; y++)
                    {
                        if (!World.Levels[World.CurrentLevel].CollisionLayer[x, y])
                        {
                            FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, "0",
                                new Vector2(x * 20, 80 + 20 * y), Color.ForestGreen);
                        }
                        else
                        {
                            FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, "1",
                            new Vector2(x * 20, 80 + 20 * y), Color.Purple);
                        }
                    }
                }
            }
            else if (Config.Debug && InputHandler.KeyDown(Keys.O))
            {
                for (int x = 0; x < World.Levels[World.CurrentLevel].Size.X; x++)
                {
                    for (int y = 0; y < World.Levels[World.CurrentLevel].Size.Y; y++)
                    {
                        string itemMapType = "";
                        Color colorMapItem = Color.White;
                        if (World.Levels[World.CurrentLevel].Board[x, y] != null)
                        {
                            switch (World.Levels[World.CurrentLevel].Board[x, y].GetType().Name)
                            {
                                case "Wall":
                                    itemMapType = "W";
                                    colorMapItem = Color.Gray;
                                    break;
                                case "Bomb":
                                    itemMapType = "B";
                                    colorMapItem = Color.Red;
                                    break;
                                case "Player":
                                    itemMapType = "P";
                                    colorMapItem = Color.Green;
                                    break;
                                case "Item":
                                    itemMapType = "I";
                                    colorMapItem = Color.Yellow;
                                    break;
                                case "EdgeWall":
                                    itemMapType = "E";
                                    colorMapItem = Color.Black;
                                    break;
                                case "UnbreakableWall":
                                    itemMapType = "U";
                                    colorMapItem = Color.Black;
                                    break;
                                case "Teleporter":
                                    itemMapType = "T";
                                    colorMapItem = Color.Blue;
                                    break;
                                case "Arrow":
                                    itemMapType = "A";
                                    colorMapItem = Color.DarkViolet;
                                    break;
                            }
                        }
                        else
                            itemMapType = ".";

                        FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, itemMapType,
                            new Vector2(x * 20, 80 + 20 * y), colorMapItem);
                    }
                }
            }
            // Sudden Death
            else if (Config.Debug && InputHandler.KeyDown(Keys.S))
            {
                if (Config.ActiveSuddenDeath)
                {
                    for (int x = 0; x < World.Levels[World.CurrentLevel].Size.X; x++)
                    {
                        for (int y = 0; y < World.Levels[World.CurrentLevel].Size.Y; y++)
                        {
                            if (SuddenDeath != null && SuddenDeath.Visited[x, y])
                            {
                                FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, "1",
                                    new Vector2(x * 30, 80 + 20 * y), Color.Red);
                            }
                            else
                            {
                                FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, "0",
                                    new Vector2(x * 30, 80 + 20 * y), Color.Green);
                            }
                        }
                    }
                }
            }
            else if (Config.Debug && Config.IsThereAIPlayer && InputHandler.KeyDown(Keys.L))
            {
                /*
                int[,] costMatrix = AIFunction.CostMatrix(
                    PlayerList[PlayerList.Count - 1].CellPosition,
                    World.Levels[World.CurrentLevel].CollisionLayer,
                    World.Levels[World.CurrentLevel].HazardMap, Config.MapSize);

                for (int x = 0; x < costMatrix.GetLength(0); x++)
                {
                    for (int y = 0; y < costMatrix.GetLength(1); y++)
                    {
                        if (costMatrix[x, y] == _mapSize.X * _mapSize.Y)
                        {
                            FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, ".",
                                new Vector2(x * 30, 80 + 20 * y), Color.White);
                        }
                        else
                        {
                            FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, costMatrix[x, y].ToString(),
                                new Vector2(x * 30, 80 + 20 * y), Color.Red);
                        }
                    }
                }
                */
            }
            else if (Config.Debug && InputHandler.KeyDown(Keys.T))
            {
                /*
                int[,] interestMatrix = AIFunction.MakeInterestMatrix(
                    PlayerList[PlayerList.Count - 1].CellPosition,
                    World.Levels[World.CurrentLevel].Board,
                    World.Levels[World.CurrentLevel].CollisionLayer,
                    World.Levels[World.CurrentLevel].HazardMap, _mapSize);

                for (int x = 0; x < interestMatrix.GetLength(0); x++)
                {
                    for (int y = 0; y < interestMatrix.GetLength(1); y++)
                    {
                        if (interestMatrix[x, y] == 0)
                        {
                            FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, ".",
                                new Vector2(x * 30, 80 + 20 * y), Color.White);
                        }
                        else
                        {
                            FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, interestMatrix[x, y].ToString(),
                                new Vector2(x * 30, 80 + 20 * y), Color.Red);
                        }
                    }
                }
                */
            }
            #endregion
            else
            {
                #region Debug - Moving of the map
                if (Config.Debug && InputHandler.KeyDown(Keys.Tab))
                {
                    if (InputHandler.KeyDown(Keys.Up))
                        Engine.ChangeYOrigin(Engine.Origin.Y - Config.MapMovingScale);
                    if (InputHandler.KeyDown(Keys.Down))
                        Engine.ChangeYOrigin(Engine.Origin.Y + Config.MapMovingScale);
                    if (InputHandler.KeyDown(Keys.Left))
                        Engine.ChangeXOrigin(Engine.Origin.X - Config.MapMovingScale);
                    if (InputHandler.KeyDown(Keys.Right))
                        Engine.ChangeXOrigin(Engine.Origin.X + Config.MapMovingScale);
                    /*
                    hudOrigin.X = Config.HUDOrigin.X - (int)Config.BaseOrigin.X + (int)Engine.Origin.X;
                    hudOrigin.Y = Config.HUDOrigin.Y - (int)Config.BaseOrigin.Y + (int)Engine.Origin.Y;
                    // Window boxes update
                    scoresWindowBox.Position = new Vector2(hudOrigin.X, hudOrigin.Y);
                    timerWindowBox.Position = new Vector2(hudOrigin.X, scoresWindowBox.Size.Y + hudOrigin.Y);
                    */
                }
                #endregion

                // Background
                var position = new Vector2(
                    Engine.Origin.X - (int)(Engine.Origin.X / Engine.TileWidth) * Engine.TileWidth - Engine.TileWidth,
                    Engine.Origin.Y - (int)(Engine.Origin.Y / Engine.TileHeight) * Engine.TileHeight - Engine.TileHeight);

                for (int i = 0; i < (GraphicsDevice.Viewport.Width / Engine.TileWidth) + 2; i++)
                {
                    for (int j = 0; j < (GraphicsDevice.Viewport.Height / Engine.TileHeight) + 2; j++)
                    {
                        if (!((position.X + i * Engine.TileWidth > Engine.Origin.X &&
                            position.X + i * Engine.TileWidth < Engine.Origin.X + _mapSize.X * Engine.TileWidth - Engine.TileWidth) &&
                            (position.Y + j * Engine.TileHeight > Engine.Origin.Y &&
                            position.Y + j * Engine.TileHeight < Engine.Origin.Y + _mapSize.Y * Engine.TileHeight - Engine.TileHeight)))
                        {
                            FinalBomber.Instance.SpriteBatch.Draw(_wallTexture, new Vector2(position.X + (i * Engine.TileWidth), position.Y + (j * Engine.TileHeight)), Color.White);
                        }
                    }
                }

                World.DrawLevel(gameTime, FinalBomber.Instance.SpriteBatch, PlayerList[0].Camera);

                #region Draw each elements

                foreach (EdgeWall e in _edgeWallList)
                    e.Draw(gameTime);

                foreach (UnbreakableWall u in UnbreakableWallList)
                    u.Draw(gameTime);

                foreach (Wall w in _wallList)
                    w.Draw(gameTime);

                foreach (PowerUp i in _itemList)
                    i.Draw(gameTime);

                foreach (Teleporter t in TeleporterList)
                    t.Draw(gameTime);

                foreach (Arrow a in ArrowList)
                    a.Draw(gameTime);

                foreach (Bomb b in BombList)
                    b.Draw(gameTime);

                // Window boxes
                _scoresWindowBox.Draw(FinalBomber.Instance.SpriteBatch);
                _timerWindowBox.Draw(FinalBomber.Instance.SpriteBatch);

                #region Draw each players

                foreach (Player p in PlayerList)
                {
                    p.Draw(gameTime);

                    // HUD => Item Info
                    FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, Config.PlayersName[p.Id] + ": "
                         + Config.PlayersScores[p.Id] + " pt(s)",
                        new Vector2(_hudOrigin.X + _hudMarginLeft, _hudOrigin.Y + _hudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace), Color.Black);

                    if (Config.Debug)
                    {
                        FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, "Player " + p.Id + ": " + (p.CellPosition).ToString(),
                            new Vector2(_hudOrigin.X + _hudMarginLeft, _hudOrigin.Y + _hudTopSpace + Config.HUDPlayerInfoSpace * Config.PlayersNumber + 60 + 20 * (p.Id)), Color.Black);
                    }

                    // To space the red icons and the "normal color" icons
                    int iconLag = 0;

                    // Player's power
                    int counterRedFlames = 0;
                    if (p.BombPower >= 10)
                    {
                        iconLag = 10;
                        counterRedFlames = p.BombPower / 10;
                        for (int i = 0; i < counterRedFlames; i++)
                            FinalBomber.Instance.SpriteBatch.Draw(_itemInfoIcon,
                                new Vector2(_hudOrigin.X + _hudMarginLeft + 7 * i,
                                _hudOrigin.Y + _hudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 30),
                                new Rectangle(14, 0, 14, 14), Color.White);
                    }
                    else
                        iconLag = 0;
                    int counterYellowFlames = p.BombPower % 10;
                    for (int i = 0; i < counterYellowFlames; i++)
                        FinalBomber.Instance.SpriteBatch.Draw(_itemInfoIcon,
                            new Vector2(_hudOrigin.X + _hudMarginLeft + 14 * counterRedFlames + 7 * i + iconLag,
                                _hudOrigin.Y + _hudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 30),
                            new Rectangle(0, 0, 14, 14), Color.White);

                    // Player's bomb number
                    int counterRedBombs = 0;
                    if (p.CurrentBombAmount >= 10)
                    {
                        iconLag = 10;
                        counterRedBombs = p.CurrentBombAmount / 10;
                        for (int i = 0; i < counterRedBombs; i++)
                        {
                            FinalBomber.Instance.SpriteBatch.Draw(_itemInfoIcon, new Vector2(_hudOrigin.X + _hudMarginLeft + 7 * i,
                                _hudOrigin.Y + _hudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 50),
                                new Rectangle(42, 0, 14, 14), Color.White);
                        }
                    }
                    else
                        iconLag = 0;
                    int counterBlackBombs = p.CurrentBombAmount % 10;
                    int finalCounter = 0;
                    for (int i = 0; i < counterBlackBombs; i++)
                    {
                        FinalBomber.Instance.SpriteBatch.Draw(_itemInfoIcon,
                            new Vector2(_hudOrigin.X + 7 * counterRedBombs + _hudMarginLeft + 7 * i + iconLag, _hudOrigin.Y +
                                _hudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 50),
                            new Rectangle(28, 0, 14, 14), Color.White);
                        finalCounter = i;
                    }

                    if (p.HasBadEffect && p.BadEffect == BadEffect.NoBomb)
                    {
                        FinalBomber.Instance.SpriteBatch.Draw(_cross, new Rectangle(_hudOrigin.X + _hudMarginLeft,
                            _hudOrigin.Y + _hudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 50,
                            7 * counterRedBombs + 7 * finalCounter + iconLag + 15, 15), Color.White);
                    }

                    // Player's speed
                    int counterRedShoes = 0;
                    if (p.Speed / Config.PlayerSpeedIncrementeur >= 10)
                    {
                        iconLag = 10;
                        counterRedShoes = (int)(p.Speed / Config.PlayerSpeedIncrementeur) / 10;
                        for (int i = 0; i < counterRedShoes; i++)
                            FinalBomber.Instance.SpriteBatch.Draw(_itemInfoIcon, new Vector2(_hudOrigin.X + _hudMarginLeft + 7 * i,
                                _hudOrigin.Y + _hudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 70),
                                new Rectangle(70, 0, 14, 14), Color.White);
                    }
                    else
                        iconLag = 0;
                    int counterBlueShoes = (int)(p.Speed / Config.PlayerSpeedIncrementeur) % 10;

                    for (int i = 0; i < counterBlueShoes; i++)
                        FinalBomber.Instance.SpriteBatch.Draw(_itemInfoIcon,
                            new Vector2(_hudOrigin.X + _hudMarginLeft + counterRedShoes * 7 + 7 * i + iconLag,
                                _hudOrigin.Y + _hudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 70),
                            new Rectangle(56, 0, 14, 14), Color.White);

                    // Player's bad item timer
                    if (p.HasBadEffect)
                    {
                        FinalBomber.Instance.SpriteBatch.Draw(_itemInfoIcon, new Vector2(_hudOrigin.X + _hudMarginLeft,
                            _hudOrigin.Y + _hudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 90),
                                    new Rectangle(98, 0, 14, 14), Color.White);

                        int lenght = 200 - (int)((200 / p.BadEffectTimerLenght.TotalSeconds) * p.BadEffectTimer.TotalSeconds);
                        string badItemTimer = ((int)Math.Round(p.BadEffectTimerLenght.TotalSeconds - p.BadEffectTimer.TotalSeconds) + 1).ToString();
                        if (p.BadEffect == BadEffect.BombTimerChanged)
                            badItemTimer += " (" + p.BombTimer.TotalSeconds.ToString() + ")";

                        for (int i = 0; i < lenght; i++)
                        {
                            FinalBomber.Instance.SpriteBatch.Draw(_badItemTimerBar,
                                new Vector2(_hudOrigin.X + _hudMarginLeft + 20 + 1 * i,
                                    _hudOrigin.Y + _hudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 90), Color.White);
                        }
                        FinalBomber.Instance.SpriteBatch.DrawString(_smallFont, badItemTimer,
                            new Vector2(_hudOrigin.X + _hudMarginLeft + 20 + 1 * (lenght / 2) - _smallFont.MeasureString(badItemTimer).X / 2,
                                _hudOrigin.Y + _hudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 90), Color.White);
                    }
                    else
                    {
                        FinalBomber.Instance.SpriteBatch.Draw(_itemInfoIcon, new Vector2(_hudOrigin.X + _hudMarginLeft,
                            _hudOrigin.Y + _hudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 90),
                                    new Rectangle(84, 0, 14, 14), Color.White);
                    }
                }
                #endregion

                #endregion

                if (Config.ActiveSuddenDeath)
                {
                    Debug.Assert(SuddenDeath != null, "suddenDeath != null");
                    SuddenDeath.Draw(gameTime);
                }
            }

            // FPS
            FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, "FPS: " + _fps.ToString(), Vector2.Zero, Color.Black);
            // Bomb number
            //FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, "Bombs#: " + bombList.Count, new Vector2(0, 25), Color.Black);
            // Timer
            FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, _timer.ToString("mm") + ":" + _timer.ToString("ss"),
                new Vector2(_hudOrigin.X + _hudMarginLeft + (ControlManager.SpriteFont.MeasureString(_timer.ToString("mm") + ":" + _timer.ToString("ss")).X) + 25,
                    _hudOrigin.Y + _hudTopSpace + Config.HUDPlayerInfoSpace * Config.PlayersNumber + 22), Color.Black);


            if (_pause)
            {
                FinalBomber.Instance.SpriteBatch.DrawString(BigFont, "PAUSE",
                    new Vector2(FinalBomber.Instance.GraphicsDevice.Viewport.Width / 2 - _gameFont.MeasureString("PAUSE").X / 2,
                        FinalBomber.Instance.GraphicsDevice.Viewport.Height / 2 - _gameFont.MeasureString("PAUSE").Y / 2),
                    Color.Blue);
            }

            // We draw the path of each AI players
            foreach (Player player in PlayerList)
            {
                var aiPlayer = player as AIPlayer;
                if (aiPlayer != null)
                {
                    if (aiPlayer.Path != null && aiPlayer.Path.Count > 0)
                    {
                        foreach (Point p in aiPlayer.Path)
                        {
                            FinalBomber.Instance.SpriteBatch.Draw(_cross, new Rectangle((int)(Engine.CellToVector(p).X + Engine.Origin.X),
                                (int)(Engine.CellToVector(p).Y + Engine.Origin.Y), 32, 32),
                                new Rectangle(0, 0, _cross.Width, _cross.Height), Color.White);
                        }
                    }
                }
            }

            FinalBomber.Instance.SpriteBatch.End();
        }

        #endregion

        #region Method Region

        #region Initialization Method Region

        private void Reset()
        {
            MediaPlayer.IsRepeating = true;
            _songNumber = Random.Next(_mapSong.Length);
            MediaPlayer.Play(_mapSong[_songNumber]);

            _timer = TimeSpan.Zero;

            _engine = new Engine(32, 32, Vector2.Zero);

            // Lists
            _wallList = new List<Wall>();
            _itemList = new List<PowerUp>();
            BombList = new List<Bomb>();
            PlayerList = new List<Player>();
            UnbreakableWallList = new List<UnbreakableWall>();
            _edgeWallList = new List<EdgeWall>();
            TeleporterList = new List<Teleporter>();
            ArrowList = new List<Arrow>();

            _deadPlayersNumber = 0;

            CreateWorld();
            //ParseMap("classic.map");

            var origin = new Vector2(_hudOrigin.X / 2 - ((32 * World.Levels[World.CurrentLevel].Size.X) / 2),
                FinalBomber.Instance.GraphicsDevice.Viewport.Height / 2 - ((32 * World.Levels[World.CurrentLevel].Size.Y) / 2));

            Engine.Origin = origin;

            SuddenDeath = new SuddenDeath(FinalBomber.Instance, Config.PlayersPositions[0]);
        }

        private void CreateWorld()
        {
            var tilesets = new List<Tileset>() { new Tileset(_mapTexture, 64, 32, 32, 32) };

            var collisionLayer = new bool[Config.MapSize.X, Config.MapSize.Y];
            var mapPlayersPosition = new int[Config.MapSize.X, Config.MapSize.Y];
            var map = new IEntity[Config.MapSize.X, Config.MapSize.Y];
            var layer = new MapLayer(Config.MapSize.X, Config.MapSize.Y);
            var voidPosition = new List<Point>();

            // Item Map
            IEntity entity;

            // List of player position
            var playerPositions = new Dictionary<int, Point>();

            // We don't put wall around the players
            for (int i = 0; i < Config.PlayersNumber; i++)
            {
                Point playerPosition = Config.PlayersPositions[i];
                playerPositions[i + 1] = playerPosition;

                mapPlayersPosition[playerPosition.X, playerPosition.Y] = 2;
                mapPlayersPosition[playerPosition.X + 1, playerPosition.Y] = 1;
                mapPlayersPosition[playerPosition.X, playerPosition.Y + 1] = 1;
                mapPlayersPosition[playerPosition.X, playerPosition.Y - 1] = 1;
                mapPlayersPosition[playerPosition.X - 1, playerPosition.Y] = 1;

                mapPlayersPosition[playerPosition.X - 1, playerPosition.Y - 1] = 1;
                mapPlayersPosition[playerPosition.X - 1, playerPosition.Y + 1] = 1;
                mapPlayersPosition[playerPosition.X + 1, playerPosition.Y - 1] = 1;
                mapPlayersPosition[playerPosition.X + 1, playerPosition.Y + 1] = 1;
            }

            /*
            entity = new Teleporter(FinalBomber.Instance, new Vector2(
                        5 * Engine.TileWidth,
                        1 * Engine.TileHeight));
            teleporterList.Add((Teleporter)entity);
            map[5,1] = entity;

            entity = new Teleporter(FinalBomber.Instance, new Vector2(
                        10 * Engine.TileWidth,
                        1 * Engine.TileHeight));
            teleporterList.Add((Teleporter)entity);
            map[10, 1] = entity;
            */

            for (int x = 0; x < Config.MapSize.X; x++)
            {
                for (int y = 0; y < Config.MapSize.Y; y++)
                {
                    if (!(x == 0 || y == 0 || x == (Config.MapSize.X - 1) || y == (Config.MapSize.Y - 1) ||
                        (x % 2 == 0 && y % 2 == 0)) && (mapPlayersPosition[x, y] != 1 && mapPlayersPosition[x, y] != 2))
                        voidPosition.Add(new Point(x, y));
                }
            }

            #region Teleporter
            if (Config.ActiveTeleporters)
            {
                if (Config.TeleporterPositionType == TeleporterPositionTypeEnum.Randomly)
                {
                    int randomVoid = 0;
                    for (int i = 0; i < MathHelper.Clamp(Config.TeleporterNumber, 0, voidPosition.Count - 1); i++)
                    {
                        randomVoid = Random.Next(voidPosition.Count);
                        entity = new Teleporter(new Point(
                            voidPosition[randomVoid].X,
                            voidPosition[randomVoid].Y));
                        TeleporterList.Add((Teleporter)entity);
                        map[voidPosition[randomVoid].X, voidPosition[randomVoid].Y] = entity;
                        voidPosition.Remove(voidPosition[randomVoid]);
                    }
                }
                else if (Config.TeleporterPositionType == TeleporterPositionTypeEnum.PlusForm)
                {
                    var teleporterPositions = new Point[]
                    {
                        new Point((int)Math.Ceiling((double)(Config.MapSize.X - 2)/(double)2), 1),
                        new Point(1, (int)Math.Ceiling((double)(Config.MapSize.Y - 2)/(double)2)),
                        new Point((int)Math.Ceiling((double)(Config.MapSize.X - 2)/(double)2), Config.MapSize.Y - 2),
                        new Point(Config.MapSize.X - 2, (int)Math.Ceiling((double)(Config.MapSize.Y - 2)/(double)2))
                    };

                    for (int i = 0; i < teleporterPositions.Length; i++)
                    {
                        entity = new Teleporter(new Point(
                            teleporterPositions[i].X,
                            teleporterPositions[i].Y));
                        TeleporterList.Add((Teleporter)entity);
                        map[teleporterPositions[i].X, teleporterPositions[i].Y] = entity;
                    }
                }
            }
            #endregion

            #region Arrow
            if (Config.ActiveArrows)
            {
                if (Config.ArrowPositionType == ArrowPositionTypeEnum.Randomly)
                {
                    var lookDirectionArray = new LookDirection[] { LookDirection.Up, LookDirection.Down, LookDirection.Left, LookDirection.Right };
                    for (int i = 0; i < MathHelper.Clamp(Config.ArrowNumber, 0, voidPosition.Count - 1); i++)
                    {
                        int randomVoid = Random.Next(voidPosition.Count);
                        entity = new Arrow(new Point(
                            voidPosition[randomVoid].X,
                            voidPosition[randomVoid].Y),
                            lookDirectionArray[Random.Next(lookDirectionArray.Length)]);
                        ArrowList.Add((Arrow)entity);
                        map[voidPosition[randomVoid].X, voidPosition[randomVoid].Y] = entity;
                        voidPosition.Remove(voidPosition[randomVoid]);
                    }
                }
                else if (Config.ArrowPositionType == ArrowPositionTypeEnum.SquareForm)
                {
                    int outsideArrowsLag = 0;
                    var ratio = (int)Math.Ceiling((double)(4 * (Config.MapSize.X - 2)) / (double)5);
                    if (ratio % 2 == 0)
                        outsideArrowsLag = 1;

                    var arrowPositions = new Point[]
                    {
                        // ~~ Inside ~~ //
                        // Top left
                        new Point(
                            (int)Math.Ceiling((double)(Config.MapSize.X - 2)/(double)4) + 1, 
                            (int)Math.Ceiling((double)(Config.MapSize.Y - 2)/(double)4) + 1),

                        // Top right
                        new Point(
                            (int)Math.Ceiling((double)(3 * (Config.MapSize.X - 2))/(double)4) - 1, 
                            (int)Math.Ceiling((double)(Config.MapSize.Y - 2)/(double)4) + 1),

                        // Bottom left
                        new Point(
                            (int)Math.Ceiling((double)(Config.MapSize.X - 2)/(double)4) + 1, 
                            (int)Math.Ceiling((double)(3 * (Config.MapSize.Y - 2))/(double)4) - 1),

                        // Bottom right
                        new Point(
                            (int)Math.Ceiling((double)(3 * (Config.MapSize.X - 2))/(double)4) - 1, 
                            (int)Math.Ceiling((double)(3 * (Config.MapSize.Y - 2))/(double)4) - 1),

                        // ~~ Outside ~~ //
                        // Top left
                        new Point(
                            (int)Math.Ceiling((double)(Config.MapSize.X - 2)/(double)5), 
                            (int)Math.Ceiling((double)(Config.MapSize.Y - 2)/(double)5)),

                        // Top right
                        new Point(
                            (int)Math.Ceiling((double)(4 * (Config.MapSize.X - 2))/(double)5) + outsideArrowsLag, 
                            (int)Math.Ceiling((double)(Config.MapSize.Y - 2)/(double)5)),

                        // Bottom left
                        new Point(
                            (int)Math.Ceiling((double)(Config.MapSize.X - 2)/(double)5), 
                            (int)Math.Ceiling((double)(4 * (Config.MapSize.Y - 2))/(double)5) + outsideArrowsLag),

                        // Bottom right
                        new Point(
                            (int)Math.Ceiling((double)(4 * (Config.MapSize.X - 2))/(double)5 + outsideArrowsLag), 
                            (int)Math.Ceiling((double)(4 * (Config.MapSize.Y - 2))/(double)5) + outsideArrowsLag)
                    };

                    for (int i = 0; i < arrowPositions.Length; i++)
                    {
                        entity = new Arrow(new Point(
                            arrowPositions[i].X,
                            arrowPositions[i].Y),
                            Config.ArrowLookDirection[i % 4]);
                        ArrowList.Add((Arrow)entity);
                        map[arrowPositions[i].X, arrowPositions[i].Y] = entity;
                    }
                }
            }
            #endregion

            int counter = 0;
            for (int x = 0; x < Config.MapSize.X; x++)
            {
                for (int y = 0; y < Config.MapSize.Y; y++)
                {
                    var tile = new Tile(0, 0);
                    if (x == 0 || y == 0 || x == (Config.MapSize.X - 1) || y == (Config.MapSize.Y - 1) ||
                        (x % 2 == 0 && y % 2 == 0))
                    {
                        // Inside wallList
                        if ((x % 2 == 0 && y % 2 == 0 && !(x == 0 || y == 0 || x == (Config.MapSize.X - 1) || y == (Config.MapSize.Y - 1))) && mapPlayersPosition[x, y] != 2)
                        {
                            entity = new UnbreakableWall(new Point(x, y));
                            this.UnbreakableWallList.Add((UnbreakableWall)entity);
                            map[x, y] = entity;
                            collisionLayer[x, y] = true;
                        }
                        // Outside wallList
                        else if (mapPlayersPosition[x, y] != 2)
                        {
                            entity = new EdgeWall(new Point(x, y));
                            this._edgeWallList.Add((EdgeWall)entity);
                            counter++;
                            map[x, y] = entity;
                            collisionLayer[x, y] = true;
                        }
                    }
                    else
                    {
                        // Wall
                        if ((mapPlayersPosition[x, y] != 1 && mapPlayersPosition[x, y] != 2) && map[x, y] == null &&
                            Random.Next(0, 100) < GameConfiguration.WallPercentage)
                        {
                            collisionLayer[x, y] = true;
                            entity = new Wall(new Point(x, y));
                            _wallList.Add((Wall)entity);
                            map[x, y] = entity;
                        }
                        //tile = new Tile(0, 0);
                    }
                    layer.SetTile(x, y, tile);
                }
            }

            var mapLayers = new List<MapLayer> { layer };

            var tileMap = new TileMap(tilesets, mapLayers);
            Map level = null;//new Map(Config.MapSize, tileMap, map, collisionLayer);

            World = new World(FinalBomber.Instance, FinalBomber.Instance.ScreenRectangle);
            World.Levels.Add(level);
            World.CurrentLevel = 0;

            foreach (int playerID in playerPositions.Keys)
            {
                if (Config.AIPlayers[playerID - 1])
                {
                    var player = new AIPlayer(Math.Abs(playerID));
                    PlayerList.Add(player);
                    map[playerPositions[playerID].X, playerPositions[playerID].Y] = player;
                }
                else
                {
                    var player = new HumanPlayer(Math.Abs(playerID));
                    PlayerList.Add(player);
                    map[playerPositions[playerID].X, playerPositions[playerID].Y] = player;
                }
            }
        }

        private void ParseMap(string file)
        {
            try
            {
                var streamReader = new StreamReader("Content/Maps/" + file);
                string line = streamReader.ReadLine();
                string[] lineSplit = line.Split(' ');
                var parsedMapSize = new int[] { int.Parse(lineSplit[0]), int.Parse(lineSplit[1]) };

                var mapSize = new Point(parsedMapSize[0], parsedMapSize[1]);
                var tilesets = new List<Tileset>() { new Tileset(_mapTexture, 64, 32, 32, 32) };

                var collisionLayer = new bool[mapSize.X, mapSize.Y];
                var mapPlayersPosition = new int[mapSize.X, mapSize.Y];
                var map = new IEntity[mapSize.X, mapSize.Y];
                var layer = new MapLayer(mapSize.X, mapSize.Y);
                var voidPosition = new List<Point>();
                var playerPositions = new Dictionary<int, Point>();

                int j = 0;
                while (!streamReader.EndOfStream)
                {
                    line = streamReader.ReadLine();
                    Debug.Assert(line != null, "line != null");
                    lineSplit = line.Split(' ');
                    for (int i = 0; i < lineSplit.Length; i++)
                    {
                        int id = int.Parse(lineSplit[i]);
                        switch (id)
                        {
                            case 1:
                                var unbreakableWall = new UnbreakableWall(new Point(i, j));
                                map[i, j] = unbreakableWall;
                                UnbreakableWallList.Add(unbreakableWall);
                                collisionLayer[i, j] = true;
                                break;
                            case 2:
                                var edgeWall = new EdgeWall(new Point(i, j));
                                map[i, j] = edgeWall;
                                _edgeWallList.Add(edgeWall);
                                collisionLayer[i, j] = true;
                                break;
                            case 3:
                                var wall = new Wall(new Point(i, j));
                                _wallList.Add(wall);
                                map[i, j] = wall;
                                collisionLayer[i, j] = true;
                                break;
                            case 6:
                                var teleporter = new Teleporter(new Point(i, j));
                                map[i, j] = teleporter;
                                TeleporterList.Add(teleporter);
                                break;
                            case 7:
                                var arrow = new Arrow(new Point(i, j), LookDirection.Down);
                                ArrowList.Add(arrow);
                                map[i, j] = arrow;
                                break;
                            default:
                                if (id < 0 && Math.Abs(id) <= Config.PlayersNumber)
                                {
                                    playerPositions[Math.Abs(id)] = new Point(i, j);
                                }
                                break;
                        }
                    }
                    j++;
                }

                var mapLayers = new List<MapLayer> { layer };

                var tileMap = new TileMap(tilesets, mapLayers);
                Map level = null;//new Map(mapSize, tileMap, map, collisionLayer);

                World = new World(FinalBomber.Instance, FinalBomber.Instance.ScreenRectangle);
                World.Levels.Add(level);
                World.CurrentLevel = 0;

                foreach (int playerID in playerPositions.Keys)
                {
                    if (Config.AIPlayers[playerID])
                    {
                        var player = new AIPlayer(Math.Abs(playerID));
                        PlayerList.Add(player);
                        map[playerPositions[playerID].X, playerPositions[playerID].Y] = player;
                    }
                    else
                    {
                        var player = new HumanPlayer(Math.Abs(playerID));
                        PlayerList.Add(player);
                        map[playerPositions[playerID].X, playerPositions[playerID].Y] = player;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion

        public void AddBomb(Bomb bomb)
        {
            if (World.Levels[FinalBomber.Instance.GamePlayScreen.World.CurrentLevel].
                            Board[bomb.CellPositionX, bomb.CellPositionY] is Player)
            {
                World.Levels[FinalBomber.Instance.GamePlayScreen.World.CurrentLevel].
                    Board[bomb.CellPosition.X, bomb.CellPosition.Y] = bomb;
                World.Levels[FinalBomber.Instance.GamePlayScreen.World.CurrentLevel].
                CollisionLayer[bomb.CellPosition.X, bomb.CellPosition.Y] = true;
            }

            BombList.Add(bomb);
        }

        #endregion
    }
}
