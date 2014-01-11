using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FBClient.Network
{
    partial class GameHandler
    {
        /*
        Timer tmr_NotConnected;
        Timer tmr_WaitUntilStart;

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

        private void ConnectedGameProcessing()
        {
            if (isReady && !mainGame.IsLoaded)
            {
                isReady = false;
                if (!lobbyScreen.IsLoaded)
                {
                    lobbyScreen.Load();
                    lobbyScreen.Start();
                }
                mainGame = new MainGame();
                mainGame.Load(GameSettings.Maps.GetMapById(GameSettings.currentMap));
                tmr_WaitUntilStart = new Timer();
                tmr_WaitUntilStart.Start();
            }
            if (tmr_WaitUntilStart.Each(5000)) //Vänta 5 sekunder innan man skickar isready, för att man ska hinna hoppa ur matchen
            {
                GameServer.Instance.SendIsReady();
                tmr_WaitUntilStart.Stop();
            }
        }

        private bool isReady = false;

        private void GameServer_StartInfo() //När servern har skickat banan som ska köras
        {
            isReady = true;
        }

        private void GameServer_StartGame(bool gameInProgress, int playerId, float moveSpeed, int suddenDeathTime)
        {
            if (!mainGame.IsStarted)
            {
                if (lobbyScreen.IsLoaded)
                {
                    lobbyScreen.End();
                    lobbyScreen.Unload();
                }
                if (!gameInProgress)
                {
                    mainGame.me.PlayerID = playerId;
                    mainGame.me.MoveSpeed = moveSpeed;
                    mainGame.suddenDeathTime = suddenDeathTime;
                }
                else
                {
                    mainGame.me.Kill();
                    mainGame.Spectator = true;
                }
                mainGame.Start();
            }
        }
        */
        private void DisplayStatusBeforeExiting(string status)
        {
            //this.Exit();
            /*
            if (E2D_Engine.GetGraphicManager.IsFullScreen)
            {
                E2D_Engine.GetGraphicManager.IsFullScreen = false;
                E2D_Engine.GetGraphicManager.ApplyChanges();
            }
            MessageBox.Show(status);
            */
        }
    }
}
