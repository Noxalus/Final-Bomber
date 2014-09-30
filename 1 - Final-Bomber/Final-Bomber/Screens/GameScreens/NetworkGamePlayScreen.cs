using System;
using System.Diagnostics;
using FBLibrary;
using FBLibrary.Core;
using FBClient.Controls;
using FBClient.Core;
using FBClient.GUI;
using FBClient.Network;
using FBClient.WorldEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FBClient.Screens.GameScreens
{
    public class NetworkGamePlayScreen : BaseGamePlayScreen
    {
        #region Field region
        // Game manager
        private NetworkGameManager _gameManager;

        Process _serverProcess;
        
        #endregion

        #region Constructor region
        public NetworkGamePlayScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
        }
        #endregion

        #region XNA Method region

        public override void Initialize()
        {
            HudOrigin = new Point(FinalBomber.Instance.GraphicsDevice.Viewport.Width - 234, 0);

            // Launch the dedicated server as host
            _serverProcess = new Process
            {
                StartInfo =
                {
                    FileName = "Server.exe",
                    Arguments = "COUCOU",
                    //WindowStyle = ProcessWindowStyle.Hidden 
                }
            };

            //_serverProcess.Start();

            _gameManager = GameServer.Instance.GameManager;

            _gameManager.AddPlayers();

            base.Initialize();

            // Server events
            GameServer.Instance.End += GameServer_End;

            HudOrigin = new Point(GraphicsDevice.Viewport.Width - 234, 0);
            HudTopSpace = 15;
            HudMarginLeft = 15;

            ScoresWindowBox = new WindowBox(WindowSkin, new Vector2(HudOrigin.X, HudOrigin.Y),
                                             new Point(GraphicsDevice.Viewport.Width - (HudOrigin.X),
                                             HudTopSpace + _gameManager.Players.Count * Config.HUDPlayerInfoSpace + 15));

            TimerWindowBox = new WindowBox(WindowSkin, new Vector2(HudOrigin.X, ScoresWindowBox.Size.Y),
                new Point(GraphicsDevice.Viewport.Width - HudOrigin.X, 40));

            _gameManager.NetworkManager.AddPlayer += ResizeHud;
        }

        protected override void LoadContent()
        {
            _gameManager.LoadContent();

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            //_serverProcess.Kill();

            _gameManager.NetworkManager.AddPlayer -= ResizeHud;

            GameServer.Instance.End -= GameServer_End;

            _gameManager.Dispose();

            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            _gameManager.Update();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            FinalBomber.Instance.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, _gameManager.Camera.GetTransformation());

            _gameManager.Draw(gameTime, _gameManager.Camera);

            FinalBomber.Instance.SpriteBatch.End();

            FinalBomber.Instance.SpriteBatch.Begin();

            const string str = "Networking Tests";
            FinalBomber.Instance.SpriteBatch.DrawString(BigFont, str,
                    new Vector2(
                        Config.Resolutions[Config.IndexResolution, 0] / 2f -
                        this.BigFont.MeasureString(str).X / 2 - 75,
                        0),
            Color.Black);

            // Players ping
            for (int i = 0; i < GameServer.Instance.Clients.Count; i++)
            {
                string ping = "Ping: " + GameServer.Instance.Clients[i].GetRoundedPing();

                FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, ping, new Vector2(1, 21 + (20 * i)), Color.Black);
                FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, ping, new Vector2(0, 20 + (20 * i)), Color.White);
            }

            /*
            // Draw IP adress
            FinalBomber.Instance.SpriteBatch.DrawString(BigFont, _networkManager.PublicIp, new Vector2(530, 60), Color.Black);

            // Draw ping
            FinalBomber.Instance.SpriteBatch.DrawString(BigFont, GameServer.Ping.ToString(), new Vector2(740, 100), Color.Black);
            */
            /*
            int counter = 0;
            foreach (var player in GameManager.Players)
            {
                FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, "Player #" + player.Id + ": (" + player.IsChangingCell() +
                    ")\nR: " + player.Position +
                    "\nP: " + player.CellPosition, new Vector2(530, 60 + (100 * counter)), Color.Black);

                counter++;
            }
            */
            #region HUD
            // Window boxes
            ScoresWindowBox.Draw(FinalBomber.Instance.SpriteBatch);
            TimerWindowBox.Draw(FinalBomber.Instance.SpriteBatch);
            
            // Draw player HUD
            foreach (var p in _gameManager.Players)
            {
                // HUD => Item Info
                FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, p.Name + ": "
                     + p.Stats.Score + " | " + _gameManager.GetClientFromPlayer(p).GetRoundedPing(),
                    new Vector2(HudOrigin.X + HudMarginLeft, HudOrigin.Y + HudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace), Color.Black);

