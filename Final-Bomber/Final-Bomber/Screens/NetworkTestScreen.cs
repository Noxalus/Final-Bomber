using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Final_Bomber.Controls;
using Microsoft.Xna.Framework.Input;
using Lidgren.Network;
using Final_Bomber.Network;
using System.Diagnostics;

namespace Final_Bomber.Screens
{
    public class NetworkTestScreen : BaseGameState
    {
        #region Field region
        private bool hasConnected;
        #endregion

        #region Constructor region
        public NetworkTestScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
        }
        #endregion

        #region XNA Method region

        public override void Initialize()
        {
            hasConnected = false;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            GameSettings.GameServer.EndClientConnection("Quit the game !");

            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            ControlManager.Update(gameTime, PlayerIndex.One);

            if (!hasConnected)
            {
                GameSettings.GameServer.StartClientConnection("127.0.0.1", "2643");

                Timer connectedTmr = new Timer();
                connectedTmr.Start();
                while (!hasConnected)
                {
                    GameSettings.GameServer.RunClientConnection();
                    if (GameSettings.GameServer.Connected)
                    {
                        hasConnected = true;
                    }
                    else
                    {
                        if (connectedTmr.Each(5000))
                        {
                            Debug.Print("Couldn't connect to the Game Server, please refresh the game list");
                            GameRef.Exit();
                        }
                    }
                }
            }

            /*
            var config = new NetPeerConfiguration("Final-Bomber");

            NetClient client;
            client = new NetClient(config);

            client.Connect("127.0.0.1", 2643);

            //client.Connect(Ip, int.Parse(2643));

            //msgOut = client.CreateMessage();
            /*
            NetOutgoingMessage sendMsg = server.CreateMessage();
            sendMsg.Write("Hello");
            sendMsg.Write(42);

            server.SendMessage(sendMsg, recipient, NetDeliveryMethod.ReliableOrdered);
            */

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GameRef.SpriteBatch.Begin();

            base.Draw(gameTime);

            ControlManager.Draw(GameRef.SpriteBatch);

            string str = "Networking Tests";
            GameRef.SpriteBatch.DrawString(this.BigFont, str,
                    new Vector2(
                        Config.Resolutions[Config.IndexResolution, 0] / 2 -
                        this.BigFont.MeasureString(str).X / 2,
                        20),
            Color.Black);

            GameRef.SpriteBatch.End();
        }

        #endregion
    }
}
