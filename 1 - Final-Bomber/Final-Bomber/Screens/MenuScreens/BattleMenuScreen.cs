using System;
using System.Globalization;
using FBLibrary;
using FBClient.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FBClient.Screens.MenuScreens
{
    public class BattleMenuScreen : BaseMenuScreen
    {
        #region Field Region
        bool _disabledArrows;
        #endregion

        #region Constructor Region

        public BattleMenuScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            MenuString = new string[] 
            { 
                "Nombre de joueur", 
                "Taille", 
                Config.MapSize.X.ToString(CultureInfo.InvariantCulture), 
                Config.MapSize.Y.ToString(CultureInfo.InvariantCulture), 
                "Téléporteurs",
                "Flèches",
                "Nombre de murs",
                "Nombre d'objets",
                "Mort Subite", 
                "Objets", 
                "Lancer la partie !" };

            _disabledArrows = false;
        }

        #endregion

        #region XNA Method Region

        public override void Initialize()
        {
            base.Initialize();

            MenuPosition.Y /= 2;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            ControlManager.Update(gameTime, PlayerIndex.One);

            if (InputHandler.KeyPressed(Keys.Escape))
                StateManager.PushState(FinalBomber.Instance.TitleScreen);

            switch (IndexMenu)
            {
                case 0:
                    if (InputHandler.KeyPressed(Keys.Left))
                    {
                        if (Config.PlayersNumber <= 1)
                            Config.PlayersNumber = 5;
                        else
                            Config.PlayersNumber--;
                    }
                    else if (InputHandler.KeyPressed(Keys.Right))
                    {
                        if (Config.PlayersNumber < 5)
                            Config.PlayersNumber++;
                        else
                            Config.PlayersNumber = 1;
                    }
                    break;
                case 1:
                    if (InputHandler.KeyPressed(Keys.Left))
                    {
                        if (Config.MapSize.X <= Config.MinimumMapSize.X || Config.MapSize.Y <= Config.MinimumMapSize.Y)
                        {
                            Config.MapSize = Config.MaximumMapSize[Config.IndexResolution];
                        }
                        else
                        {
                            Config.MapSize = new Point(Config.MapSize.X - 2, Config.MapSize.Y - 2);
                        }
                    }
                    else if (InputHandler.KeyPressed(Keys.Right))
                    {
                        if (Config.MapSize.X == Config.MaximumMapSize[Config.IndexResolution].X ||
                            Config.MapSize.Y == Config.MaximumMapSize[Config.IndexResolution].Y)
                        {
                            Config.MapSize = Config.MinimumMapSize;
                        }
                        else
                        {
                            Config.MapSize = new Point(Config.MapSize.X + 2, Config.MapSize.Y + 2);
                        }
                    }
                    MenuString[2] = Config.MapSize.X.ToString(CultureInfo.InvariantCulture);
                    MenuString[3] = Config.MapSize.Y.ToString(CultureInfo.InvariantCulture);

                    if (Config.MapSize.X < 17 || Config.MapSize.Y < 17)
                    {
                        _disabledArrows = true;
                        Config.ActiveArrows = false;
                    }
                    else
                        _disabledArrows = false;
                    break;
                case 2:
                    if (InputHandler.KeyPressed(Keys.Left))
                    {
                        if (Config.MapSize.X <= Config.MinimumMapSize.X)
                            Config.MapSize.X = Config.MaximumMapSize[Config.IndexResolution].X;
                        else
                            Config.MapSize.X -= 2;
                    }
                    else if (InputHandler.KeyPressed(Keys.Right))
                    {
                        if (Config.MapSize.X == Config.MaximumMapSize[Config.IndexResolution].X)
                            Config.MapSize.X = Config.MinimumMapSize.X;
                        else
                            Config.MapSize.X += 2;
                    }
                    MenuString[2] = Config.MapSize.X.ToString(CultureInfo.InvariantCulture);
                    if (Config.MapSize.X < 17)
                    {
                        _disabledArrows = true;
                        Config.ActiveArrows = false;
                    }
                    else if (Config.MapSize.Y >= 17)
                        _disabledArrows = false;   
                    break;
                case 3:
                    if (InputHandler.KeyPressed(Keys.Left))
                    {
                        if (Config.MapSize.Y <= Config.MinimumMapSize.Y)
                            Config.MapSize.Y = Config.MaximumMapSize[Config.IndexResolution].Y;
                        else
                            Config.MapSize.Y -= 2;
                    }
                    else if (InputHandler.KeyPressed(Keys.Right))
                    {
                        if (Config.MapSize.Y == Config.MaximumMapSize[Config.IndexResolution].Y)
                            Config.MapSize.Y = Config.MinimumMapSize.Y;
                        else
                            Config.MapSize.Y += 2;
                    }
                    MenuString[3] = Config.MapSize.Y.ToString();
                    if (Config.MapSize.Y < 17)
                    {
                        _disabledArrows = true;
                        Config.ActiveArrows = false;
                    }
                    else if(Config.MapSize.X >= 17)
                        _disabledArrows = false;                    
                    break;
                case 4:
                    if(InputHandler.KeyPressed(Keys.Right) || InputHandler.KeyPressed(Keys.Left) || InputHandler.KeyPressed(Keys.Enter))
                        Config.ActiveTeleporters = !Config.ActiveTeleporters;
                    break;
                case 5:
                    if(InputHandler.KeyPressed(Keys.Right) || InputHandler.KeyPressed(Keys.Left) || InputHandler.KeyPressed(Keys.Enter))
                        Config.ActiveArrows = !Config.ActiveArrows;
                    break;
                case 6:
                    if (InputHandler.KeyDown(Keys.Right) || InputHandler.KeyPressed(Keys.Enter))
                        GameConfiguration.WallPercentage = (GameConfiguration.WallPercentage + 1) % 101;
                    else if (InputHandler.KeyDown(Keys.Left))
                    {
                        if (GameConfiguration.WallPercentage <= 0)
                            GameConfiguration.WallPercentage = 100;
                        else
                            GameConfiguration.WallPercentage--;
                    }
                    break;
                case 7:
                    if (InputHandler.KeyDown(Keys.Right) || InputHandler.KeyPressed(Keys.Enter))
                        Config.ItemNumber = (Config.ItemNumber + 1) % 101;
                    else if (InputHandler.KeyDown(Keys.Left))
                    {
                        if (Config.ItemNumber <= 0)
                            Config.ItemNumber = 100;
                        else
                            Config.ItemNumber--;
                    }
                    break;
                case 8:
                    if(InputHandler.KeyPressed(Keys.Enter))
                        StateManager.ChangeState(FinalBomber.Instance.SuddenDeathMenuScreen);
                    break;
                case 9:
                    if (InputHandler.KeyPressed(Keys.Enter))
                        StateManager.ChangeState(FinalBomber.Instance.ItemMenuScreen);
                    break;
                case 10:
                    if (InputHandler.KeyPressed(Keys.Enter))
                    {
                        Config.PlayersPositions = new Point[]
                        {
                            new Point(1, 1),
                            new Point(Config.MapSize.X - 2, Config.MapSize.Y - 2),
                            new Point(1, Config.MapSize.Y - 2),
                            new Point(Config.MapSize.X - 2, 1),
                            new Point((int)Math.Ceiling((double)(Config.MapSize.X - 2)/(double)2), 
                                (int)Math.Ceiling((double)(Config.MapSize.Y - 2)/(double)2))
                        };
                        StateManager.ChangeState(FinalBomber.Instance.GamePlayScreen);
                    }
                    break;
            }

            if (InputHandler.KeyPressed(Keys.Up))
            {
                if (IndexMenu == 6 && _disabledArrows)
                    IndexMenu = 4;
            }
            else if (InputHandler.KeyPressed(Keys.Down))
            {
                if (IndexMenu == 4 && _disabledArrows)
                    IndexMenu = 6;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            FinalBomber.Instance.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Matrix.Identity);

            base.Draw(gameTime);

            ControlManager.Draw(FinalBomber.Instance.SpriteBatch);

            int xLag = 0;
            int yLag = 0;
            for (int i = 0; i < MenuString.Length; i++)
            {
                if (i < 2)
                    yLag = i;
                else if (i >= 2 && i <= 3)
                {
                    xLag = Config.MapSize.X >= 10 ? 70 : 60;
                    if (i == 3)
                    {
                        if (Config.MapSize.Y >= 10)
                        {
                            if (Config.MapSize.X >= 10)
                                xLag += 70;
                            else
                                xLag += 60;
                        }
                        else
                        {
                            if (Config.MapSize.X >= 10)
                                xLag += 60;
                            else
                                xLag += 50;
                        }
                    }
                }
                else if (i > 3)
                {
                    yLag = i - 2;
                    xLag = 0;
                }

                if (i >= 1 && i <= 3)
                    xLag -= 50;
                else if (i >= 4 && i <= 7)
                    xLag -= 50;
                else if (i == 0)
                    xLag -= 30;

                string text = MenuString[i];

                Color textColor = Color.Black;
                if (i == IndexMenu)
                    textColor = Color.Green;
                else if (i == 5 && _disabledArrows)
                    textColor = Color.Gray;

                FinalBomber.Instance.SpriteBatch.DrawString(BigFont, text,
                    new Vector2(MenuPosition.X - BigFont.MeasureString(text).X / 2 + xLag,
                        MenuPosition.Y + BigFont.MeasureString(text).Y * yLag - BigFont.MeasureString(text).Y / 2), textColor);
                
                switch(i)
                {
                    case 0:
                        FinalBomber.Instance.SpriteBatch.DrawString(BigFont, ": " + Config.PlayersNumber,
                        new Vector2(MenuPosition.X - BigFont.MeasureString(text).X / 2 + xLag + BigFont.MeasureString(text).X,
                            MenuPosition.Y + BigFont.MeasureString(text).Y * yLag - BigFont.MeasureString(text).Y / 2), Color.Black);
                        break;
                    case 1:
                        FinalBomber.Instance.SpriteBatch.DrawString(BigFont, ": ",
                        new Vector2(MenuPosition.X - BigFont.MeasureString(text).X / 2 + xLag + BigFont.MeasureString(text).X,
                            MenuPosition.Y + BigFont.MeasureString(text).Y * yLag - BigFont.MeasureString(text).Y / 2), Color.Black);
                        break;
                    case 2:
                        FinalBomber.Instance.SpriteBatch.DrawString(BigFont, "x",
                        new Vector2(MenuPosition.X - BigFont.MeasureString(text).X / 2 + xLag + BigFont.MeasureString(text).X,
                            MenuPosition.Y + BigFont.MeasureString(text).Y * yLag - BigFont.MeasureString(text).Y / 2), Color.Black);
                        break;
                    case 4:
                        FinalBomber.Instance.SpriteBatch.DrawString(BigFont, ": " + Config.ActiveTeleporters,
                        new Vector2(MenuPosition.X - BigFont.MeasureString(text).X / 2 + xLag + BigFont.MeasureString(text).X,
                            MenuPosition.Y + BigFont.MeasureString(text).Y * yLag - BigFont.MeasureString(text).Y / 2), Color.Black);
                        break;
                    case 5:
                        if(!_disabledArrows)
                            textColor = Color.Black;
                        FinalBomber.Instance.SpriteBatch.DrawString(BigFont, ": " + Config.ActiveArrows,
                        new Vector2(MenuPosition.X - BigFont.MeasureString(text).X / 2 + xLag + BigFont.MeasureString(text).X,
                            MenuPosition.Y + BigFont.MeasureString(text).Y * yLag - BigFont.MeasureString(text).Y / 2), textColor);
                        break;
                    case 6:
                        FinalBomber.Instance.SpriteBatch.DrawString(BigFont, ": " + GameConfiguration.WallPercentage + "%",
                        new Vector2(MenuPosition.X - BigFont.MeasureString(text).X / 2 + xLag + BigFont.MeasureString(text).X,
                            MenuPosition.Y + BigFont.MeasureString(text).Y * yLag - BigFont.MeasureString(text).Y / 2), Color.Black);
                        break;
                    case 7:
                        FinalBomber.Instance.SpriteBatch.DrawString(BigFont, ": " + Config.ItemNumber + "%",
                        new Vector2(MenuPosition.X - BigFont.MeasureString(text).X / 2 + xLag + BigFont.MeasureString(text).X,
                            MenuPosition.Y + BigFont.MeasureString(text).Y * yLag - BigFont.MeasureString(text).Y / 2), Color.Black);
                        break;
                }
            }

            FinalBomber.Instance.SpriteBatch.End();
        }

        #endregion

        #region Abstract Method Region
        #endregion

        #region Method Region
        #endregion
    }
}
