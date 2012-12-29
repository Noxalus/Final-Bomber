using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;

using Final_Bomber.Controls;
using Final_Bomber.Components;
using Final_Bomber.TileEngine;
using Final_Bomber.Sprites;
using Final_Bomber.WorldEngine;
using Microsoft.Xna.Framework.Audio;
using Final_Bomber.DataBase;
using System.IO;

namespace Final_Bomber.Screens
{
    public class GamePlayScreen : BaseGameState
    {
        #region Field Region
        private bool pause;
        
        Engine engine;

        // Map
        Point mapSize;
        private World world;
        Texture2D mapTexture;

        // List
        private List<Player> playerList;
        private List<Wall> wallList;
        private List<Item> itemList;
        private List<Bomb> bombList;
        private List<UnbreakableWall> unbreakableWallList;
        private List<EdgeWall> edgeWallList;
        private List<Teleporter> teleporterList;
        private List<Arrow> arrowList;

        // Sudden Death
        SuddenDeath suddenDeath;

        // Sound Effects & Musics
        Song[] mapSong;
        Song[] mapSong_hurry;
        int songNumber;
        SoundEffect bombExplosionSound;
        SoundEffect itemPickUpSound;
        SoundEffect playerDeathSound;

        // HUD
        Texture2D itemInfoIcon;
        TimeSpan timer;
        SpriteFont gameFont;
        SpriteFont smallFont;
        Point hudOrigin;
        int hudTopSpace;
        int hudMarginLeft;
        Texture2D cross;
        Texture2D badItemTimerBar;
        
        // Window box
        Texture2D windowSkin;
        WindowBox scoresWindowBox;
        WindowBox timerWindowBox;

        Texture2D wallTexture;

        // Dead players number
        int deadPlayersNumber;

        // FPS => Performance
        float elapseTime;
        int frameCounter;
        int fps;

        // Random
        static Random random;

        #endregion

        #region Property Region

        public World World
        {
            get { return world; }
            set { world = value; }
        }

        public List<Player> PlayerList
        {
            get { return playerList; }
        }

        public List<Wall> WallList
        {
            get { return wallList; }
        }

        public List<Bomb> BombList
        {
            get { return bombList; }
        }

        public List<Item> ItemList
        {
            get { return itemList; }
        }

        public List<UnbreakableWall> UnbreakableWallList
        {
            get { return unbreakableWallList; }
        }

        public List<EdgeWall> EdgeWallArray
        {
            get { return edgeWallList; }
        }

        public List<Teleporter> TeleporterList
        {
            get { return teleporterList; }
        }

        public List<Arrow> ArrowList
        {
            get { return arrowList; }
        }

        // Sudden Death
        public SuddenDeath SuddenDeath
        {
            get { return suddenDeath; }
        }

        // Sound Effects
        public Song MapSongHurry
        {
            get { return mapSong_hurry[songNumber]; }
        }

        public SoundEffect BombExplosionSound
        {
            get { return bombExplosionSound; }
        }

        public SoundEffect ItemPickUpSound
        {
            get { return itemPickUpSound; }
        }

        public SoundEffect PlayerDeathSound
        {
            get { return playerDeathSound; }
        }

        // Random
        public Random Random
        {
            get { return random; }
        }

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
            random = new Random();
            //PostHTTPRequest.postSynchronous("http://finalbomber.free.fr/scores/scores.php", 
            //new Dictionary<string, string>() { { "score", random.Next(100).ToString() } });

            // FPS
            elapseTime = 0f;
            frameCounter = 0;
            fps = 0;
            pause = false;
            hudOrigin = new Point(GameRef.GraphicsDevice.Viewport.Width - 234, 0);
            hudTopSpace = 15;
            hudMarginLeft = 15;

            base.Initialize();
            Reset();
            scoresWindowBox = new WindowBox(windowSkin, new Vector2(hudOrigin.X, hudOrigin.Y), 
                new Point(GraphicsDevice.Viewport.Width - (hudOrigin.X),
                    hudTopSpace + Config.PlayersNumber * Config.HUDPlayerInfoSpace + 15));

            timerWindowBox = new WindowBox(windowSkin, new Vector2(hudOrigin.X, scoresWindowBox.Size.Y),
                new Point(GraphicsDevice.Viewport.Width - hudOrigin.X, 40));
        }

