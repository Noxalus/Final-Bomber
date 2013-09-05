using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Final_Bomber.Controls;
using Microsoft.Xna.Framework.Input;
using Lidgren.Network;

namespace Final_Bomber.Screens
{
    public class LobbyMenuScreen : BaseGameState
    {
        #region Field region

        #endregion

        #region Constructor region
        public LobbyMenuScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
        }
        #endregion

        #region XNA Method region

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            ControlManager.Update(gameTime, PlayerIndex.One);

            if (InputHandler.KeyPressed(Keys.A))
            {
                var config = new NetPeerConfiguration("Final-Bomber");

                NetClient client;
                client = new NetClient(config);

                //client.Connect(Ip, int.Parse(2643));

                //msgOut = client.CreateMessage();
                /*
                NetOutgoingMessage sendMsg = server.CreateMessage();
                sendMsg.Write("Hello");
                sendMsg.Write(42);

                server.SendMessage(sendMsg, recipient, NetDeliveryMethod.ReliableOrdered);
                */
            }

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
