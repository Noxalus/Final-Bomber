using FBLibrary;
using Final_Bomber.Screens.GameScreens;
using Final_Bomber.Screens.MenuScreens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Final_Bomber.Screens;
using Final_Bomber.Controls;
using Final_Bomber.Utils;

namespace Final_Bomber
{
    public class FinalBomber : Game
    {
        // Static instance
        public static FinalBomber Instance;

        // Password
        string _password;
        SoundEffect _debugModeEnabled;

        // Net
        //public MainServer Server;
        public bool HasLoggedIn = false;

        #region XNA Field Region

        public readonly GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch;

        #endregion

        #region Game State Region

        // ~~ Menu ~~ //

        public readonly TitleScreen TitleScreen;
        public readonly OptionMenuScreen OptionMenuScreen;
        public readonly KeysMenuScreen KeysMenuScreen;
        public readonly CreditMenuScreen CreditMenuScreen;

        public readonly SinglePlayerGameModeMenuScreen SinglePlayerGameModeMenuScreen;

        public readonly BattleMenuScreen BattleMenuScreen;
        public readonly SuddenDeathMenuScreen SuddenDeathMenuScreen;
        public readonly ItemMenuScreen ItemMenuScreen;

        // Local
        public readonly SinglePlayerGamePlayScreen SinglePlayerGamePlayScreen;

        // Network
        public readonly LobbyMenuScreen LobbyMenuScreen;
        public readonly OnlineGameMenuScreen OnlineGameMenuScreen;
        public readonly MultiplayerGameModeMenuScreen MultiplayerGameModeMenuScreen;
        public readonly UserMenuScreen UserMenuScreen;
        public readonly UserLoginMenuScreen UserLoginMenuScreen;
        public readonly UserRegistrationMenuScreen UserRegistrationMenuScreen;
        public readonly NetworkMenuScreen NetworkMenuScreen;
        public readonly CreateServerMenuScreen CreateServerMenuScreen;
        public readonly JoinServerMenuScreen JoinServerMenuScreen;

        public readonly NetworkTestScreen NetworkTestScreen;

        // ~~ Game ~~ //
        public readonly GamePlayScreen GamePlayScreen;

        #endregion

        public Rectangle ScreenRectangle;

        public FinalBomber()
        {
            Instance = this;
            Graphics = new GraphicsDeviceManager(this)
                {
                    PreferredBackBufferWidth = Config.Resolutions[Config.IndexResolution, 0],
                    PreferredBackBufferHeight = Config.Resolutions[Config.IndexResolution, 1]
                };

            ScreenRectangle = new Rectangle(0, 0, Config.Resolutions[Config.IndexResolution, 0], Config.Resolutions[Config.IndexResolution, 1]);

            Graphics.IsFullScreen = Config.FullScreen;
            Graphics.ApplyChanges();

            Content.RootDirectory = "Content";

            Components.Add(new InputHandler(this));

            var stateManager = new GameStateManager(this);
            Components.Add(stateManager);

            TitleScreen = new TitleScreen(this, stateManager);
            BattleMenuScreen = new BattleMenuScreen(this, stateManager);
            SuddenDeathMenuScreen = new SuddenDeathMenuScreen(this, stateManager);
            ItemMenuScreen = new ItemMenuScreen(this, stateManager);
            GamePlayScreen = new GamePlayScreen(this, stateManager);
            SinglePlayerGamePlayScreen = new SinglePlayerGamePlayScreen(this, stateManager);

            OptionMenuScreen = new OptionMenuScreen(this, stateManager);
            KeysMenuScreen = new KeysMenuScreen(this, stateManager);
            CreditMenuScreen = new CreditMenuScreen(this, stateManager);

            SinglePlayerGameModeMenuScreen = new SinglePlayerGameModeMenuScreen(this, stateManager);
            LobbyMenuScreen = new LobbyMenuScreen(this, stateManager);
            OnlineGameMenuScreen = new OnlineGameMenuScreen(this, stateManager);
            MultiplayerGameModeMenuScreen = new MultiplayerGameModeMenuScreen(this, stateManager);
            UserMenuScreen = new UserMenuScreen(this, stateManager);
            UserLoginMenuScreen = new UserLoginMenuScreen(this, stateManager);
            UserRegistrationMenuScreen = new UserRegistrationMenuScreen(this, stateManager);
            NetworkMenuScreen = new NetworkMenuScreen(this, stateManager);
            CreateServerMenuScreen = new CreateServerMenuScreen(this, stateManager);
            JoinServerMenuScreen = new JoinServerMenuScreen(this, stateManager);

            NetworkTestScreen = new NetworkTestScreen(this, stateManager);

            stateManager.ChangeState(TitleScreen);

            // FPS
            Components.Add(new FrameRateCounter(this));

            _password = "";
        }

        protected override void Initialize()
        {
            // We get all map files to store name + md5 checksum in a dictionary 
            MapLoader.LoadMapFiles();

            // We load player info
            StaticClassSerializer.Load(typeof(PlayerInfo), "PlayerInfo.xml", false);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            _debugModeEnabled = Content.Load<SoundEffect>("Audio/Sounds/debug");
        }

        protected override void UnloadContent()
        {
            /*
            if (Server != null)
                Server.client.Disconnect("Bye");
            */

            StaticClassSerializer.Save(typeof(PlayerInfo), "PlayerInfo.xml", false);
        }

        protected override void Update(GameTime gameTime)
        {
            // Delta time
            GameConfiguration.DeltaTime = gameTime.ElapsedGameTime.Milliseconds;

            UpdatePasswordManagement();

            base.Update(gameTime);
        }

        private void UpdatePasswordManagement()
        {
            if (InputHandler.KeyPressed(Keys.F) && _password == "")
                _password += "F";
            else if (InputHandler.KeyPressed(Keys.I) && _password == "F")
                _password += "i";
            else if (InputHandler.KeyPressed(Keys.N) && _password == "Fi")
                _password += "n";
            else if (InputHandler.KeyPressed(Keys.A) && _password == "Fin")
                _password += "a";
            else if (InputHandler.KeyPressed(Keys.L) && _password == "Fina")
                _password += "l";
            else if (InputHandler.KeyPressed(Keys.D6) && _password == "Final")
                _password += "-";
            else if (InputHandler.KeyPressed(Keys.B) && _password == "Final-")
                _password += "B";
            else if (InputHandler.KeyPressed(Keys.O) && _password == "Final-B")
                _password += "o";
            else if (InputHandler.KeyPressed(Keys.M) && _password == "Final-Bo")
                _password += "m";
            else if (InputHandler.KeyPressed(Keys.B) && _password == "Final-Bom")
                _password += "b";
            else if (InputHandler.KeyPressed(Keys.E) && _password == "Final-Bomb")
                _password += "e";
            else if (InputHandler.KeyPressed(Keys.R) && _password == "Final-Bombe")
                _password += "r";
            else if (_password == "Final-Bomber")
            {
                _debugModeEnabled.Play();
                Config.Debug = !Config.Debug;
                _password = "";
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}