#if DEBUG
                FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, "Player " + p.Id + ": " + (p.CellPosition).ToString(),
                    new Vector2(HudOrigin.X + HudMarginLeft, HudOrigin.Y + HudTopSpace + Config.HUDPlayerInfoSpace * _gameManager.Players.Count + 60 + 20 * (p.Id)), Color.Black);
#endif

                // To space the red icons and the "normal color" icons
                int iconLag = 0;

                // Player's power
                int counterRedFlames = 0;
                if (p.BombPower >= 10)
                {
                    iconLag = 10;
                    counterRedFlames = p.BombPower / 10;
                    for (int i = 0; i < counterRedFlames; i++)
                        FinalBomber.Instance.SpriteBatch.Draw(ItemInfoIcon,
                            new Vector2(HudOrigin.X + HudMarginLeft + 7 * i,
                            HudOrigin.Y + HudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 30),
                            new Rectangle(14, 0, 14, 14), Color.White);
                }
                else
                    iconLag = 0;
                int counterYellowFlames = p.BombPower % 10;
                for (int i = 0; i < counterYellowFlames; i++)
                    FinalBomber.Instance.SpriteBatch.Draw(ItemInfoIcon,
                        new Vector2(HudOrigin.X + HudMarginLeft + 14 * counterRedFlames + 7 * i + iconLag,
                            HudOrigin.Y + HudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 30),
                        new Rectangle(0, 0, 14, 14), Color.White);

                // Player's bomb number
                int counterRedBombs = 0;
                if (p.CurrentBombAmount >= 10)
                {
                    iconLag = 10;
                    counterRedBombs = p.CurrentBombAmount / 10;
                    for (int i = 0; i < counterRedBombs; i++)
                    {
                        FinalBomber.Instance.SpriteBatch.Draw(ItemInfoIcon, new Vector2(HudOrigin.X + HudMarginLeft + 7 * i,
                            HudOrigin.Y + HudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 50),
                            new Rectangle(42, 0, 14, 14), Color.White);
                    }
                }
                else
                    iconLag = 0;
                int counterBlackBombs = p.CurrentBombAmount % 10;
                int finalCounter = 0;
                for (int i = 0; i < counterBlackBombs; i++)
                {
                    FinalBomber.Instance.SpriteBatch.Draw(ItemInfoIcon,
                        new Vector2(HudOrigin.X + 7 * counterRedBombs + HudMarginLeft + 7 * i + iconLag, HudOrigin.Y +
                            HudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 50),
                        new Rectangle(28, 0, 14, 14), Color.White);
                    finalCounter = i;
                }

                if (p.HasBadEffect && p.BadEffect == BadEffect.NoBomb)
                {
                    FinalBomber.Instance.SpriteBatch.Draw(Cross, new Rectangle(HudOrigin.X + HudMarginLeft,
                        HudOrigin.Y + HudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 50,
                        7 * counterRedBombs + 7 * finalCounter + iconLag + 15, 15), Color.White);
                }

                // Player's speed
                int counterRedShoes = 0;
                var counterIncrementeur = (int)((p.Speed - GameConfiguration.BasePlayerSpeed) / (GameConfiguration.BasePlayerSpeed * (GameConfiguration.PlayerSpeedIncrementeurPercentage / 100))) + 1;
                if (counterIncrementeur >= 10)
                {
                    iconLag = 10;
                    counterRedShoes = (int)(counterIncrementeur) / 10;
                    for (int i = 0; i < counterRedShoes; i++)
                        FinalBomber.Instance.SpriteBatch.Draw(ItemInfoIcon, new Vector2(HudOrigin.X + HudMarginLeft + 7 * i,
                            HudOrigin.Y + HudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 70),
                            new Rectangle(70, 0, 14, 14), Color.White);
                }
                else
                    iconLag = 0;
                int counterBlueShoes = (int)(counterIncrementeur) % 10;

                for (int i = 0; i < counterBlueShoes; i++)
                    FinalBomber.Instance.SpriteBatch.Draw(ItemInfoIcon,
                        new Vector2(HudOrigin.X + HudMarginLeft + counterRedShoes * 7 + 7 * i + iconLag,
                            HudOrigin.Y + HudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 70),
                        new Rectangle(56, 0, 14, 14), Color.White);

                // Player's bad item GameTimer
                if (p.HasBadEffect)
                {
                    FinalBomber.Instance.SpriteBatch.Draw(ItemInfoIcon, new Vector2(HudOrigin.X + HudMarginLeft,
                        HudOrigin.Y + HudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 90),
                                new Rectangle(98, 0, 14, 14), Color.White);

                    int lenght = 200 - (int)((200 / p.BadEffectTimerLenght.TotalSeconds) * p.BadEffectTimer.TotalSeconds);
                    string badItemTimer = ((int)Math.Round(p.BadEffectTimerLenght.TotalSeconds - p.BadEffectTimer.TotalSeconds) + 1).ToString();
                    if (p.BadEffect == BadEffect.BombTimerChanged)
                        badItemTimer += " (" + p.BombTimer.TotalSeconds.ToString() + ")";

                    for (int i = 0; i < lenght; i++)
                    {
                        FinalBomber.Instance.SpriteBatch.Draw(BadItemTimerBar,
                            new Vector2(HudOrigin.X + HudMarginLeft + 20 + 1 * i,
                                HudOrigin.Y + HudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 90), Color.White);
                    }
                    FinalBomber.Instance.SpriteBatch.DrawString(SmallFont, badItemTimer,
                        new Vector2(HudOrigin.X + HudMarginLeft + 20 + 1 * (lenght / 2) - SmallFont.MeasureString(badItemTimer).X / 2,
                            HudOrigin.Y + HudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 90), Color.White);
                }
                else
                {
                    FinalBomber.Instance.SpriteBatch.Draw(ItemInfoIcon, new Vector2(HudOrigin.X + HudMarginLeft,
                        HudOrigin.Y + HudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 90),
                                new Rectangle(84, 0, 14, 14), Color.White);
                }
            }

            var pos =
                new Vector2(
                    HudOrigin.X + HudMarginLeft +
                    (ControlManager.SpriteFont.MeasureString(_gameManager.GameTimer.ToString("mm") + ":" +
                                                             _gameManager.GameTimer.ToString("ss")).X) + 25,
                    HudOrigin.Y + HudTopSpace + Config.HUDPlayerInfoSpace * _gameManager.Players.Count + 22);
            FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, _gameManager.GameTimer.ToString("mm") + ":" + _gameManager.GameTimer.ToString("ss"),
                pos, Color.Black);

            #endregion

