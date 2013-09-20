using Final_Bomber.Entities;
using Final_Bomber.Screens;
using Final_Bomber.Sprites;
using Final_Bomber.WorldEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Bomber.Core.Entities
{
    public class Teleporter : StaticEntity
    {
        #region Field Region
        public override sealed Sprites.AnimatedSprite Sprite { get; protected set; }
        private bool _isAlive;
        #endregion

        #region Property Region

        public bool IsAlive
        {
            get { return _isAlive; }
        }

        #endregion

        #region Constructor Region
        public Teleporter(Point position)
        {
            var spriteTexture = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Characters/teleporter");
            var animation = new Animation(2, 32, 32, 0, 0, 2);
            Sprite = new Sprites.AnimatedSprite(spriteTexture, animation, position) {IsAnimating = true};

            _isAlive = true;
        }
        #endregion

        #region XNA Method Region

        public override void Update(GameTime gameTime)
        {
            Sprite.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Sprite.Draw(gameTime, FinalBomber.Instance.SpriteBatch);
        }

        #endregion

        #region Public Method Region

        public void ChangePosition(Entity mapItem)
        {
            bool allTeleporterCellTaken = true;
            Map level = FinalBomber.Instance.GamePlayScreen.World.Levels[FinalBomber.Instance.GamePlayScreen.World.CurrentLevel];
            foreach (Teleporter t in FinalBomber.Instance.GamePlayScreen.TeleporterList)
            {
                Point position = t.Sprite.CellPosition;
                if (position != this.Sprite.CellPosition && level.Board[position.X, position.Y] is Teleporter)
                    allTeleporterCellTaken = false;
            }

            if (!allTeleporterCellTaken)
            {
                Point position = Sprite.CellPosition;
                while (position == Sprite.CellPosition)
                {
                    position = FinalBomber.Instance.GamePlayScreen.TeleporterList[
                    GamePlayScreen.Random.Next(FinalBomber.Instance.GamePlayScreen.TeleporterList.Count)].
                    Sprite.CellPosition;
                }

                var bomb = mapItem as Bomb;
                if (bomb != null)
                {
                    bomb.ChangeSpeed(mapItem.Sprite.Speed + Config.BombSpeedIncrementeur);
                    bomb.ResetTimer();
                    bomb.Sprite.ChangePosition(position);
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
            this._isAlive = false;
        }
        #endregion
    }
}
