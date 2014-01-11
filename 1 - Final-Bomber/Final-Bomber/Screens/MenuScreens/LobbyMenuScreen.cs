using System;
using System.Collections.Generic;
using System.Diagnostics;
using FBLibrary;
using FBClient.Controls;
using FBClient.Network;
using FBClient.Screens.GameScreens;
using Microsoft.Xna.Framework;

namespace FBClient.Screens.MenuScreens
{
    public class LobbyMenuScreen : BaseGameState
    {
        #region Field region
        TimeSpan _timer;
        readonly Timer _connectedTimer;
        private Timer _timerWaitUntilStart;
        private bool _isConnected;
        private bool _isReady;
        #endregion

        #region Constructor region
        public LobbyMenuScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            _timer = new TimeSpan();
            _connectedTimer = new Timer();
            _isConnected = false;
            _isReady = false;
        }
        #endregion

        #region XNA Method region

        public override void Initialize()
        {
            GameServer.Instance.StartInfo += GameServer_StartInfo;
            GameServer.Instance.StartGame += GameServer_StartGame;

            GameServer.Instance.StartClientConnection(GameConfiguration.ServerIp, GameConfiguration.ServerPort);

            _timerWaitUntilStart = new Timer();
            _connectedTimer.Start();

            GameServer.Instance.SetGameManager(new NetworkGameManager());

            base.Initialize();
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
            ControlManager.Update(gameTime, PlayerIndex.One);

            if (!_isConnected)
            {
                GameServer.Instance.RunClientConnection();
                if (GameServer.Instance.Connected)
                {
                    _isConnected = true;
                    //_publicIp = GetPublicIP();
                }
                else if (_connectedTimer.Each(5000))
                {
                    Debug.Print("Couldn't connect to the Game Server, please refresh the game list");
                    //FinalBomber.Instance.Exit();
                }
            }
            else
            {
                if (GameServer.Instance.HasStarted)
                    GameServer.Instance.RunClientConnection();

                ProgramStepProccesing();
            }

            _timer += TimeSpan.FromTicks(GameConfiguration.DeltaTime);

            if (_timer.Seconds == 5)
            {
                _timer = TimeSpan.Zero;
            }

            base.Update(gameTime);
        }

        private void ProgramStepProccesing()
        {
            if (!GameServer.Instance.Connected)
            {
                DisplayStatusBeforeExiting("The Game Server has closed/disconnected");
            }
            if (GameServer.Instance.Connected)
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

                _timerWaitUntilStart = new Timer();
                GameServer.Instance.SendIsReady();
                _timerWaitUntilStart.Start();
            }

            // Wait 5 seconds before to say to the server that we are ready
            if (_timerWaitUntilStart.Each(5000))
            {
                GameServer.Instance.SendIsReady();
                _timerWaitUntilStart.Stop();
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
                    NetworkGamePlayScreen.NetworkManager.Me.Id = playerId;
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

                GameServer.Instance.GameManager.LoadMap(GameSettings.CurrentMapName);
                GameServer.Instance.GameManager.AddWalls(wallPositions);

                //mainGame.Start();
            }


            StateManager.ChangeState(FinalBomber.Instance.NetworkTestScreen);
            UnloadContent();
        }

        #endregion
    }
}
