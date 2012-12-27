using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Final_Bomber.Screens;
using Final_Bomber.Controls;
using Final_Bomber.Net.MainServer;

namespace Final_Bomber
{
    public class FinalBomber : Microsoft.Xna.Framework.Game
    {
        // Password
        string password;
        SoundEffect debugModeEnabled;

        // Net
        public MainServer server;
        public bool hasLoggedIn = false;

        #region XNA Field Region

        public GraphicsDeviceManager graphics;
        public SpriteBatch SpriteBatch;

        #endregion

        #region Game State Region

        GameStateManager stateManager;
        
        // ~~ Menu ~~ //

        public TitleScreen TitleScreen;
        public OptionMenuScreen OptionMenuScreen;
        public KeysMenuScreen KeysMenuScreen;
        public CreditMenuScreen CreditMenuScreen;

        // Local
        public BattleMenuScreen BattleMenuScreen;
        public SuddenDeathMenuScreen SuddenDeathMenuScreen;
        public ItemMenuScreen ItemMenuScreen;

        // Network
        public GameModeMenuScreen GameModeMenuScreen;
        public UserMenuScreen UserMenuScreen;
        public UserLoginMenuScreen UserLoginMenuScreen;
        public UserRegistrationMenuScreen UserRegistrationMenuScreen;
        public NetworkMenuScreen NetworkMenuScreen;
        public CreateServerMenuScreen CreateServerMenuScreen;
        public JoinServerMenuScreen JoinServerMenuScreen;
        

        // ~~ Game ~~ //
        public GamePlayScreen GamePlayScreen;

        #endregion

        public Rectangle ScreenRectangle;

        public FinalBomber()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = Config.Resolutions[Config.IndexResolution, 0];
            graphics.PreferredBackBufferHeight = Config.Resolutions[Config.IndexResolution, 1];

            ScreenRectangle = new Rectangle(0, 0, Config.Resolutions[Config.IndexResolution, 0], Config.Resolutions[Config.IndexResolution, 1]);

            graphics.IsFullScreen = Config.FullScreen;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";

            Components.Add(new InputHandler(this));

            stateManager = new GameStateManager(this);
            Components.Add(stateManager);

            
            TitleScreen = new TitleScreen(this, stateManager);
            BattleMenuScreen = new BattleMenuScreen(this, stateManager);
            SuddenDeathMenuScreen = new SuddenDeathMenuScreen(this, stateManager);
            ItemMenuScreen = new ItemMenuScreen(this, stateManager);
            GamePlayScreen = new GamePlayScreen(this, stateManager);
            OptionMenuScreen = new OptionMenuScreen(this, stateManager);
            KeysMenuScreen = new KeysMenuScreen(this, stateManager);
            CreditMenuScreen = new CreditMenuScreen(this, stateManager);

            GameModeMenuScreen = new GameModeMenuScreen(this, stateManager);
            UserMenuScreen = new UserMenuScreen(this, stateManager);
            UserLoginMenuScreen = new UserLoginMenuScreen(this, stateManager);
            UserRegistrationMenuScreen = new UserRegistrationMenuScreen(this, stateManager);
            NetworkMenuScreen = new NetworkMenuScreen(this, stateManager);
            CreateServerMenuScreen = new CreateServerMenuScreen(this, stateManager);
            JoinServerMenuScreen = new JoinServerMenuScreen(this, stateManager);

            stateManager.ChangeState(TitleScreen);

            password = "";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            debugModeEnabled = Content.Load<SoundEffect>("Audio/Sounds/boom");
        }

        protected override void UnloadContent()
        {
            if (server != null)
                server.client.Disconnect("Bye");
        }

        protected override void Update(GameTime gameTime)
        {
            if (InputHandler.KeyPressed(Keys.F) && password == "")
                password += "F";
            else if (InputHandler.KeyPressed(Keys.I) && password == "F")
                password += "i";
            else if (InputHandler.KeyPressed(Keys.N) && password == "Fi")
                password += "n";
            else if (InputHandler.KeyPressed(Keys.A) && password == "Fin")
                password += "a";
            else if (InputHandler.KeyPressed(Keys.L) && password == "Fina")
                password += "l";
            else if (InputHandler.KeyPressed(Keys.D6) && password == "Final")
                password += "-";
            else if (InputHandler.KeyPressed(Keys.B) && password == "Final-")
                password += "B";
            else if (InputHandler.KeyPressed(Keys.O) && password == "Final-B")
                password += "o";
            else if (InputHandler.KeyPressed(Keys.M) && password == "Final-Bo")
                password += "m";
            else if (InputHandler.KeyPressed(Keys.B) && password == "Final-Bom")
                password += "b";
            else if (InputHandler.KeyPressed(Keys.E) && password == "Final-Bomb")
                password += "e";
            else if (InputHandler.KeyPressed(Keys.R) && password == "Final-Bombe")
                password += "r";
            else if (password == "Final-Bomber")
            {
                debugModeEnabled.Play();
                Config.Debug = !Config.Debug;
                password = "";
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}
