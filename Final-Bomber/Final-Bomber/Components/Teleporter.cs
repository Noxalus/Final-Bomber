using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Final_Bomber.Sprites;
using Final_Bomber.WorldEngine;

namespace Final_Bomber.Components
{
    public class Teleporter : MapItem
    {
        #region Field Region
        public override Sprites.AnimatedSprite Sprite { get; protected set; }
        private FinalBomber gameRef;
        private bool isAlive;
        #endregion

        #region Property Region

        public bool IsAlive
        {
            get { return isAlive; }
        }

        #endregion

        #region Constructor Region
        public Teleporter(FinalBomber game, Vector2 position)
        {
            this.gameRef = game;
            Texture2D spriteTexture = gameRef.Content.Load<Texture2D>("Graphics/Characters/teleporter");
            Animation animation = new Animation(2, 32, 32, 0, 0, 2);
            Sprite = new Sprites.AnimatedSprite(spriteTexture, animation, position);
            Sprite.IsAnimating = true;

            isAlive = true;
        }
        #endregion

        #region XNA Method Region

        public override void Update(GameTime gameTime)
        {
            Sprite.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Sprite.Draw(gameTime, gameRef.SpriteBatch);
        }

        #endregion

        #region Public Method Region

        public void ChangePosition(MapItem mapItem)
        {
            bool allTeleporterCellTaken = true;
            Point position = Point.Zero;
            Level level = gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel];
            for (int i = 0; i < gameRef.GamePlayScreen.TeleporterList.Count; i++)
            {
                position = gameRef.GamePlayScreen.TeleporterList[i].Sprite.CellPosition;
                if (position != this.Sprite.CellPosition && level.Map[position.X, position.Y] is Teleporter)
                    allTeleporterCellTaken = false;
            }

            if (!allTeleporterCellTaken)
            {
                position = Sprite.CellPosition;
                while (position == Sprite.CellPosition)
                {
                    position = gameRef.GamePlayScreen.TeleporterList[
                    gameRef.GamePlayScreen.Random.Next(gameRef.GamePlayScreen.TeleporterList.Count)].
                    Sprite.CellPosition;
                }

                if (mapItem is Bomb)
                {
                    Bomb b = (Bomb)mapItem;
                    b.ChangeSpeed(mapItem.Sprite.Speed + Config.BombSpeedIncrementeur);
                    b.ResetTimer();
                    b.Sprite.ChangePosition(position);
                }
                else
                    mapItem.Sprite.ChangePosition(position);
            }
        }

        #endregion

        #region Override Method Region
        public override void Destroy()
        {
        }

        public override  void Remove()
        {
            this.isAlive = false;
        }
        #endregion
    }
}
