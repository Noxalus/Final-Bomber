using System;
using System.Collections.Generic;
using System.Linq;
using FBLibrary;
using FBClient.Controls;
using FBClient.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FBClient.Screens.MenuScreens
{
    public class LobbyMenuScreen : BaseGameState
    {
        #region Constructor region
        public LobbyMenuScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
        }
        #endregion

        #region XNA Method region

        public override void Initialize()
        {
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
            GameServer.Instance.StartGame -= GameServer_StartGame;

            base.UnloadContent();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            GameServer.Instance.GameManager.NetworkManager.Update();

            // Read new messages from server and check connection status
            GameServer.Instance.Update();

            if (GameServer.Instance.FailedToConnect)
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

            else if (!GameServer.Instance.Disconnected)
            {
                if (GameServer.Instance.Clients.Me != null)
                {
                    // Send that the player is ready/not ready to start the game
                    if (InputHandler.KeyPressed(Keys.R))
                    {
                        GameServer.Instance.GameManager.NetworkManager.IsReady = !GameServer.Instance.GameManager.NetworkManager.IsReady;
                        GameServer.Instance.Clients.Me.IsReady = GameServer.Instance.GameManager.NetworkManager.IsReady;
                        GameServer.Instance.SendIsReady(GameServer.Instance.GameManager.NetworkManager.IsReady);
                    }

                    if (!GameServer.Instance.GameManager.NetworkManager.IsReady)
                    {
                        if (GameServer.Instance.Clients.Me.IsHost)
                        {
                            // Select map
                            if (InputHandler.KeyPressed(Keys.Up))
                            {
                                var mapList = GameServer.Instance.Maps.Values.ToList();
                                var mapIndex =
                                    mapList.FindIndex(mapName => mapName == GameServer.Instance.SelectedMapMd5);

                                mapIndex = (mapIndex - 1);
                                if (mapIndex < 0)
                                    mapIndex = mapList.Count - 1;

                                GameServer.Instance.SendMapSelection(mapList[mapIndex]);
                            }
                            else if (InputHandler.KeyPressed(Keys.Down))
                            {
                                var mapList = GameServer.Instance.Maps.Values.ToList();
                                var mapIndex =
                                    mapList.FindIndex(mapName => mapName == GameServer.Instance.SelectedMapMd5);

                                mapIndex = (mapIndex + 1)%mapList.Count;

                                GameServer.Instance.SendMapSelection(mapList[mapIndex]);
                            }
                        }
                    }

                    if (GameServer.Instance.Clients.TrueForAll(client => client.IsReady) &&
                        GameServer.Instance.Clients.Count >= GameConfiguration.MinimumPlayerNumber)
                    {
                        if (GameServer.Instance.Clients.Me.IsHost && InputHandler.KeyPressed(Keys.Space))
                        {
                            // Start the game
                            GameServer.Instance.SendWantToStartGame();
                        }
                    }
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

                // Ping
                str += " | Ping: " + GameServer.Instance.Clients[i].Ping;

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
                    var missingPlayerNumber = GameConfiguration.MinimumPlayerNumber - GameServer.Instance.Clients.Count;
                    if (missingPlayerNumber > 0)
                    {
                        str = "Not enough players :'(\n(" + missingPlayerNumber + " more player(s) to play)";
                        strColor = Color.Red;
                    }
                    else
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
                    }

                    FinalBomber.Instance.SpriteBatch.DrawString(BigFont, str,
                        new Vector2(
                            Config.Resolutions[Config.IndexResolution, 0] / 2f -
                            BigFont.MeasureString(str).X / 2,
                            Config.Resolutions[Config.IndexResolution, 1] / 2f -
                            BigFont.MeasureString(str).Y / 2 + 80),
                        strColor);
                }
            }

            // Available maps list
            for (int i = 0; i < GameServer.Instance.Maps.Count; i++)
            {
                str = GameServer.Instance.Maps.Keys.ElementAt(i);

                strColor = MapLoader.MapFileDictionary.Values.Contains(GameServer.Instance.Maps.Values.ElementAt(i)) ? Color.Green : Color.Red;

                if (GameServer.Instance.Maps.Values.ElementAt(i) == GameServer.Instance.SelectedMapMd5)
                    strColor = Color.White;

                FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, str,
                        new Vector2(
                            Config.Resolutions[Config.IndexResolution, 0] / 2f -
                            ControlManager.SpriteFont.MeasureString(str).X / 2,
                            Config.Resolutions[Config.IndexResolution, 1] / 2f -
                            ControlManager.SpriteFont.MeasureString(str).Y / 2 + 140 + (20 * i)),
                        strColor);
            }

            FinalBomber.Instance.SpriteBatch.End();
        }

        #endregion

        #region Server events

        private void GameServer_StartGame(bool gameInProgress, List<Point> wallPositions)
        {
            // Load the selected map
            GameServer.Instance.GameManager.LoadMap(MapLoader.GetMapNameFromMd5(GameServer.Instance.SelectedMapMd5));

            // Add the wall from server
            GameServer.Instance.GameManager.AddWalls(wallPositions);

            // Go to gameplay screen and let's start the battle ! :)
            StateManager.ChangeState(FinalBomber.Instance.NetworkTestScreen);

            UnloadContent();
        }

        #endregion
    }
}
