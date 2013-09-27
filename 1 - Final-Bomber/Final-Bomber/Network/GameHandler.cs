using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Final_Bomber.Network
{
    /*
    public partial class GameHandler : Microsoft.Xna.Framework.Game
    {
        //MainGame mainGame;
        //LobbyScreen lobbyScreen;

        Stopwatch speedTmr;

        public void SetScreen(string sX, string sY, string fs)
        {
            screenX = sX;
            screenY = sY;
            fullscreen = fs;
        }
        string screenX, screenY, fullscreen;

        public GameHandler()
        {
            //E2D_Engine.Initialize(this, "Content", Color.Black);
            //E2D_Engine.AudioEngineName = "hudsound";
        }

        protected override void Initialize()
        {
            speedTmr = new Stopwatch();
            speedTmr.Start();

            tmr_NotConnected = new Timer();
            tmr_NotConnected.Start();

            GameSettings.Maps.GetMaps();

            bool fs = (fullscreen == "1");
            //E2D_Engine.Begin(true, int.Parse(screenX), int.Parse(screenY), fs);

            //E2D_Engine.Special.Debug_Font = E2D_Engine.GetContentManager.Load<SpriteFont>("Font");
            
            //mainGame = new MainGame();
            //lobbyScreen = new LobbyScreen();
            
            GameSettings.GameServer.StartInfo += new GameServer.StartInfoEventHandler(GameServer_StartInfo);
            GameSettings.GameServer.StartGame += new GameServer.StartGameEventHandler(GameServer_StartGame);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            //GameSettings.font_playerName = Content.Load<SpriteFont>("PlayerName");
        }

        protected override void UnloadContent()
        {
            if (GameSettings.GameServer.Connected)
                GameSettings.GameServer.EndClientConnection("byebye");
        }

        protected override void Update(GameTime gameTime)
        {
            GameSettings.speed = speedTmr.ElapsedMilliseconds;
            speedTmr.Reset();
            speedTmr.Start();
            if (GameSettings.GameServer.HasStarted)
                GameSettings.GameServer.RunClientConnection();

            ProgramStepProccesing();

            E2D_Engine.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            E2D_Engine.Draw(gameTime);
        }

        private void GameHandler_Exiting(object sender, EventArgs arg) //När Spelet avslutas
        {

        }

    }
    */
}
