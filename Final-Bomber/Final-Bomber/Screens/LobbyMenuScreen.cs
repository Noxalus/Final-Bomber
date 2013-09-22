using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using FBLibrary.Core;
using Final_Bomber.Core.Entities;
using Microsoft.Xna.Framework;
using Final_Bomber.Controls;
using Microsoft.Xna.Framework.Input;
using Lidgren.Network;
using Final_Bomber.Network;

namespace Final_Bomber.Screens
{
    public class LobbyMenuScreen : BaseGameState
    {
        #region Field region
        TimeSpan timer;
        Timer tmr;
        Timer connectedTmr;
        private Timer _tmrWaitUntilStart;
        public bool IsConnected;
        private bool _isReady;
        #endregion

        #region Constructor region
        public LobbyMenuScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            timer = new TimeSpan();
            tmr = new Timer();
            connectedTmr = new Timer();
            IsConnected = false;
            _isReady = false;
        }
        #endregion

        #region XNA Method region

        public override void Initialize()
        {
            GameSettings.GameServer.StartInfo += new GameServer.StartInfoEventHandler(GameServer_StartInfo);
            GameSettings.GameServer.StartGame += new GameServer.StartGameEventHandler(GameServer_StartGame);

            GameSettings.GameServer.StartClientConnection(GameConfiguration.ServerIp, GameConfiguration.ServerPort);

            tmr.Start();
            connectedTmr.Start();

            base.Initialize();
        }

        protected override void UnloadContent()
        {
            GameSettings.GameServer.StartInfo -= new GameServer.StartInfoEventHandler(GameServer_StartInfo);
            GameSettings.GameServer.StartGame -= new GameServer.StartGameEventHandler(GameServer_StartGame);

            base.UnloadContent();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            ControlManager.Update(gameTime, PlayerIndex.One);

            if (!IsConnected)
            {
                GameSettings.GameServer.RunClientConnection();
                if (GameSettings.GameServer.Connected)
                {
                    IsConnected = true;
                    //_publicIp = GetPublicIP();
                }
                else if (connectedTmr.Each(5000))
                {
                    Debug.Print("Couldn't connect to the Game Server, please refresh the game list");
                    FinalBomber.Instance.Exit();
                }
            }
            else
            {
                if (GameSettings.GameServer.HasStarted)
                    GameSettings.GameServer.RunClientConnection();

                ProgramStepProccesing();
            }


            if (tmr.Each(1000))
            {
                timer = timer.Add(new TimeSpan(0, 0, 1));
                if (timer.Seconds == 5)
                {
                    tmr.Stop();
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

            str = "Waiting for players...";
            FinalBomber.Instance.SpriteBatch.DrawString(BigFont, str,
                new Vector2(
                    Config.Resolutions[Config.IndexResolution, 0] / 2f -
                    BigFont.MeasureString(str).X / 2,
                    Config.Resolutions[Config.IndexResolution, 1] / 2f -
                    BigFont.MeasureString(str).Y / 2),
                Color.Black);

            if (timer.Seconds != 5)
            {
                str = "Quit Timer: " + (5 - timer.Seconds).ToString();
                FinalBomber.Instance.SpriteBatch.DrawString(BigFont, str,
                    new Vector2(
                        Config.Resolutions[Config.IndexResolution, 0] / 2f -
                        BigFont.MeasureString(str).X / 2,
                        Config.Resolutions[Config.IndexResolution, 1] / 2f -
                        BigFont.MeasureString(str).Y / 2 + 60),
                    Color.Black);
            }

            FinalBomber.Instance.SpriteBatch.End();
        }

        #endregion

        private void ProgramStepProccesing()
        {
            if (!GameSettings.GameServer.Connected)
            {
                DisplayStatusBeforeExiting("The Game Server has closed/disconnected");
            }
            if (GameSettings.GameServer.Connected)
            {
                ConnectedGameProcessing();
            }
        }

        private void DisplayStatusBeforeExiting(string status)
        {
            throw new Exception("Exit ! (not connected to the server !)");
        }

        private void ConnectedGameProcessing()
        {
            if (_isReady)
            {
                _isReady = false;

                _tmrWaitUntilStart = new Timer();
                _tmrWaitUntilStart.Start();
            }

            // Wait 5 seconds before to say to the server that we are ready
            if (_tmrWaitUntilStart.Each(5000))
            {
                GameSettings.GameServer.SendIsReady();
                _tmrWaitUntilStart.Stop();
            }
        }

        #region Server events

        private void GameServer_StartInfo()
        {
            _isReady = true;
        }

        private void GameServer_StartGame(bool gameInProgress, int playerId, float moveSpeed, int suddenDeathTime, List<Point> wallPositions)
        {
            if (true /*!mainGame.IsStarted*/)
            {
                /*
                if (lobbyScreen.IsLoaded)
                {
                    lobbyScreen.End();
                    lobbyScreen.Unload();
                }
                */
                if (!gameInProgress)
                {
                    NetworkTestScreen.NetworkManager.Me.Id = playerId;
                    /*
                    me.MoveSpeed = moveSpeed;
                    suddenDeathTime = suddenDeathTime;
                    */
                }
                else
                {
                    /*
                    mainGame.me.Kill();
                    mainGame.Spectator = true;
                    */
                }

                NetworkTestScreen.GameManager.ParseMap(GameSettings.CurrentMapName);
                NetworkTestScreen.GameManager.AddWalls(wallPositions);

                //mainGame.Start();
            }

            StateManager.ChangeState(FinalBomber.Instance.NetworkTestScreen);
        }

        #endregion
    }
}
