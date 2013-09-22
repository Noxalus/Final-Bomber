using System;
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
        Point _hudOrigin;

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
        }

        protected override void LoadContent()
        {
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

            int counter = 0;
            foreach (var player in GameManager.Players)
            {
                FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, "Player #" + player.Id + ": (" + player.IsChangingCell() + 
                    ")\nR: " + player.Position + 
                    "\nP: " + player.CellPosition, new Vector2(530, 60 + (100 * counter)), Color.Black);

                counter++;
            }

            FinalBomber.Instance.SpriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion
    }

}
