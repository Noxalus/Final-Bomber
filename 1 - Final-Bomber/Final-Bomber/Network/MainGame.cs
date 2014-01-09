using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FBClient.Network
{
    /*
    public partial class MainGame : E2D_Map
    {
        #region Variables
        Cue maintheme;

        Timer tmr_UntilStart = new Timer();
        bool hasStarted = false;

        public int suddenDeathTime;
        TimeSpan untilSD;
        StatusPic suddenDeath;
        bool suddenDeathStarted = false;
        Timer tmr_SuddenDeath = new Timer();

        Map currentMap;
        PlayerCollection Players = new PlayerCollection();
        BombCollection Bombs = new BombCollection();
        ExplosionCollection Explosions = new ExplosionCollection();
        PowerupCollection Powerups = new PowerupCollection();
        public ThisPlayer me;
        public bool Spectator = false;

        bool shouldEnd = false;
        Timer endTmr;
        bool haveWon = false;
        StatusPic win;
        StatusPic lost;
        int moveCameraSpeed = 5;

        public int MoveCameraSpeed
        {
            get { return moveCameraSpeed; }
            set
            {
                moveCameraSpeed = value;
                if (value < 1)
                    moveCameraSpeed = 1;
                if (value > 50)
                    moveCameraSpeed = 50;
            }
        }

        #endregion

        public void Load(Map map)
        {
            #region Background
            E2D_Texture2D tstar = E2D_Engine.TextureManager.GetTextureOrLoad("Entities\\Stars");
            for (int x = 0; x < 1500; x += tstar.Texture.Width)
            {
                for (int y = 0; y < 1000; y += tstar.Texture.Height)
                {
                    E2D_Entity star = new E2D_Entity(E2D_Entity.LayerStatus.Background);
                    star.AbsolutePosition = true;
                    star.Initialize(tstar);
                    star.Position = new Vector2(x, y);
                    Entities.Add(star);
                }
            }
            #endregion

            #region Map
            currentMap = map;
            if (!map.hasLoad)
            {
                currentMap.Load();
            }
            else
            {
                currentMap.Reload();
            }
            foreach (MapTile maptile in currentMap.MapTiles)
            {
                Entities.Add(maptile);
                if (maptile.CurrentTile == MapTile.MapTiles.StartFloor) //Ändrar startfloors till vanligta gråa floors
                    ((Floor)maptile).ChangeColor(Floor.FloorColor.Gray);
            }
            #endregion

            #region Players
            me = new ThisPlayer();
            me.PlayerName = GameSettings.Username;
            Entities.Add(me);
            Players.Add(me);
            #endregion

            #region EndMessage
            win = new StatusPic(E2D_Engine.TextureManager.GetTextureOrLoad("Entities\\Win"), 2000,
                new Vector2(E2D_Engine.GetGraphicManager.PreferredBackBufferWidth / 2 - 150,
                    E2D_Engine.GetGraphicManager.PreferredBackBufferHeight / 2 - 75));
            Entities.Add(win);
            lost = new StatusPic(E2D_Engine.TextureManager.GetTextureOrLoad("Entities\\Defeat"), 2000,
                new Vector2(E2D_Engine.GetGraphicManager.PreferredBackBufferWidth / 2 - 150,
                    E2D_Engine.GetGraphicManager.PreferredBackBufferHeight / 2 - 75));
            Entities.Add(lost);
            #endregion

            #region SuddenDeath
            int height = E2D_Engine.GetGraphicManager.PreferredBackBufferHeight / 2;
            suddenDeath = new StatusPic(E2D_Engine.TextureManager.GetTextureOrLoad("Entities\\SuddenDeath"), 4000,
                new Vector2(E2D_Engine.GetGraphicManager.PreferredBackBufferWidth / 2 - 350, height - 94.5f - (height / 2) / 2));
            Entities.Add(suddenDeath);
            #endregion

            maintheme = E2D_Engine.SoundBank.GetCue("maintheme");

            GameSettings.GameServer.NewPlayer += new GameServer.NewPlayerEventHandler(GameServer_NewPlayer);
            GameSettings.GameServer.RemovePlayer += new GameServer.RemovePlayerEventHandler(GameServer_RemovePlayer);
            GameSettings.GameServer.MovePlayer += new GameServer.MovePlayerEventHandler(GameServer_MovePlayer);
            GameSettings.GameServer.PlacingBomb += new GameServer.PlacingBombEventHandler(GameServer_PlacingBomb);
            GameSettings.GameServer.BombExploded += new GameServer.BombExplodedEventHandler(GameServer_BombExploded);
            GameSettings.GameServer.Burn += new GameServer.BurnEventHandler(GameServer_Burn);
            GameSettings.GameServer.ExplodeTile += new GameServer.ExplodeTileEventHandler(GameServer_ExplodeTile);
            GameSettings.GameServer.PowerupDrop += new GameServer.PowerupDropEventHandler(GameServer_PowerupDrop);
            GameSettings.GameServer.PowerupPick += new GameServer.PowerupPickEventHandler(GameServer_PowerupPick);
            GameSettings.GameServer.SuddenDeath += new GameServer.SuddenDeathEventHandler(GameServer_SuddenDeath);
            GameSettings.GameServer.SDExplosion += new GameServer.SDExplosionEventHandler(GameServer_SDExplosion);
            GameSettings.GameServer.End += new GameServer.EndEventHandler(GameServer_End);

            endTmr = new Timer();
            base.Load();
        }

        public override void Start()
        {
            base.Start(1000, new Vector2(currentMap.mapWidth * MapTile.SIZE, currentMap.mapHeight * MapTile.SIZE));

            E2D_Engine.Camera.KeepWithinMapX = false;
            E2D_Engine.Camera.KeepWithinMapY = false;

            tmr_UntilStart.Start();
            untilSD = new TimeSpan(0, 0, suddenDeathTime / 1000);

            win.Hide();
            lost.Hide();
            suddenDeath.Hide();
            maintheme.Play();
        }

        public override void End()
        {
            maintheme.Stop(AudioStopOptions.Immediate);
            base.End();
        }

        public override void Unload()
        {
            GameSettings.GameServer.NewPlayer -= new GameServer.NewPlayerEventHandler(GameServer_NewPlayer);
            GameSettings.GameServer.RemovePlayer -= new GameServer.RemovePlayerEventHandler(GameServer_RemovePlayer);
            GameSettings.GameServer.MovePlayer -= new GameServer.MovePlayerEventHandler(GameServer_MovePlayer);
            GameSettings.GameServer.PlacingBomb -= new GameServer.PlacingBombEventHandler(GameServer_PlacingBomb);
            GameSettings.GameServer.BombExploded -= new GameServer.BombExplodedEventHandler(GameServer_BombExploded);
            GameSettings.GameServer.Burn -= new GameServer.BurnEventHandler(GameServer_Burn);
            GameSettings.GameServer.ExplodeTile -= new GameServer.ExplodeTileEventHandler(GameServer_ExplodeTile);
            GameSettings.GameServer.PowerupDrop -= new GameServer.PowerupDropEventHandler(GameServer_PowerupDrop);
            GameSettings.GameServer.PowerupPick -= new GameServer.PowerupPickEventHandler(GameServer_PowerupPick);
            GameSettings.GameServer.SuddenDeath -= new GameServer.SuddenDeathEventHandler(GameServer_SuddenDeath);
            GameSettings.GameServer.SDExplosion -= new GameServer.SDExplosionEventHandler(GameServer_SDExplosion);
            GameSettings.GameServer.End -= new GameServer.EndEventHandler(GameServer_End);
            base.Unload();
        }

        public override void Update(GameTime gameTime)
        {
            #region MoveCamera
            if (me.IsDead)
            {
                KeyboardState state = E2D_Engine.GetKeysState;
                Keys[] keys = state.GetPressedKeys();
                if (keys.Length > 0)
                {
                    switch (keys[0])
                    {
                        case Keys.Left:
                            E2D_Engine.Camera.Position += new Vector2(-MoveCameraSpeed, 0);
                            break;
                        case Keys.Right:
                            E2D_Engine.Camera.Position += new Vector2(MoveCameraSpeed, 0);
                            break;
                        case Keys.Up:
                            E2D_Engine.Camera.Position += new Vector2(0, -MoveCameraSpeed);
                            break;
                        case Keys.Down:
                            E2D_Engine.Camera.Position += new Vector2(0, MoveCameraSpeed);
                            break;
                        case Keys.Add:
                            MoveCameraSpeed++;
                            break;
                        case Keys.Subtract:
                            MoveCameraSpeed--;
                            break;
                    }
                }
            }
            else
            {
                E2D_Engine.Camera.CenterCamera(me.MapPosition);
            }
            #endregion

            #region RemoveExplosions
            for (int i = 0; i < Explosions.Count; i++)
            {
                if (Explosions[i].Dispose)
                {
                    Entities.Remove(Explosions[i]);
                    Explosions.RemoveAt(i);
                }
            }
            #endregion

            #region Ending
            if (shouldEnd)
            {
                if (haveWon)
                {
                    if (!win.IsShowed)
                        win.Show();
                }
                else
                {
                    if (!lost.IsShowed)
                        lost.Show();
                }
            }
            if (endTmr.Each(2000))
            {
                endTmr.Reset();
                End();
                Unload();
            }
            #endregion

            #region SuddenDeathTimer
            if (tmr_SuddenDeath.Each(1000))
                untilSD = untilSD.Subtract(new TimeSpan(0, 0, 1));
            #endregion

            #region Sound
            KeyboardState newstate = E2D_Engine.GetKeysState;
            KeyboardState oldstate = E2D_Engine.GetOldKeysState;
            if (newstate.IsKeyDown(Keys.Tab) && oldstate.IsKeyUp(Keys.Tab))
            {
                if (maintheme.IsPaused)
                {
                    maintheme.Resume();
                }
                else
                {
                    maintheme.Pause();
                }
            }
            #endregion

            if (hasStarted)
                base.Update(gameTime);

            if (!hasStarted && tmr_UntilStart.Each(3000))
            {
                hasStarted = true;
                tmr_UntilStart.Stop();
                tmr_SuddenDeath.Start();
            }
        }

        public override void InFrontOfDrawing()
        {
            E2D_Engine.GetSpriteBatch.DrawString(E2D_Engine.Special.Debug_Font, GameSettings.GameServer.Ping.ToString(),
                    new Vector2(E2D_Engine.ScreenWidth - 50, 10), Color.Blue);

            if (!Spectator)
            {
                if (me.IsDead)
                    E2D_Engine.GetSpriteBatch.DrawString(E2D_Engine.Special.Debug_Font, "Camera Speed: " +
                        moveCameraSpeed.ToString(), new Vector2(50, 50), Color.Red);

                if (!suddenDeathStarted && hasStarted)
                {
                    string output = "Sudden Death: " + untilSD.Minutes.ToString() + ":" + untilSD.Seconds.ToString();
                    E2D_Engine.GetSpriteBatch.DrawString(E2D_Engine.Special.Debug_Font, output
                        , new Vector2(E2D_Engine.GetGraphicManager.PreferredBackBufferWidth / 2 - E2D_Engine.Special.Debug_Font.MeasureString(output).X / 2, 100),
                        Color.Red);
                }

                if (!hasStarted)
                {
                    string output = "Start game in " + (3 - (tmr_UntilStart.ElapsedMilliseconds / 1000)).ToString();
                    E2D_Engine.GetSpriteBatch.DrawString(E2D_Engine.Special.Debug_Font, output
                        , new Vector2(E2D_Engine.GetGraphicManager.PreferredBackBufferWidth / 2 - E2D_Engine.Special.Debug_Font.MeasureString(output).X / 2, 100),
                        Color.Red);
                }
            }
        }

        #region ServerEvents
        private void GameServer_NewPlayer(int playerID, float moveSpeed, string username)
        {
            if (Players.GetPlayerByID(playerID) == null)
            {
                Player player = new Player();
                player.PlayerID = playerID;
                player.MoveSpeed = moveSpeed;
                player.PlayerName = username;
                Entities.Add(player);
                Players.Add(player);
            }
        }

        private void GameServer_RemovePlayer(int playerID)
        {
            Player player = Players.GetPlayerByID(playerID);
            if (player != null && me.PlayerID != playerID)
            {
                Players.Remove(player);
                Entities.Remove(player);
            }
        }

        private void GameServer_MovePlayer(object sender, MovePlayerArgs arg)
        {
            Player player = Players.GetPlayerByID(arg.playerID);
            if (player != null)
            {
                player.MapPosition = arg.pos;
                if (arg.action != 255)
                    player.movementAction = (Player.ActionEnum)arg.action;
                player.UpdateAnimation();
            }
        }

        private void GameServer_PlacingBomb(int playerId, float xPos, float yPos)
        {
            Player player = Players.GetPlayerByID(playerId);
            if (player != null)
            {
                Bomb bomb = new Bomb(new Vector2(xPos, yPos), player.BombTicksPerSek, (player != me));
                Entities.Add(bomb);
                Bombs.Add(bomb);
            }
        }

        private void GameServer_BombExploded(float xPos, float yPos, List<Explosion> explosions)
        {
            Bomb bomb = Bombs.GetBombByPos(xPos, yPos);
            Bombs.Remove(bomb);
            Entities.Remove(bomb);
            foreach (Explosion ex in explosions)
            {
                //Ser till att explosionerna smällter ihop på ett snyggt sätt
                if (ex.Type == Explosion.ExplosionType.Down || ex.Type == Explosion.ExplosionType.Left
                        || ex.Type == Explosion.ExplosionType.Right || ex.Type == Explosion.ExplosionType.Up)
                {
                    Explosion temp_ex = Explosions.GetExplosionAtPosition(ex.originalPos, true);
                    if (temp_ex != null)
                    {
                        if (temp_ex.Type != ex.Type && Explosion.ConvertToOpposit(temp_ex.Type) != ex.Type)
                        {
                            Explosion temp_ex2 = new Explosion(ex.originalPos, Explosion.ExplosionType.Mid, true);
                            temp_ex2.explosionExistanceTime -= (int)temp_ex.tmrEnd.ElapsedMilliseconds;
                            Explosions.Add(temp_ex2);
                            Entities.Add(temp_ex2);
                        }
                    }
                }
                //Lägger till explosionerna till listorna
                Explosions.Add(ex);
                Entities.Add(ex);
            }
        }

        private void GameServer_Burn(int playerId)
        {
            if (Players.GetPlayerByID(playerId) != null)
            {
                Players.GetPlayerByID(playerId).Burn();
            }
        }

        private void GameServer_ExplodeTile(int tilePos)
        {
            if (tilePos > -1 && tilePos < currentMap.MapTiles.Count)
            {
                Entities.Remove(currentMap.MapTiles[tilePos]);
                currentMap.ExplodeTile(tilePos);
                Entities.Add(currentMap.MapTiles[tilePos]);
            }
        }

        private void GameServer_PowerupDrop(Powerup.PowerupType type, float xPos, float yPos)
        {
            if (Powerups.GetPowerupFromPos(new Vector2(xPos, yPos)) == null)
            {
                Powerup powerup = new Powerup(type, new Vector2(xPos, yPos));
                Powerups.Add(powerup);
                Entities.Add(powerup);
            }
        }

        private void GameServer_PowerupPick(float xPos, float yPos, int playerId, float amount)
        {
            Powerup powerup = Powerups.GetPowerupFromPos(new Vector2(xPos, yPos));
            Player player = Players.GetPlayerByID(playerId);
            if (powerup != null)
            {
                if (player != null)
                {
                    powerup.GetPowerup(player, amount);
                    Powerups.Remove(powerup);
                    Entities.Remove(powerup);
                }
            }
        }

        private void GameServer_SuddenDeath()
        {
            suddenDeath.Show();
            suddenDeathStarted = true;
            me.Lifes = 1;
        }

        private void GameServer_SDExplosion(int tilePos)
        {
            MapTile tile = currentMap.MapTiles[tilePos];
            Entities.Remove(tile);
        }

        private void GameServer_End(bool Won)
        {
            endTmr.Start();
            if (!Spectator)
            {
                shouldEnd = true;
                haveWon = Won;
            }
        }

        #endregion
    }
    */
}
