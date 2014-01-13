using System;
using System.Collections.Generic;
using System.Diagnostics;
using FBLibrary;
using FBClient.Controls;
using FBClient.Network;
using FBLibrary.Core.BaseEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FBClient.Screens.MenuScreens
{
    public class LobbyMenuScreen : BaseGameState
    {
        #region Field region

        private bool _isConnected;
        private bool _isReady;
        #endregion

        #region Constructor region
        public LobbyMenuScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            _isConnected = false;
            _isReady = false;
        }
        #endregion

        #region XNA Method region

        public override void Initialize()
        {
            GameServer.Instance.StartInfo += GameServer_StartInfo;
            GameServer.Instance.StartGame += GameServer_StartGame;

            FinalBomber.Instance.Exiting += Instance_Exiting;

            GameServer.Instance.SetGameManager(new NetworkGameManager());

            base.Initialize();

            // Start connexion with server
            GameServer.Instance.StartClientConnection(GameConfiguration.ServerIp, GameConfiguration.ServerPort);
        }

        void Instance_Exiting(object sender, EventArgs e)
        {
            // We have to disconnect from the server and stop all threads
            GameServer.Instance.EndClientConnection("Client left the game.");
        }

        protected override void UnloadContent()
        {
            GameServer.Instance.StartInfo -= GameServer_StartInfo;
            GameServer.Instance.StartGame -= GameServer_StartGame;

            base.UnloadContent();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            // Read new messages from server and check connection status
            GameServer.Instance.Update();

            // First connection
            if (!_isConnected)
            {
                if (GameServer.Instance.Connected)
                {
                    _isConnected = true;
                    // TODO: Find a good way to display client IP
                    //_publicIp = GetPublicIP();
                }
                else if (GameServer.Instance.FailedToConnect)
                {
                    // TODO: Display a disconnection message when connection time out is done
                    // => ask to reconnect or quit
                    // FinalBomber.Instance.Exit();

                    // Try to reconnect
                    if (InputHandler.KeyPressed(Keys.Space))
                    {
                        GameServer.Instance.FailedToConnect = false;
                        GameServer.Instance.TryToConnect(GameConfiguration.ServerIp, GameConfiguration.ServerPort);
                    }
                }
            }
            else
            {
                if (!GameServer.Instance.Disconnected)
                {
                    // Send that the player is ready/not ready to start the game
                    if (InputHandler.KeyPressed(Keys.R))
                    {
                        _isReady = !_isReady;
                        GameServer.Instance.Clients.Me.IsReady = _isReady;
                        GameServer.Instance.SendIsReady(_isReady);
                    }

                    if (GameServer.Instance.Clients.Me != null)
                    {
                        if (GameServer.Instance.Clients.TrueForAll(client => client.IsReady))
                        {
                            if (GameServer.Instance.Clients.Me.IsHost && InputHandler.KeyPressed(Keys.Space))
                            {
                                // Start the game
                                GameServer.Instance.SendStartGame();
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("Exit ! (not connected to the server !)");
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            FinalBomber.Instance.SpriteBatch.Begin();

            base.Draw(gameTime);

            ControlManager.Draw(FinalBomber.Instance.SpriteBatch);

            string str = "Lobby Screen !";
            FinalBomber.Instance.SpriteBatch.DrawString(BigFont, str,
                    new Vector2(
                        Config.Resolutions[Config.IndexResolution, 0] / 2f -
                        BigFont.MeasureString(str).X / 2,
                        20),
            Color.Black);

            var strColor = Color.Black;
            GameServer.Instance.Clients.Sort(new BasicClientSorter());
            for (var i = 0; i < GameServer.Instance.Clients.Count; i++)
            {
                strColor = Color.Black;
                if (GameServer.Instance.Clients[i].IsReady)
                {
                    strColor = Color.Green;
                }

                str = GameServer.Instance.Clients[i].Username;
                if (GameServer.Instance.Clients[i].Id == GameServer.Instance.Clients.Me.Id)
                    str += " (you)";

                if (GameServer.Instance.Clients[i].IsHost)
                    str = "[*] " + str;

                FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, str,
                    new Vector2(0, 60 + (20 * i)), strColor);
            }

            // Connecting message
            strColor = Color.Orange;
            if (GameServer.Instance.Connected)
            {
                str = "Connected ! :)";
                strColor = Color.Green;
            }
            else if (GameServer.Instance.FailedToConnect)
            {
                str = "Failed to connect :'( (Press space to try again)";
                strColor = Color.Red;
            }
            else
                str = "Connecting to server...";

            FinalBomber.Instance.SpriteBatch.DrawString(BigFont, str,
                new Vector2(
                    Config.Resolutions[Config.IndexResolution, 0] / 2f -
                    BigFont.MeasureString(str).X / 2,
                    Config.Resolutions[Config.IndexResolution, 1] / 2f -
                    BigFont.MeasureString(str).Y / 2),
                strColor);

            if (GameServer.Instance.Clients.Me != null)
            {
                // Start game message
                if (GameServer.Instance.Clients.TrueForAll(client => client.IsReady))
                {
                    if (GameServer.Instance.Clients.Me.IsHost)
                    {
                        str = "Press Space to launch the game !";
                        strColor = Color.Green;
                    }
                    else
                    {
                        str = "Please wait that the host starts the game...";
                        strColor = Color.Orange;
                    }

                    FinalBomber.Instance.SpriteBatch.DrawString(BigFont, str,
                        new Vector2(
                            Config.Resolutions[Config.IndexResolution, 0]/2f -
                            BigFont.MeasureString(str).X/2,
                            Config.Resolutions[Config.IndexResolution, 1]/2f -
                            BigFont.MeasureString(str).Y/2 + 60),
                        strColor);
                }
            }

            FinalBomber.Instance.SpriteBatch.End();
        }

        #endregion

        #region Server events

        private void GameServer_StartInfo()
        {
        }

        private void GameServer_StartGame(bool gameInProgress, int playerId, float moveSpeed, int suddenDeathTime, List<Point> wallPositions)
        {
            if (true /*!mainGame.IsStarted*/)
            {
                GameServer.Instance.GameManager.LoadMap(GameSettings.CurrentMapName);
                GameServer.Instance.GameManager.AddWalls(wallPositions);
            }

            StateManager.ChangeState(FinalBomber.Instance.NetworkTestScreen);
            UnloadContent();
        }

        #endregion
    }
}