        protected override void LoadContent()
        {
            // Pictures      
            mapTexture = GameRef.Content.Load<Texture2D>("Graphics/Tilesets/tileset1");
            itemInfoIcon = GameRef.Content.Load<Texture2D>("Graphics/Pictures/ItemInfo");
            cross = GameRef.Content.Load<Texture2D>("Graphics/Pictures/Cross");
            badItemTimerBar = GameRef.Content.Load<Texture2D>("Graphics/Pictures/BadItemTimerCross");
            wallTexture = GameRef.Content.Load<Texture2D>("Graphics/Characters/edgeWall");
            windowSkin = GameRef.Content.Load<Texture2D>("Graphics/Windowskins/Windowskin1");

            // Sound Effects & Musics
            mapSong = new Song[]
            {
                GameRef.Content.Load<Song>("Audio/Musics/map1")
            };
            mapSong_hurry = new Song[]
            {
                GameRef.Content.Load<Song>("Audio/Musics/map1_hurry")
            };
            bombExplosionSound = GameRef.Content.Load<SoundEffect>("Audio/Sounds/boom");
            itemPickUpSound = GameRef.Content.Load<SoundEffect>("Audio/Sounds/item");
            playerDeathSound = GameRef.Content.Load<SoundEffect>("Audio/Sounds/playerDeath");

            // Fonts
            gameFont = GameRef.Content.Load<SpriteFont>("Graphics/Fonts/GameFont");
            smallFont = GameRef.Content.Load<SpriteFont>("Graphics/Fonts/SmallFont");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (!pause)
            {
                #region FPS
                elapseTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                frameCounter++;

                if (elapseTime > 1)
                {
                    fps = frameCounter;
                    frameCounter = 0;
                    elapseTime = 0;
                }
                #endregion

                timer += gameTime.ElapsedGameTime;

                #region Controls
                ControlManager.Update(gameTime, PlayerIndex.One);

                if (InputHandler.KeyPressed(Keys.P))
                {
                    MediaPlayer.Pause();
                    pause = true;
                }

                if (InputHandler.KeyDown(Keys.Escape))
                {
                    MediaPlayer.Stop();
                    StateManager.ChangeState(GameRef.BattleMenuScreen);
                }

                if (Config.Debug)
                {
                    if (InputHandler.KeyPressed(Keys.R))
                        Reset();
                    else if (InputHandler.KeyPressed(Keys.A))
                    {
                        foreach (Wall w in wallList)
                            w.Destroy();
                    }
                    else if (InputHandler.KeyPressed(Keys.I))
                    {
                        foreach (Item i in itemList)
                            i.Destroy();
                    }
                    else if (InputHandler.KeyDown(Keys.B))
                    {
                        Point pos = new Point(
                            random.Next(1, world.Levels[world.CurrentLevel].Size.X - 1),
                            random.Next(1, world.Levels[world.CurrentLevel].Size.Y - 1));
                        while (world.Levels[world.CurrentLevel].CollisionLayer[pos.X, pos.Y])
                        {
                            pos = new Point(
                                random.Next(1, world.Levels[world.CurrentLevel].Size.X - 1),
                                random.Next(1, world.Levels[world.CurrentLevel].Size.Y - 1));
                        }
                        Bomb bomb = new Bomb(GameRef, -42, pos, random.Next(Config.MinBombPower, Config.MaxBombPower), Config.BombTimer, 0f);
                        bombList.Add(bomb);
                        world.Levels[world.CurrentLevel].Map[pos.X, pos.Y] = bomb;
                    }
                    
                    else if (InputHandler.KeyDown(Keys.F1))
                    {
                        playerList[0].IncreasePower(1);
                        playerList[0].IncreaseTotalBombNumber(1);
                        playerList[0].IncreaseSpeed(Config.PlayerSpeedIncrementeur);
                    }
                    else if (InputHandler.KeyDown(Keys.F2))
                    {
                        playerList[0].IncreasePower(-1);
                        playerList[0].IncreaseTotalBombNumber(-1);
                        playerList[0].IncreaseSpeed(-Config.PlayerSpeedIncrementeur);
                    }
                    
                }
                #endregion

                #region Walls

                for (int i = 0; i < wallList.Count; i++)
                {
                    wallList[i].Update(gameTime);

                    // Is it die ?
                    if (world.Levels[world.CurrentLevel].
                        HazardMap[wallList[i].Sprite.CellPosition.X, wallList[i].Sprite.CellPosition.Y] == 3)
                    {
                        world.Levels[world.CurrentLevel].
                        HazardMap[wallList[i].Sprite.CellPosition.X, wallList[i].Sprite.CellPosition.Y] = 0;
                        wallList[i].Destroy();
                    }

                    // We clean the obsolete elements
                    if (!wallList[i].IsAlive)
                    {
                        if (!Config.ActiveSuddenDeath || 
                            (Config.ActiveSuddenDeath && !suddenDeath.Visited[wallList[i].Sprite.CellPosition.X, wallList[i].Sprite.CellPosition.Y]))
                        {
                            world.Levels[world.CurrentLevel].CollisionLayer[wallList[i].Sprite.CellPosition.X, wallList[i].Sprite.CellPosition.Y] = false;
                            if (random.Next(0, 100) < MathHelper.Clamp(Config.ItemNumber, 0, 100))
                            {
                                Item item = new Item(GameRef, wallList[i].Sprite.Position);
                                itemList.Add(item);
                                world.Levels[world.CurrentLevel].Map[wallList[i].Sprite.CellPosition.X, wallList[i].Sprite.CellPosition.Y] = item;
                            }
                            else
                            {
                                world.Levels[world.CurrentLevel].Map[wallList[i].Sprite.CellPosition.X, wallList[i].Sprite.CellPosition.Y] = null;
                            }
                        }
                        wallList.Remove(wallList[i]);
                    }
                }
                #endregion

                #region Bombs
                for (int i = 0; i < bombList.Count; i++)
                {
                    bombList[i].Update(gameTime);

                    // Is it die ?
                    if (world.Levels[world.CurrentLevel].
                        HazardMap[bombList[i].Sprite.CellPosition.X, bombList[i].Sprite.CellPosition.Y] == 3 &&
                        !bombList[i].InDestruction)
                    {
                        bombList[i].Destroy();
                    }

                    // We clean the obsolete elements
                    if (!bombList[i].IsAlive)
                    {
                        if( world.Levels[world.CurrentLevel].Map[bombList[i].Sprite.CellPosition.X, bombList[i].Sprite.CellPosition.Y] is Bomb)
                            world.Levels[world.CurrentLevel].Map[bombList[i].Sprite.CellPosition.X, bombList[i].Sprite.CellPosition.Y] = null;
                        world.Levels[world.CurrentLevel].CollisionLayer[bombList[i].Sprite.CellPosition.X, bombList[i].Sprite.CellPosition.Y] = false;

                        // We don't forget to give it back to its owner
                        if (bombList[i].Id > 0)
                        {
                            Player pl = playerList.Find(p => p.Id == bombList[i].Id);
                            if (pl != null && pl.CurrentBombNumber < pl.TotalBombNumber)
                                pl.CurrentBombNumber++;
                        }

                        // Update the hazard map
                        List<Bomb> bL = bombList.FindAll(b => !b.InDestruction);
                        foreach (Point p in bombList[i].ActionField)
                        {
                            bool sameCellThanAnOther = false;
                            foreach (Bomb b in bombList)
                            {
                                if (!(b.Sprite.CellPosition.X == bombList[i].Sprite.CellPosition.X &&
                                    b.Sprite.CellPosition.Y == bombList[i].Sprite.CellPosition.Y))
                                {
                                    if (b.ActionField.Find(c => c.X == p.X && c.Y == p.Y) != Point.Zero)
                                    {
                                        this.world.Levels[this.world.CurrentLevel].HazardMap[p.X, p.Y] = 2;
                                        sameCellThanAnOther = true;
                                        break;
                                    }
                                }
                            }
                            if (!sameCellThanAnOther)
                                this.world.Levels[this.world.CurrentLevel].HazardMap[p.X, p.Y] = 0;
                        }
                        bombList.Remove(bombList[i]);
                    }
                }
                #endregion

                #region Items
                for (int i = 0; i < itemList.Count; i++)
                {
                    itemList[i].Update(gameTime);

                    // Is it die ?
                    if (world.Levels[world.CurrentLevel].
                        HazardMap[itemList[i].Sprite.CellPosition.X, itemList[i].Sprite.CellPosition.Y] == 3)
                        itemList[i].Destroy();

                    // We clean the obsolete elements
                    if (!itemList[i].IsAlive)
                    {
                        if(world.Levels[world.CurrentLevel].Map[itemList[i].Sprite.CellPosition.X, itemList[i].Sprite.CellPosition.Y] is Item)
                            world.Levels[world.CurrentLevel].Map[itemList[i].Sprite.CellPosition.X, itemList[i].Sprite.CellPosition.Y] = null;
                        itemList.Remove(itemList[i]);
                    }
                }
                #endregion

                #region Player
                for (int i = 0; i < playerList.Count; i++)
                {
                    playerList[i].Update(gameTime);

                    // Is it die ?
                    if (!playerList[i].InDestruction && !playerList[i].IsInvincible && world.Levels[world.CurrentLevel].
                        HazardMap[playerList[i].Sprite.CellPosition.X, playerList[i].Sprite.CellPosition.Y] == 3)
                    {
                        int bombId = -42;
                        List<Bomb> bl = bombList.FindAll(b => b.InDestruction);
                        foreach (Bomb b in bl)
                        {
                            foreach (Point po in b.ActionField)
                            {
                                if (po == playerList[i].Sprite.CellPosition)
                                {
                                    bombId = b.Id;
                                    break;
                                }
                            }
                        }
                        // Suicide
                        if (bombId == playerList[i].Id)
                            Config.PlayersScores[bombId - 1]--;
                        else if (bombId >= 0 && bombId < Config.PlayersNumber)
                        {
                            Config.PlayersScores[bombId - 1]++;
                            Player player = playerList.Find(p => p.Id == bombId);
                            if (player.OnEdge)
                            {
                                player.Rebirth(playerList[i].Sprite.Position);
                                deadPlayersNumber--;
                            }
                        }
                        playerList[i].Destroy();
                    }

                    // We clean the obsolete players
                    if (!playerList[i].IsAlive)
                    {
                        if (!playerList[i].OnEdge)
                        {
                            if (world.Levels[world.CurrentLevel].Map[playerList[i].Sprite.CellPosition.X, playerList[i].Sprite.CellPosition.Y] is Player)
                            {
                                Player p = (Player)world.Levels[world.CurrentLevel].Map[playerList[i].Sprite.CellPosition.X, playerList[i].Sprite.CellPosition.Y];
                                if (p.Id == playerList[i].Id)
                                    world.Levels[world.CurrentLevel].Map[playerList[i].Sprite.CellPosition.X, playerList[i].Sprite.CellPosition.Y] = null;
                            }
                            deadPlayersNumber++;
                        }

                        if ((Config.ActiveSuddenDeath && suddenDeath.HasStarted))
                            playerList.Remove(playerList[i]);
                        else
                            playerList[i].OnEdge = true;
                    }
                }
                #endregion

                #region Teleporters

                for (int i = 0; i < teleporterList.Count; i++)
                {
                    teleporterList[i].Update(gameTime);

                    if (!teleporterList[i].IsAlive)
                    {
                        teleporterList.Remove(teleporterList[i]);
                    }
                }

                #endregion

                #region Arrows

                for (int i = 0; i < arrowList.Count; i++)
                {
                    arrowList[i].Update(gameTime);

                    if (!arrowList[i].IsAlive)
                    {
                        arrowList.Remove(arrowList[i]);
                    }
                }

                #endregion

                // Sudden Death
                if (Config.ActiveSuddenDeath)
                    suddenDeath.Update(gameTime);

                // World Update
                world.Update(gameTime);

                // End of the round
                if ((Config.PlayersNumber > 1 && deadPlayersNumber >= Config.PlayersNumber - 1) ||
                    (Config.PlayersNumber == 1 && deadPlayersNumber == 1))
                {
                    foreach (Player p in playerList)
                    {
                        if (p.IsAlive)
                            Config.PlayersScores[p.Id - 1] += 5;
                    }
                    Reset();
                }

                base.Update(gameTime);
            }
            else if (InputHandler.KeyPressed(Keys.P))
            {
                MediaPlayer.Resume();
                pause = false;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (suddenDeath != null && suddenDeath.HasStarted)
            {
                Matrix translate = Matrix.CreateTranslation(Functions.Shake(random));

                // Displace everything that gets drawn in this batch 
                GameRef.SpriteBatch.Begin(
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
                GameRef.SpriteBatch.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    null,
                    null,
                    null,
                    Matrix.Identity);
            }

            base.Draw(gameTime);

            ControlManager.Draw(GameRef.SpriteBatch);

            #region Debug Region
            if (Config.Debug && InputHandler.KeyDown(Keys.H))
            {
                for (int x = 0; x < world.Levels[world.CurrentLevel].Size.X; x++)
                {
                    for (int y = 0; y < world.Levels[world.CurrentLevel].Size.Y; y++)
                    {
                        GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, world.Levels[world.CurrentLevel].HazardMap[x, y].ToString(),
                            new Vector2(x * 30, 80 + 20 * y), Color.Black);
                    }
                }
            }
            else if (Config.Debug && InputHandler.KeyDown(Keys.C))
            {
                for (int x = 0; x < world.Levels[world.CurrentLevel].Size.X; x++)
                {
                    for (int y = 0; y < world.Levels[world.CurrentLevel].Size.Y; y++)
                    {
                        if (!world.Levels[world.CurrentLevel].CollisionLayer[x, y])
                        {
                            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, "0",
                                new Vector2(x * 20, 80 + 20 * y), Color.ForestGreen);
                        }
                        else
                        {
                            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, "1",
                            new Vector2(x * 20, 80 + 20 * y), Color.Purple);
                        }
                    }
                }
            }
            else if (Config.Debug && InputHandler.KeyDown(Keys.O))
            {
                for (int x = 0; x < world.Levels[world.CurrentLevel].Size.X; x++)
                {
                    for (int y = 0; y < world.Levels[world.CurrentLevel].Size.Y; y++)
                    {
                        string itemMapType = "";
                        Color colorMapItem = Color.White;
                        if (world.Levels[world.CurrentLevel].Map[x, y] != null)
                        {
                            switch (world.Levels[world.CurrentLevel].Map[x, y].GetType().Name)
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
                        
                        GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, itemMapType,
                            new Vector2(x * 20, 80 + 20 * y), colorMapItem);
                    }
                }
            }
            // Sudden Death
            else if (Config.Debug && InputHandler.KeyDown(Keys.S))
            {
                if (Config.ActiveSuddenDeath)
                {
                    for (int x = 0; x < world.Levels[world.CurrentLevel].Size.X; x++)
                    {
                        for (int y = 0; y < world.Levels[world.CurrentLevel].Size.Y; y++)
                        {
                            if (suddenDeath.Visited[x, y])
                            {
                                GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, "1",
                                    new Vector2(x * 30, 80 + 20 * y), Color.Red);
                            }
                            else
                            {
                                GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, "0",
                                    new Vector2(x * 30, 80 + 20 * y), Color.Green);
                            }
                        }
                    }
                }
            }
            else if (Config.Debug && Config.isThereAIPlayer && InputHandler.KeyDown(Keys.L))
            {
                int[,] costMatrix = AI.CostMatrix(
                    playerList[playerList.Count - 1].Sprite.CellPosition, 
                    world.Levels[world.CurrentLevel].CollisionLayer,
                    world.Levels[world.CurrentLevel].HazardMap, Config.MapSize);

                for (int x = 0; x < costMatrix.GetLength(0); x++)
                {
                    for (int y = 0; y < costMatrix.GetLength(1); y++)
                    {
                        if (costMatrix[x, y] == mapSize.X * mapSize.Y)
                        {
                            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, ".",
                                new Vector2(x * 30, 80 + 20 * y), Color.White);
                        }
                        else
                        {
                            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, costMatrix[x, y].ToString(),
                                new Vector2(x * 30, 80 + 20 * y), Color.Red);
                        }
                    }
                }

            }
            else if (Config.Debug && InputHandler.KeyDown(Keys.T))
            {
                int[,] interestMatrix = AI.MakeInterestMatrix(
                    playerList[playerList.Count - 1].Sprite.CellPosition, 
                    world.Levels[world.CurrentLevel].Map, 
                    world.Levels[world.CurrentLevel].CollisionLayer,
                    world.Levels[world.CurrentLevel].HazardMap, mapSize);

                for (int x = 0; x < interestMatrix.GetLength(0); x++)
                {
                    for (int y = 0; y < interestMatrix.GetLength(1); y++)
                    {
                        if (interestMatrix[x, y] == 0)
                        {
                            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, ".",
                                new Vector2(x * 30, 80 + 20 * y), Color.White);
                        }
                        else
                        {
                            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, interestMatrix[x, y].ToString(),
                                new Vector2(x * 30, 80 + 20 * y), Color.Red);
                        }
                    }
                }

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
                Vector2 position = new Vector2(
                    Engine.Origin.X - (int)(Engine.Origin.X / Engine.TileWidth) * Engine.TileWidth - Engine.TileWidth,
                    Engine.Origin.Y - (int)(Engine.Origin.Y / Engine.TileHeight) * Engine.TileHeight - Engine.TileHeight);

                for (int i = 0; i < (GraphicsDevice.Viewport.Width / Engine.TileWidth) + 2; i++)
                {
                    for (int j = 0; j < (GraphicsDevice.Viewport.Height / Engine.TileHeight) + 2; j++)
                    {
                        if (!((position.X + i * Engine.TileWidth > Engine.Origin.X &&
                            position.X + i * Engine.TileWidth < Engine.Origin.X + mapSize.X * Engine.TileWidth - Engine.TileWidth) &&
                            (position.Y + j * Engine.TileHeight > Engine.Origin.Y &&
                            position.Y + j * Engine.TileHeight < Engine.Origin.Y + mapSize.Y * Engine.TileHeight - Engine.TileHeight)))
                        {
                            GameRef.SpriteBatch.Draw(wallTexture, new Vector2(position.X + (i * Engine.TileWidth), position.Y + (j * Engine.TileHeight)), Color.White);
                        }
                    }
                }

                world.DrawLevel(gameTime, GameRef.SpriteBatch, playerList[0].Camera);

                #region Draw each elements

                foreach (EdgeWall e in edgeWallList)
                    e.Draw(gameTime);

                foreach (UnbreakableWall u in unbreakableWallList)
                    u.Draw(gameTime);

                foreach (Wall w in wallList)
                    w.Draw(gameTime);

                foreach (Item i in itemList)
                    i.Draw(gameTime);

                foreach (Teleporter t in teleporterList)
                    t.Draw(gameTime);

                foreach (Arrow a in arrowList)
                    a.Draw(gameTime);

                foreach (Bomb b in bombList)
                    b.Draw(gameTime);

                // Window boxes
                scoresWindowBox.Draw(GameRef.SpriteBatch);
                timerWindowBox.Draw(GameRef.SpriteBatch);

                #region Draw each players

                foreach (Player p in playerList)
                {
                    p.Draw(gameTime);

                    // HUD => Item Info
                    GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, Config.PlayersName[p.Id - 1] + ": "
                         + Config.PlayersScores[p.Id - 1] + " pt(s)",
                        new Vector2(hudOrigin.X + hudMarginLeft, hudOrigin.Y + hudTopSpace + (p.Id - 1) * Config.HUDPlayerInfoSpace), Color.Black);

                    if (Config.Debug)
                    {
                        GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, "Player " + p.Id + ": " + (p.Sprite.CellPosition).ToString(),
                            new Vector2(hudOrigin.X + hudMarginLeft, hudOrigin.Y + hudTopSpace + Config.HUDPlayerInfoSpace * Config.PlayersNumber + 60 + 20 * (p.Id - 1)), Color.Black);
                    }

                    // To space the red icons and the "normal color" icons
                    int iconLag = 0;

                    // Player's power
                    int counterRedFlames = 0;
                    if (p.Power >= 10)
                    {
                        iconLag = 10;
                        counterRedFlames = p.Power / 10;
                        for (int i = 0; i < counterRedFlames; i++)
                            GameRef.SpriteBatch.Draw(itemInfoIcon,
                                new Vector2(hudOrigin.X + hudMarginLeft + 7 * i,
                                hudOrigin.Y + hudTopSpace + (p.Id - 1) * Config.HUDPlayerInfoSpace + 30),
                                new Rectangle(14, 0, 14, 14), Color.White);
                    }
                    else
                        iconLag = 0;
                    int counterYellowFlames = p.Power % 10;
                    for (int i = 0; i < counterYellowFlames; i++)
                        GameRef.SpriteBatch.Draw(itemInfoIcon,
                            new Vector2(hudOrigin.X + hudMarginLeft + 14 * counterRedFlames + 7 * i + iconLag,
                                hudOrigin.Y + hudTopSpace + (p.Id - 1) * Config.HUDPlayerInfoSpace + 30),
                            new Rectangle(0, 0, 14, 14), Color.White);

                    // Player's bomb number
                    int counterRedBombs = 0;
                    if (p.CurrentBombNumber >= 10)
                    {
                        iconLag = 10;
                        counterRedBombs = p.CurrentBombNumber / 10;
                        for (int i = 0; i < counterRedBombs; i++)
                        {
                            GameRef.SpriteBatch.Draw(itemInfoIcon, new Vector2(hudOrigin.X + hudMarginLeft + 7 * i,
                                hudOrigin.Y + hudTopSpace + (p.Id - 1) * Config.HUDPlayerInfoSpace + 50),
                                new Rectangle(42, 0, 14, 14), Color.White);
                        }
                    }
                    else
                        iconLag = 0;
                    int counterBlackBombs = p.CurrentBombNumber % 10;
                    int finalCounter = 0;
                    for (int i = 0; i < counterBlackBombs; i++)
                    {
                        GameRef.SpriteBatch.Draw(itemInfoIcon,
                            new Vector2(hudOrigin.X + 7 * counterRedBombs + hudMarginLeft + 7 * i + iconLag, hudOrigin.Y +
                                hudTopSpace + (p.Id - 1) * Config.HUDPlayerInfoSpace + 50),
                            new Rectangle(28, 0, 14, 14), Color.White);
                        finalCounter = i;
                    }

                    if (p.HasBadItemEffect && p.BadItemEffect == BadItemEffect.NoBomb)
                    {
                        GameRef.SpriteBatch.Draw(cross, new Rectangle(hudOrigin.X + hudMarginLeft,
                            hudOrigin.Y + hudTopSpace + (p.Id - 1) * Config.HUDPlayerInfoSpace + 50,
                            7 * counterRedBombs + 7 * finalCounter + iconLag + 15, 15), Color.White);
                    }

                    // Player's speed
                    int counterRedShoes = 0;
                    if (p.Sprite.Speed / Config.PlayerSpeedIncrementeur >= 10)
                    {
                        iconLag = 10;
                        counterRedShoes = (int)(p.Sprite.Speed / Config.PlayerSpeedIncrementeur) / 10;
                        for (int i = 0; i < counterRedShoes; i++)
                            GameRef.SpriteBatch.Draw(itemInfoIcon, new Vector2(hudOrigin.X + hudMarginLeft + 7 * i,
                                hudOrigin.Y + hudTopSpace + (p.Id - 1) * Config.HUDPlayerInfoSpace + 70),
                                new Rectangle(70, 0, 14, 14), Color.White);
                    }
                    else
                        iconLag = 0;
                    int counterBlueShoes = (int)(p.Sprite.Speed / Config.PlayerSpeedIncrementeur) % 10;

                    for (int i = 0; i < counterBlueShoes; i++)
                        GameRef.SpriteBatch.Draw(itemInfoIcon,
                            new Vector2(hudOrigin.X + hudMarginLeft + counterRedShoes * 7 + 7 * i + iconLag,
                                hudOrigin.Y + hudTopSpace + (p.Id - 1) * Config.HUDPlayerInfoSpace + 70),
                            new Rectangle(56, 0, 14, 14), Color.White);

                    // Player's bad item timer
                    if (p.HasBadItemEffect)
                    {
                        GameRef.SpriteBatch.Draw(itemInfoIcon, new Vector2(hudOrigin.X + hudMarginLeft,
                            hudOrigin.Y + hudTopSpace + (p.Id - 1) * Config.HUDPlayerInfoSpace + 90),
                                    new Rectangle(98, 0, 14, 14), Color.White);

                        int lenght = 200 - (int)((200 / p.BadItemTimerLenght.TotalSeconds) * p.BadItemTimer.TotalSeconds);
                        string badItemTimer = ((int)Math.Round(p.BadItemTimerLenght.TotalSeconds - p.BadItemTimer.TotalSeconds) + 1).ToString();
                        if (p.BadItemEffect == BadItemEffect.BombTimerChanged)
                            badItemTimer += " (" + p.BombTimer.TotalSeconds.ToString() + ")";
                        for (int i = 0; i < lenght; i++)
                        {
                            GameRef.SpriteBatch.Draw(badItemTimerBar,
                                new Vector2(hudOrigin.X + hudMarginLeft + 20 + 1 * i,
                                    hudOrigin.Y + hudTopSpace + (p.Id - 1) * Config.HUDPlayerInfoSpace + 90), Color.White);
                        }
                        GameRef.SpriteBatch.DrawString(smallFont, badItemTimer,
                            new Vector2(hudOrigin.X + hudMarginLeft + 20 + 1 * (lenght / 2) - smallFont.MeasureString(badItemTimer).X / 2,
                                hudOrigin.Y + hudTopSpace + (p.Id - 1) * Config.HUDPlayerInfoSpace + 90), Color.White);
                    }
                    else
                    {
                        GameRef.SpriteBatch.Draw(itemInfoIcon, new Vector2(hudOrigin.X + hudMarginLeft,
                            hudOrigin.Y + hudTopSpace + (p.Id - 1) * Config.HUDPlayerInfoSpace + 90),
                                    new Rectangle(84, 0, 14, 14), Color.White);
                    }
                }
                #endregion

                #endregion

                if (Config.ActiveSuddenDeath)
                    suddenDeath.Draw(gameTime);
            }
            
            // FPS
            //GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, fps.ToString(), Vector2.Zero, Color.Red);
            // Timer
            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, timer.ToString("mm") + ":" + timer.ToString("ss"),
                new Vector2(hudOrigin.X + hudMarginLeft + (ControlManager.SpriteFont.MeasureString(timer.ToString("mm") + ":" + timer.ToString("ss")).X) + 25,
                    hudOrigin.Y + hudTopSpace + Config.HUDPlayerInfoSpace * Config.PlayersNumber + 22), Color.Black);


            if (pause)
            {
                GameRef.SpriteBatch.DrawString(BigFont, "PAUSE",
                    new Vector2(GameRef.GraphicsDevice.Viewport.Width / 2 - gameFont.MeasureString("PAUSE").X / 2,
                        GameRef.GraphicsDevice.Viewport.Height / 2 - gameFont.MeasureString("PAUSE").Y / 2),
                    Color.Blue);
            }

            if (playerList[playerList.Count - 1].aiWay != null && playerList[playerList.Count - 1].aiWay.Count != 0)
            {
                foreach (Point p in playerList[playerList.Count - 1].aiWay)
                {
                    GameRef.SpriteBatch.Draw(cross, new Rectangle((int)(Engine.CellToVector(p).X + Engine.Origin.X), 
                        (int)(Engine.CellToVector(p).Y + Engine.Origin.Y), 32, 32), 
                        new Rectangle(0, 0, cross.Width, cross.Height), Color.White);
                }
            }

            GameRef.SpriteBatch.End();
        }

        #endregion

        #region Method Region

        #region Initialization Method Region
        public void Reset()
        {
            MediaPlayer.IsRepeating = true;
            songNumber = random.Next(mapSong.Length);
            MediaPlayer.Play(mapSong[songNumber]);

            timer = TimeSpan.Zero;

            engine = new Engine(32, 32, Vector2.Zero);

            // Lists
            wallList = new List<Wall>();
            itemList = new List<Item>();
            bombList = new List<Bomb>();
            playerList = new List<Player>();
            unbreakableWallList = new List<UnbreakableWall>();
            edgeWallList = new List<EdgeWall>();
            teleporterList = new List<Teleporter>();
            arrowList = new List<Arrow>();

            deadPlayersNumber = 0;

            createWorld();
            //ParseMap("classic.map");

            Vector2 origin = new Vector2(hudOrigin.X / 2 - ((32 * world.Levels[world.CurrentLevel].Size.X) / 2),
                GameRef.GraphicsDevice.Viewport.Height / 2 - ((32 * world.Levels[world.CurrentLevel].Size.Y) / 2));

            Engine.Origin = origin;

            if(Config.ActiveSuddenDeath)
                suddenDeath = new SuddenDeath(GameRef, Config.PlayersPositions[0]);
        }

        private void createWorld()
        {
            List<Tileset> tilesets = new List<Tileset>(){new Tileset(mapTexture, 64, 32, 32, 32)};

            bool[,] collisionLayer = new bool[Config.MapSize.X, Config.MapSize.Y];
            int[,] mapPlayersPosition = new int[Config.MapSize.X, Config.MapSize.Y];
            MapItem[,] map = new MapItem[Config.MapSize.X, Config.MapSize.Y];
            MapLayer layer = new MapLayer(Config.MapSize.X, Config.MapSize.Y);
            List<Point> voidPosition = new List<Point>();

            // Item Map
            MapItem mapItem;

            // List of player position
            Dictionary<int, Point> playerPositions = new Dictionary<int, Point>();

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
            mapItem = new Teleporter(GameRef, new Vector2(
                        5 * Engine.TileWidth,
                        1 * Engine.TileHeight));
            teleporterList.Add((Teleporter)mapItem);
            map[5,1] = mapItem;

            mapItem = new Teleporter(GameRef, new Vector2(
                        10 * Engine.TileWidth,
                        1 * Engine.TileHeight));
            teleporterList.Add((Teleporter)mapItem);
            map[10, 1] = mapItem;
            */

            for (int x = 0; x < Config.MapSize.X; x++)
            {
                for (int y = 0; y < Config.MapSize.Y; y++)
                {
                    if(!(x == 0 || y == 0 || x == (Config.MapSize.X - 1) || y == (Config.MapSize.Y - 1) ||
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
                        randomVoid = random.Next(voidPosition.Count);
                        mapItem = new Teleporter(GameRef, new Vector2(
                            voidPosition[randomVoid].X * Engine.TileWidth,
                            voidPosition[randomVoid].Y * Engine.TileHeight));
                        teleporterList.Add((Teleporter)mapItem);
                        map[voidPosition[randomVoid].X, voidPosition[randomVoid].Y] = mapItem;
                        voidPosition.Remove(voidPosition[randomVoid]);
                    }
                }
                else if (Config.TeleporterPositionType == TeleporterPositionTypeEnum.PlusForm)
                {
                    Point[] teleporterPositions = new Point[]
                    {
                        new Point((int)Math.Ceiling((double)(Config.MapSize.X - 2)/(double)2), 1),
                        new Point(1, (int)Math.Ceiling((double)(Config.MapSize.Y - 2)/(double)2)),
                        new Point((int)Math.Ceiling((double)(Config.MapSize.X - 2)/(double)2), Config.MapSize.Y - 2),
                        new Point(Config.MapSize.X - 2, (int)Math.Ceiling((double)(Config.MapSize.Y - 2)/(double)2))
                    };

                    for (int i = 0; i < teleporterPositions.Length; i++)
                    {
                        mapItem = new Teleporter(GameRef, new Vector2(
                            teleporterPositions[i].X * Engine.TileWidth,
                            teleporterPositions[i].Y * Engine.TileHeight));
                        teleporterList.Add((Teleporter)mapItem);
                        map[teleporterPositions[i].X, teleporterPositions[i].Y] = mapItem;
                    }
                }
            }
            #endregion

            #region Arrow
            if (Config.ActiveArrows)
            {
                if (Config.ArrowPositionType == ArrowPositionTypeEnum.Randomly)
                {
                    LookDirection[] lookDirectionArray = new LookDirection[] { LookDirection.Up, LookDirection.Down, LookDirection.Left, LookDirection.Right };
                    int randomVoid = 0;
                    for (int i = 0; i < MathHelper.Clamp(Config.ArrowNumber, 0, voidPosition.Count - 1); i++)
                    {
                        randomVoid = random.Next(voidPosition.Count);
                        mapItem = new Arrow(GameRef, new Vector2(
                            voidPosition[randomVoid].X * Engine.TileWidth,
                            voidPosition[randomVoid].Y * Engine.TileHeight),
                            lookDirectionArray[random.Next(lookDirectionArray.Length)]);
                        arrowList.Add((Arrow)mapItem);
                        map[voidPosition[randomVoid].X, voidPosition[randomVoid].Y] = mapItem;
                        voidPosition.Remove(voidPosition[randomVoid]);
                    }
                }
                else if (Config.ArrowPositionType == ArrowPositionTypeEnum.SquareForm)
                {
                    int outsideArrowsLag = 0;
                    int ratio = (int)Math.Ceiling((double)(4 * (Config.MapSize.X - 2))/(double)5);
                    if (ratio % 2 == 0)
                        outsideArrowsLag = 1;

                    Point[] arrowPositions = new Point[]
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
                        mapItem = new Arrow(GameRef, new Vector2(
                            arrowPositions[i].X * Engine.TileWidth,
                            arrowPositions[i].Y * Engine.TileHeight),
                            Config.ArrowLookDirection[i % 4]);
                        arrowList.Add((Arrow)mapItem);
                        map[arrowPositions[i].X, arrowPositions[i].Y] = mapItem;
                    }
                }
            }
            #endregion

            int counter = 0;
            for (int x = 0; x < Config.MapSize.X; x++)
            {
                for (int y = 0; y < Config.MapSize.Y; y++)
                {
                    Tile tile = new Tile(0, 0);
                    if (x == 0 || y == 0 || x == (Config.MapSize.X - 1) || y == (Config.MapSize.Y - 1) ||
                        (x % 2 == 0 && y % 2 == 0))
                    {
                        // Inside wallList
                        if ((x % 2 == 0 && y % 2 == 0 && !(x == 0 || y == 0 || x == (Config.MapSize.X - 1) || y == (Config.MapSize.Y - 1))) && mapPlayersPosition[x, y] != 2)
                        {
                            mapItem = new UnbreakableWall(GameRef, new Vector2(x * Engine.TileHeight, y * Engine.TileWidth));
                            this.unbreakableWallList.Add((UnbreakableWall)mapItem);
                            map[x, y] = mapItem;
                            collisionLayer[x, y] = true;
                        }
                        // Outside wallList
                        else if (mapPlayersPosition[x, y] != 2)
                        {
                            mapItem = new EdgeWall(GameRef, new Vector2(x * Engine.TileHeight, y * Engine.TileWidth));
                            this.edgeWallList.Add((EdgeWall)mapItem);
                            counter++;
                            map[x, y] = mapItem;
                            collisionLayer[x, y] = true;
                        }
                    }
                    else
                    {
                        // Wall
                        if ((mapPlayersPosition[x, y] != 1 && mapPlayersPosition[x, y] != 2) && map[x, y] == null &&
                            random.Next(0, 100) < MathHelper.Clamp(Config.WallNumber, 0, 100))
                        {
                            collisionLayer[x, y] = true;
                            mapItem = new Wall(GameRef, new Vector2(x * Engine.TileWidth, y * Engine.TileHeight));
                            wallList.Add((Wall)mapItem);
                            map[x, y] = mapItem;
                        }
                        //tile = new Tile(0, 0);
                    }
                    layer.SetTile(x, y, tile);
                }
            }

            List<MapLayer> mapLayers = new List<MapLayer>();
            mapLayers.Add(layer);

            TileMap tileMap = new TileMap(tilesets, mapLayers);
            Level level = new Level(Config.MapSize, tileMap, map, collisionLayer);

            world = new World(GameRef, GameRef.ScreenRectangle);
            world.Levels.Add(level);
            world.CurrentLevel = 0;

            foreach (int playerId in playerPositions.Keys)
            {
                Player player = new Player(Math.Abs(playerId), GameRef,
                    Engine.CellToVector(new Point(playerPositions[playerId].X, playerPositions[playerId].Y)));
                playerList.Add(player);
                map[playerPositions[playerId].X, playerPositions[playerId].Y] = player;

            }
        }

        private void ParseMap(string file)
        {
            try
            {
                StreamReader streamReader = new StreamReader("Content/Maps/" + file);
                string line = streamReader.ReadLine();
                string[] lineSplit = line.Split(' ');
                int[] Mapsize = new int[] { int.Parse(lineSplit[0]), int.Parse(lineSplit[1]) };

                Point mapSize = new Point(Mapsize[0], Mapsize[1]);
                List<Tileset> tilesets = new List<Tileset>() { new Tileset(mapTexture, 64, 32, 32, 32) };

                bool[,] collisionLayer = new bool[mapSize.X, mapSize.Y];
                int[,] mapPlayersPosition = new int[mapSize.X, mapSize.Y];
                MapItem[,] map = new MapItem[mapSize.X, mapSize.Y];
                MapLayer layer = new MapLayer(mapSize.X, mapSize.Y);
                List<Point> voidPosition = new List<Point>();
                Dictionary<int, Point> playerPositions = new Dictionary<int, Point>();

                int j = 0;
                int id = 0;
                while (!streamReader.EndOfStream)
                {
                    line = streamReader.ReadLine();
                    lineSplit = line.Split(' ');
                    for (int i = 0; i < lineSplit.Length; i++)
                    {
                        id = int.Parse(lineSplit[i]);
                        switch(id)
                        {
                            case 1:
                                UnbreakableWall unbreakableWall = new UnbreakableWall(GameRef, Engine.CellToVector(new Point(i, j)));
                                map[i, j] = unbreakableWall;
                                unbreakableWallList.Add(unbreakableWall);
                                collisionLayer[i, j] = true;
                                break;
                            case 2:
                                EdgeWall edgeWall = new EdgeWall(GameRef, Engine.CellToVector(new Point(i, j)));
                                map[i, j] = edgeWall;
                                edgeWallList.Add(edgeWall);
                                collisionLayer[i, j] = true;
                                break;
                            case 3:
                                Wall wall = new Wall(GameRef, Engine.CellToVector(new Point(i, j)));
                                wallList.Add(wall);
                                map[i, j] = wall;
                                collisionLayer[i, j] = true;
                                break;
                            case 6:
                                Teleporter teleporter = new Teleporter(GameRef, Engine.CellToVector(new Point(i, j)));
                                map[i, j] = teleporter;
                                teleporterList.Add(teleporter);
                                break;
                            case 7:
                                Arrow arrow = new Arrow(GameRef, Engine.CellToVector(new Point(i, j)), LookDirection.Down);
                                arrowList.Add(arrow);
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

                List<MapLayer> mapLayers = new List<MapLayer>();
                mapLayers.Add(layer);

                TileMap tileMap = new TileMap(tilesets, mapLayers);
                Level level = new Level(mapSize, tileMap, map, collisionLayer);

                world = new World(GameRef, GameRef.ScreenRectangle);
                world.Levels.Add(level);
                world.CurrentLevel = 0;

                foreach (int playerId in playerPositions.Keys)
                {
                    Player player = new Player(Math.Abs(playerId), GameRef, 
                        Engine.CellToVector(new Point(playerPositions[playerId].X, playerPositions[playerId].Y)));
                    playerList.Add(player);
                    map[playerPositions[playerId].X, playerPositions[playerId].Y] = player;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion

        #endregion
    }
}
