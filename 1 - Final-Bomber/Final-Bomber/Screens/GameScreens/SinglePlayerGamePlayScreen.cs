﻿using System;
using System.Linq;
using FBLibrary;
using FBLibrary.Core;
using FBClient.Controls;
using FBClient.Core;
using FBClient.Core.Players;
using FBClient.GUI;
using FBClient.WorldEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FBClient.Screens.GameScreens
{
    public class SinglePlayerGamePlayScreen : BaseGamePlayScreen
    {
        #region Field region

        // Player
        private HumanPlayer _player;

        #endregion

        #region Constructor region
        public SinglePlayerGamePlayScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            GameManager = new LocalGameManager();
        }
        #endregion

        #region XNA Method region

        public override void Initialize()
        {
            _player = new HumanPlayer(0) { Name = PlayerInfo.Username };

            // Map
            GameManager.LoadMap(MapLoader.MapFileDictionary.Keys.First());
            GameManager.GenerateRandomWalls();

            base.Initialize();

            HudOrigin = new Point(GraphicsDevice.Viewport.Width - 234, 0);

            GameManager.Initialize();
            _player.SetGameManager(GameManager);

            _player.ChangePosition(GameManager.CurrentMap.PlayerSpawnPoints[_player.Id]);

            Camera = new Camera2D(GraphicsDevice.Viewport, GameManager.CurrentMap.Size, 1f);

            GameManager.AddPlayer(_player);

            HudOrigin = new Point(GraphicsDevice.Viewport.Width - 234, 0);
            HudTopSpace = 15;
            HudMarginLeft = 15;

            ScoresWindowBox = new WindowBox(WindowSkin, new Vector2(HudOrigin.X, HudOrigin.Y),
                                             new Point(GraphicsDevice.Viewport.Width - (HudOrigin.X),
                                             HudTopSpace + GameManager.Players.Count * Config.HUDPlayerInfoSpace + 15));

            TimerWindowBox = new WindowBox(WindowSkin, new Vector2(HudOrigin.X, ScoresWindowBox.Size.Y),
                new Point(GraphicsDevice.Viewport.Width - HudOrigin.X, 40));
        }

        protected override void LoadContent()
        {
            _player.LoadContent();

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            GameManager.Update(gameTime);

            _player.Update(gameTime, GameManager.CurrentMap, GameManager.HazardMap);

            Camera.Update(gameTime, _player.Position);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            FinalBomber.Instance.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Camera.GetTransformation());

            GameManager.Draw(gameTime, Camera);

            FinalBomber.Instance.SpriteBatch.End();

            FinalBomber.Instance.SpriteBatch.Begin();
            const string str = "Single Player Tests";
            FinalBomber.Instance.SpriteBatch.DrawString(BigFont, str,
                    new Vector2(
                        Config.Resolutions[Config.IndexResolution, 0] / 2f -
                        this.BigFont.MeasureString(str).X / 2 - 75,
                        0),
            Color.Black);

            string cameraPosition = "Camera position: " + Camera.Position;
            FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, cameraPosition, new Vector2(1, 41), Color.Black);
            FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, cameraPosition, new Vector2(0, 40), Color.White);

            string playerSpeed = "Player's speed: " + _player.Speed;
            FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, playerSpeed, new Vector2(1, 61), Color.Black);
            FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, playerSpeed, new Vector2(0, 60), Color.White);

            #region HUD
            // Window boxes
            ScoresWindowBox.Draw(FinalBomber.Instance.SpriteBatch);
            TimerWindowBox.Draw(FinalBomber.Instance.SpriteBatch);

            // Draw player HUD
            foreach (var p in GameManager.Players)
            {
                // HUD => Item Info
                FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, p.Name + ": "
                     + p.Stats.Score + " pt(s)",
                    new Vector2(HudOrigin.X + HudMarginLeft, HudOrigin.Y + HudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace), Color.Black);

#if DEBUG
                FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, "Player " + p.Id + ": " + (p.CellPosition).ToString(),
                    new Vector2(HudOrigin.X + HudMarginLeft, HudOrigin.Y + HudTopSpace + Config.HUDPlayerInfoSpace * GameManager.Players.Count + 60 + 20 * (p.Id)), Color.Black);
