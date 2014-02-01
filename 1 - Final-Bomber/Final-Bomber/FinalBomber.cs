using System;
using System.Diagnostics;
using FBLibrary;
using FBClient.Screens.GameScreens;
using FBClient.Screens.MenuScreens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FBClient.Controls;
using FBClient.Utils;
using Microsoft.Xna.Framework.Media;

namespace FBClient
{
    public class FinalBomber : Game
    {
        // Static instance
        public static FinalBomber Instance;

        // Password
        string _password;
        SoundEffect _debugModeEnabled;

        // Timing
        private Stopwatch _timer;

        // Net
        //public MainServer Server;
        public bool HasLoggedIn = false;

        // Debug timer
        private TimeSpan _debugTimer = TimeSpan.Zero;

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

        public readonly NetworkGamePlayScreen NetworkTestScreen;

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

#if DEBUG
            Graphics.SynchronizeWithVerticalRetrace = false;
#else
            Graphics.SynchronizeWithVerticalRetrace = true;
#endif
            // Don't fix FPS to 60 => this wont work when vsync is ON
            IsFixedTimeStep = false;

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

            NetworkTestScreen = new NetworkGamePlayScreen(this, stateManager);

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

            // Volume
            MediaPlayer.Volume = Config.MusicVolume;
            SoundEffect.MasterVolume = Config.SoundVolume;

            _timer = new Stopwatch();
            _timer.Start();

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
            // Password to enable debug/cheat mode
            UpdatePasswordManagement();

            // Shortcuts
            var dt = (float)TimeSpan.FromTicks(GameConfiguration.DeltaTime).TotalSeconds;

            if (InputHandler.KeyDown(Keys.NumPad9))
            {
                Config.MusicVolume = MathHelper.Clamp(Config.MusicVolume + 0.5f * dt, 0, 1);
                MediaPlayer.Volume = Config.MusicVolume;
            }
            else if (InputHandler.KeyDown(Keys.NumPad7))
            {
                Config.MusicVolume = MathHelper.Clamp(Config.MusicVolume - 0.5f * dt, 0, 1);
                MediaPlayer.Volume = Config.MusicVolume;
            }

            if (InputHandler.KeyDown(Keys.NumPad6))
            {
                Config.SoundVolume = MathHelper.Clamp(Config.SoundVolume + 0.5f * dt, 0, 1);
                SoundEffect.MasterVolume = Config.SoundVolume;
            }
            else if (InputHandler.KeyDown(Keys.NumPad4))
            {
                Config.SoundVolume = MathHelper.Clamp(Config.SoundVolume - 0.5f * dt, 0, 1);
                SoundEffect.MasterVolume = Config.SoundVolume;
            }

            base.Update(gameTime);

            // Delta time
            //GameConfiguration.DeltaTime = gameTime.ElapsedGameTime.Ticks;
            GameConfiguration.DeltaTime = _timer.Elapsed.Ticks;

            /*
            // Debug time
            _debugTimer += TimeSpan.FromTicks(GameConfiguration.DeltaTime);
            Debug.Print("Timer: " + _debugTimer);
            */

            _timer.Restart();
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