#if DEBUG
            #region Debug

            if (InputHandler.KeyDown(Keys.O))
            {
                for (int x = 0; x < _gameManager.CurrentMap.Size.X; x++)
                {
                    for (int y = 0; y < _gameManager.CurrentMap.Size.Y; y++)
                    {
                        string itemMapType = "";
                        Color colorMapItem = Color.White;
                        if (_gameManager.CurrentMap.Board[x, y] != null)
                        {
                            switch (_gameManager.CurrentMap.Board[x, y].GetType().Name)
                            {
                                case "Wall":
                                    itemMapType = "W";
                                    colorMapItem = Color.Pink;
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
            else if (InputHandler.KeyDown(Keys.C))
            {
                for (int x = 0; x < _gameManager.CurrentMap.Size.X; x++)
                {
                    for (int y = 0; y < _gameManager.CurrentMap.Size.Y; y++)
                    {
                        if (!_gameManager.CurrentMap.CollisionLayer[x, y])
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
            else if (InputHandler.KeyDown(Keys.H))
            {
                for (int x = 0; x < _gameManager.CurrentMap.Size.X; x++)
                {
                    for (int y = 0; y < _gameManager.CurrentMap.Size.Y; y++)
                    {
                        FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, _gameManager.HazardMap[x, y].ToString(),
                            new Vector2(x * 30, 80 + 20 * y), Color.Black);
                    }
                }
            }
            #endregion
#endif

            FinalBomber.Instance.SpriteBatch.End();
        }

        #endregion

        private void ResizeHud(object sender, EventArgs e)
        {
            ScoresWindowBox.Size = new Point(GraphicsDevice.Viewport.Width - (HudOrigin.X),
                HudTopSpace + _gameManager.Players.Count * Config.HUDPlayerInfoSpace + 15);

            TimerWindowBox.Position = new Vector2(HudOrigin.X, ScoresWindowBox.Size.Y);
        }

        private void GameServer_End(bool won)
        {
            StateManager.ChangeState(FinalBomber.Instance.LobbyMenuScreen);
            /*
            endTmr.Start();
            if (!Spectator)
            {
                shouldEnd = true;
                haveWon = won;
            }
            */
        }
    }

}
