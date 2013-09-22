using FBLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Final_Bomber.Screens;
using Final_Bomber.Controls;
using Final_Bomber.Utils;

namespace Final_Bomber
{
    public class FinalBomber : Microsoft.Xna.Framework.Game
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

        public SinglePlayerGameModeMenuScreen SinglePlayerGameModeMenuScreen;        

        // Local
        public BattleMenuScreen BattleMenuScreen;
        public SuddenDeathMenuScreen SuddenDeathMenuScreen;
        public ItemMenuScreen ItemMenuScreen;

        // Network
        public LobbyMenuScreen LobbyMenuScreen;
        public OnlineGameMenuScreen OnlineGameMenuScreen;
        public MultiplayerGameModeMenuScreen MultiplayerGameModeMenuScreen;
        public UserMenuScreen UserMenuScreen;
        public UserLoginMenuScreen UserLoginMenuScreen;
        public UserRegistrationMenuScreen UserRegistrationMenuScreen;
        public NetworkMenuScreen NetworkMenuScreen;
        public CreateServerMenuScreen CreateServerMenuScreen;
        public JoinServerMenuScreen JoinServerMenuScreen;

        public NetworkTestScreen NetworkTestScreen;

        // ~~ Game ~~ //
        public GamePlayScreen GamePlayScreen;

        #endregion

        public Rectangle ScreenRectangle;

        public FinalBomber()
        {
            Instance = this;
            graphics = new GraphicsDeviceManager(this)
                {
                    PreferredBackBufferWidth = Config.Resolutions[Config.IndexResolution, 0],
                    PreferredBackBufferHeight = Config.Resolutions[Config.IndexResolution, 1]
                };

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
        }

        protected override void Update(GameTime gameTime)
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
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}
