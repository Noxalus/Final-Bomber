using System;
using System.Collections.Generic;
using System.Diagnostics;
using FBLibrary;
using Final_Bomber.Controls;
using Final_Bomber.Network;
using Final_Bomber.Screens.GameScreens;
using Microsoft.Xna.Framework;

namespace Final_Bomber.Screens.MenuScreens
{
    public class LobbyMenuScreen : BaseGameState
    {
        #region Field region
        TimeSpan _timer;
        readonly Timer _tmr;
        readonly Timer _connectedTmr;
        private Timer _tmrWaitUntilStart;
        private bool _isConnected;
        private bool _isReady;
        #endregion

        #region Constructor region
        public LobbyMenuScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            _timer = new TimeSpan();
            _tmr = new Timer();
            _connectedTmr = new Timer();
            _isConnected = false;
            _isReady = false;
        }
        #endregion

        #region XNA Method region

        public override void Initialize()
        {
            GameSettings.GameServer.StartInfo += GameServer_StartInfo;
            GameSettings.GameServer.StartGame += GameServer_StartGame;

            GameSettings.GameServer.StartClientConnection(GameConfiguration.ServerIp, GameConfiguration.ServerPort);

            _tmr.Start();
            _tmrWaitUntilStart = new Timer();
            _connectedTmr.Start();

            base.Initialize();
        }

        protected override void UnloadContent()
        {
            GameSettings.GameServer.StartInfo -= GameServer_StartInfo;
            GameSettings.GameServer.StartGame -= GameServer_StartGame;

            base.UnloadContent();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            ControlManager.Update(gameTime, PlayerIndex.One);

            if (!_isConnected)
            {
                GameSettings.GameServer.RunClientConnection();
                if (GameSettings.GameServer.Connected)
                {
                    _isConnected = true;
                    //_publicIp = GetPublicIP();
                }
                else if (_connectedTmr.Each(5000))
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


            if (_tmr.Each(1000))
            {
                _timer = _timer.Add(new TimeSpan(0, 0, 1));
                if (_timer.Seconds == 5)
                {
                    _tmr.Stop();
                }
            }

            base.Update(gameTime);
        }

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
                GameSettings.GameServer.SendIsReady();
                _tmrWaitUntilStart.Start();
            }

            // Wait 5 seconds before to say to the server that we are ready
            if (_tmrWaitUntilStart.Each(5000))
            {
                GameSettings.GameServer.SendIsReady();
                _tmrWaitUntilStart.Stop();
            }
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

            if (_timer.Seconds != 5)
            {
                str = "Quit Timer: " + (5 - _timer.Seconds).ToString();
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

        #region Server events

        private void GameServer_StartInfo()
        {
            _isReady = true;
        }

        private void GameServer_StartGame(bool gameInProgress, int playerId, float moveSpeed, int suddenDeathTime, List<Point> wallPositions)
        {
            if (true /*!mainGame.IsStarted*/)
            {
                if (!gameInProgress)
                {
                    NetworkTestScreen.NetworkManager.Me.Id = playerId;
                    //NetworkTestScreen.NetworkManager.MoveSpeed = moveSpeed;
                    GameConfiguration.SuddenDeathTimer = TimeSpan.FromMilliseconds(suddenDeathTime);
                }
                else
                {
                    /*
                    mainGame.me.Kill();
                    mainGame.Spectator = true;
                    */
                }

                NetworkTestScreen.GameManager.LoadMap(GameSettings.CurrentMapName);
                NetworkTestScreen.GameManager.AddWalls(wallPositions);

                //mainGame.Start();
            }


            StateManager.ChangeState(FinalBomber.Instance.NetworkTestScreen);
            UnloadContent();
        }

        #endregion
    }
}