#endif

                // To space the red icons and the "normal color" icons
                int iconLag = 0;

                // Player's power
                int counterRedFlames = 0;
                if (p.BombPower >= 10)
                {
                    iconLag = 10;
                    counterRedFlames = p.BombPower / 10;
                    for (int i = 0; i < counterRedFlames; i++)
                        FinalBomber.Instance.SpriteBatch.Draw(ItemInfoIcon,
                            new Vector2(HudOrigin.X + HudMarginLeft + 7 * i,
                            HudOrigin.Y + HudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 30),
                            new Rectangle(14, 0, 14, 14), Color.White);
                }
                else
                    iconLag = 0;
                int counterYellowFlames = p.BombPower % 10;
                for (int i = 0; i < counterYellowFlames; i++)
                    FinalBomber.Instance.SpriteBatch.Draw(ItemInfoIcon,
                        new Vector2(HudOrigin.X + HudMarginLeft + 14 * counterRedFlames + 7 * i + iconLag,
                            HudOrigin.Y + HudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 30),
                        new Rectangle(0, 0, 14, 14), Color.White);

                // Player's bomb number
                int counterRedBombs = 0;
                if (p.CurrentBombAmount >= 10)
                {
                    iconLag = 10;
                    counterRedBombs = p.CurrentBombAmount / 10;
                    for (int i = 0; i < counterRedBombs; i++)
                    {
                        FinalBomber.Instance.SpriteBatch.Draw(ItemInfoIcon, new Vector2(HudOrigin.X + HudMarginLeft + 7 * i,
                            HudOrigin.Y + HudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 50),
                            new Rectangle(42, 0, 14, 14), Color.White);
                    }
                }
                else
                    iconLag = 0;
                int counterBlackBombs = p.CurrentBombAmount % 10;
                int finalCounter = 0;
                for (int i = 0; i < counterBlackBombs; i++)
                {
                    FinalBomber.Instance.SpriteBatch.Draw(ItemInfoIcon,
                        new Vector2(HudOrigin.X + 7 * counterRedBombs + HudMarginLeft + 7 * i + iconLag, HudOrigin.Y +
                            HudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 50),
                        new Rectangle(28, 0, 14, 14), Color.White);
                    finalCounter = i;
                }

                if (p.HasBadEffect && p.BadEffect == BadEffect.NoBomb)
                {
                    FinalBomber.Instance.SpriteBatch.Draw(Cross, new Rectangle(HudOrigin.X + HudMarginLeft,
                        HudOrigin.Y + HudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 50,
                        7 * counterRedBombs + 7 * finalCounter + iconLag + 15, 15), Color.White);
                }

                // Player's speed
                int counterRedShoes = 0;
                var counterIncrementeur = (int)((p.Speed - GameConfiguration.BasePlayerSpeed) / (GameConfiguration.BasePlayerSpeed * (GameConfiguration.PlayerSpeedIncrementeurPercentage / 100))) + 1;
                if (counterIncrementeur >= 10)
                {
                    iconLag = 10;
                    counterRedShoes = (int)(counterIncrementeur) / 10;
                    for (int i = 0; i < counterRedShoes; i++)
                        FinalBomber.Instance.SpriteBatch.Draw(ItemInfoIcon, new Vector2(HudOrigin.X + HudMarginLeft + 7 * i,
                            HudOrigin.Y + HudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 70),
                            new Rectangle(70, 0, 14, 14), Color.White);
                }
                else
                    iconLag = 0;
                int counterBlueShoes = (int)(counterIncrementeur) % 10;

                for (int i = 0; i < counterBlueShoes; i++)
                    FinalBomber.Instance.SpriteBatch.Draw(ItemInfoIcon,
                        new Vector2(HudOrigin.X + HudMarginLeft + counterRedShoes * 7 + 7 * i + iconLag,
                            HudOrigin.Y + HudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 70),
                        new Rectangle(56, 0, 14, 14), Color.White);

                // Player's bad item GameTimer
                if (p.HasBadEffect)
                {
                    FinalBomber.Instance.SpriteBatch.Draw(ItemInfoIcon, new Vector2(HudOrigin.X + HudMarginLeft,
                        HudOrigin.Y + HudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 90),
                                new Rectangle(98, 0, 14, 14), Color.White);

                    int lenght = 200 - (int)((200 / p.BadEffectTimerLenght.TotalSeconds) * p.BadEffectTimer.TotalSeconds);
                    string badItemTimer = ((int)Math.Round(p.BadEffectTimerLenght.TotalSeconds - p.BadEffectTimer.TotalSeconds) + 1).ToString();
                    if (p.BadEffect == BadEffect.BombTimerChanged)
                        badItemTimer += " (" + p.BombTimer.TotalSeconds.ToString() + ")";

                    for (int i = 0; i < lenght; i++)
                    {
                        FinalBomber.Instance.SpriteBatch.Draw(BadItemTimerBar,
                            new Vector2(HudOrigin.X + HudMarginLeft + 20 + 1 * i,
                                HudOrigin.Y + HudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 90), Color.White);
                    }
                    FinalBomber.Instance.SpriteBatch.DrawString(SmallFont, badItemTimer,
                        new Vector2(HudOrigin.X + HudMarginLeft + 20 + 1 * (lenght / 2) - SmallFont.MeasureString(badItemTimer).X / 2,
                            HudOrigin.Y + HudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 90), Color.White);
                }
                else
                {
                    FinalBomber.Instance.SpriteBatch.Draw(ItemInfoIcon, new Vector2(HudOrigin.X + HudMarginLeft,
                        HudOrigin.Y + HudTopSpace + (p.Id) * Config.HUDPlayerInfoSpace + 90),
                                new Rectangle(84, 0, 14, 14), Color.White);
                }
            }

            var pos =
                new Vector2(
                    HudOrigin.X + HudMarginLeft +
                    (ControlManager.SpriteFont.MeasureString(GameManager.GameTimer.ToString("mm") + ":" +
                                                             GameManager.GameTimer.ToString("ss")).X) + 25,
                    HudOrigin.Y + HudTopSpace + Config.HUDPlayerInfoSpace * GameManager.Players.Count + 22);
            FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, GameManager.GameTimer.ToString("mm") + ":" + GameManager.GameTimer.ToString("ss"),
                pos, Color.Black);

            #endregion

