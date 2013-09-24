using System;
using FBLibrary;
using FBLibrary.Core;
using Final_Bomber.Core;
using Final_Bomber.Core.Players;
using Microsoft.Xna.Framework;
using Final_Bomber.Controls;
using Final_Bomber.Network;
using System.Diagnostics;
using Final_Bomber.Entities;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Final_Bomber.Screens
{
    public class NetworkTestScreen : BaseGameState
    {
        #region Field region
        Process _serverProcess;

        // Game manager
        public static GameManager GameManager;

        // Network
        public static NetworkManager NetworkManager;

        // HUD
        Texture2D _itemInfoIcon;
        Texture2D _cross;
        SpriteFont _gameFont;
        SpriteFont _smallFont;
        Point _hudOrigin;
        int _hudTopSpace;
        int _hudMarginLeft;
        Texture2D _badItemTimerBar;

        // Window box
        Texture2D _windowSkin;
        WindowBox _scoresWindowBox;
        WindowBox _timerWindowBox;

        #endregion

        #region Constructor region
        public NetworkTestScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            GameManager = new GameManager();
            NetworkManager = new NetworkManager(GameManager);
        }
        #endregion

        #region XNA Method region

        public override void Initialize()
        {
            _hudOrigin = new Point(FinalBomber.Instance.GraphicsDevice.Viewport.Width - 234, 0);

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

            GameManager.Initialize();
            NetworkManager.Initiliaze();

            base.Initialize();

            GameManager.Reset();
            GameManager.Players.Add(NetworkManager.Me);

            _hudOrigin = new Point(GraphicsDevice.Viewport.Width - 234, 0);
            _hudTopSpace = 15;
            _hudMarginLeft = 15;

            _scoresWindowBox = new WindowBox(_windowSkin, new Vector2(_hudOrigin.X, _hudOrigin.Y),
                                             new Point(GraphicsDevice.Viewport.Width - (_hudOrigin.X),
                                             _hudTopSpace + GameManager.Players.Count * Config.HUDPlayerInfoSpace + 15));

            _timerWindowBox = new WindowBox(_windowSkin, new Vector2(_hudOrigin.X, _scoresWindowBox.Size.Y),
                new Point(GraphicsDevice.Viewport.Width - _hudOrigin.X, 40));
        }

        protected override void LoadContent()
        {
            // Pictures      
            _itemInfoIcon = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Pictures/ItemInfo");
            _cross = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Pictures/Cross");
            _badItemTimerBar = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Pictures/BadItemTimerCross");
            _windowSkin = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Windowskins/Windowskin1");

            // Fonts
            _gameFont = FinalBomber.Instance.Content.Load<SpriteFont>("Graphics/Fonts/GameFont");
            _smallFont = FinalBomber.Instance.Content.Load<SpriteFont>("Graphics/Fonts/SmallFont");

            GameManager.LoadContent();
            NetworkManager.LoadContent();

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            GameSettings.GameServer.EndClientConnection("Quit the game !");

            //_serverProcess.Kill();

            NetworkManager.Dispose();

            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            ControlManager.Update(gameTime, PlayerIndex.One);

            NetworkManager.Update();
            GameManager.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            FinalBomber.Instance.SpriteBatch.Begin();

            ControlManager.Draw(FinalBomber.Instance.SpriteBatch);

            GameManager.Draw(gameTime);

            const string str = "Networking Tests";
            FinalBomber.Instance.SpriteBatch.DrawString(BigFont, str,
                    new Vector2(
                        Config.Resolutions[Config.IndexResolution, 0] / 2f -
                        this.BigFont.MeasureString(str).X / 2,
                        0),
            Color.Black);

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
            _scoresWindowBox.Draw(FinalBomber.Instance.SpriteBatch);
            _timerWindowBox.Draw(FinalBomber.Instance.SpriteBatch);

            // Draw player HUD
            foreach (var p in GameManager.Players)
            {
                // HUD => Item Info
                FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, Config.PlayersName[p.Id] + ": "
                     + p.Stats.PointPicked + " pt(s)",
                    new Vector2(_hudOrigin.X + _hudMarginLeft, _hudOrigin.Y + _hudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace), Color.Black);

#if DEBUG
                    FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, "Player " + p.Id + ": " + (p.CellPosition).ToString(),
                        new Vector2(_hudOrigin.X + _hudMarginLeft, _hudOrigin.Y + _hudTopSpace + Config.HUDPlayerInfoSpace * GameManager.Players.Count + 60 + 20 * (p.Id)), Color.Black);
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
                var counterIncrementeur = (int)((p.Speed - GameConfiguration.BasePlayerSpeed) / (GameConfiguration.BasePlayerSpeed * (GameConfiguration.PlayerSpeedIncrementeurPercentage / 100))) + 1;
                if (counterIncrementeur >= 10)
                {
                    iconLag = 10;
                    counterRedShoes = (int)(counterIncrementeur) / 10;
                    for (int i = 0; i < counterRedShoes; i++)
                        FinalBomber.Instance.SpriteBatch.Draw(_itemInfoIcon, new Vector2(_hudOrigin.X + _hudMarginLeft + 7 * i,
                            _hudOrigin.Y + _hudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 70),
                            new Rectangle(70, 0, 14, 14), Color.White);
                }
                else
                    iconLag = 0;
                int counterBlueShoes = (int)(counterIncrementeur) % 10;

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

            var pos =
                new Vector2(
                    _hudOrigin.X + _hudMarginLeft +
                    (ControlManager.SpriteFont.MeasureString(GameManager.Timer.ToString("mm") + ":" +
                                                             GameManager.Timer.ToString("ss")).X) + 25,
                    _hudOrigin.Y + _hudTopSpace + Config.HUDPlayerInfoSpace * GameManager.Players.Count + 22);
            FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, GameManager.Timer.ToString("mm") + ":" + GameManager.Timer.ToString("ss"),
                pos, Color.Black);

            #endregion

#if DEBUG
            #region Debug

            if (InputHandler.KeyDown(Keys.O))
            {
                for (int x = 0; x < GameManager.CurrentMap.Size.X; x++)
                {
                    for (int y = 0; y < GameManager.CurrentMap.Size.Y; y++)
                    {
                        string itemMapType = "";
                        Color colorMapItem = Color.White;
                        if (GameManager.CurrentMap.Board[x, y] != null)
                        {
                            switch (GameManager.CurrentMap.Board[x, y].GetType().Name)
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
                for (int x = 0; x < GameManager.CurrentMap.Size.X; x++)
                {
                    for (int y = 0; y < GameManager.CurrentMap.Size.Y; y++)
                    {
                        if (!GameManager.CurrentMap.CollisionLayer[x, y])
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
                for (int x = 0; x < GameManager.CurrentMap.Size.X; x++)
                {
                    for (int y = 0; y < GameManager.CurrentMap.Size.Y; y++)
                    {
                        FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, GameManager.HazardMap[x, y].ToString(),
                            new Vector2(x * 30, 80 + 20 * y), Color.Black);
                    }
                }
            }
            #endregion
#endif

            FinalBomber.Instance.SpriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion
    }

}