#if DEBUG
            #region Debug

            if (InputHandler.KeyDown(Keys.O))
            {
                for (int x = 0; x < GameManager.CurrentMap.Size.X; x++)
                {
                    for (int y = 0; y < GameManager.CurrentMap.Size.Y; y++)
                    {
                        string itemMapType = "";
                        Color colorMapItem = Color.White;
                        if (GameManager.CurrentMap.Board[x, y] != null)
                        {
                            switch (GameManager.CurrentMap.Board[x, y].GetType().Name)
                            {
                                case "Wall":
                                    itemMapType = "W";
                                    colorMapItem = Color.Pink;
                                    break;
                                case "Bomb":
                                    itemMapType = "B";
                                    colorMapItem = Color.Red;
                                    break;
                                case "Player":
                                    itemMapType = "P";
                                    colorMapItem = Color.Green;
                                    break;
                                case "Item":
                                    itemMapType = "I";
                                    colorMapItem = Color.Yellow;
                                    break;
                                case "EdgeWall":
                                    itemMapType = "E";
                                    colorMapItem = Color.Black;
                                    break;
                                case "UnbreakableWall":
                                    itemMapType = "U";
                                    colorMapItem = Color.Black;
                                    break;
                                case "Teleporter":
                                    itemMapType = "T";
                                    colorMapItem = Color.Blue;
                                    break;
                                case "Arrow":
                                    itemMapType = "A";
                                    colorMapItem = Color.DarkViolet;
                                    break;
                            }
                        }
                        else
                            itemMapType = ".";

                        FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, itemMapType,
                            new Vector2(x * 20, 80 + 20 * y), colorMapItem);
                    }
                }
            }
            else if (InputHandler.KeyDown(Keys.C))
            {
                for (int x = 0; x < GameManager.CurrentMap.Size.X; x++)
                {
                    for (int y = 0; y < GameManager.CurrentMap.Size.Y; y++)
                    {
                        if (!GameManager.CurrentMap.CollisionLayer[x, y])
                        {
                            FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, "0",
                                new Vector2(x * 20, 80 + 20 * y), Color.ForestGreen);
                        }
                        else
                        {
                            FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, "1",
                            new Vector2(x * 20, 80 + 20 * y), Color.Purple);
                        }
                    }
                }
            }
            else if (InputHandler.KeyDown(Keys.H))
            {
                for (int x = 0; x < GameManager.CurrentMap.Size.X; x++)
                {
                    for (int y = 0; y < GameManager.CurrentMap.Size.Y; y++)
                    {
                        FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, GameManager.HazardMap[x, y].ToString(),
                            new Vector2(x * 30, 80 + 20 * y), Color.Black);
                    }
                }
            }
            #endregion
#endif

            FinalBomber.Instance.SpriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion

        private void ResizeHud(object sender, EventArgs e)
        {
            ScoresWindowBox.Size = new Point(GraphicsDevice.Viewport.Width - (HudOrigin.X),
                HudTopSpace + GameManager.Players.Count*Config.HUDPlayerInfoSpace + 15);

            TimerWindowBox.Position = new Vector2(HudOrigin.X, ScoresWindowBox.Size.Y);
        }
    }

}
