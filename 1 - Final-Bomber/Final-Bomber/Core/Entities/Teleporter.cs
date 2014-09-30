using FBLibrary.Core;
using FBClient.Screens;
using FBClient.Screens.GameScreens;
using FBClient.Sprites;
using FBClient.WorldEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FBClient.Core.Entities
{
    public class Teleporter : BaseEntity
    {
        #region Field Region

        private bool _isAlive;
        public AnimatedSprite Sprite { get; protected set; }

        #endregion

        #region Property Region

        public bool IsAlive
        {
            get { return _isAlive; }
        }

        #endregion

        #region Constructor Region

        public Teleporter(Point cellPosition)
            : base(cellPosition)
        {
            var spriteTexture = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Sprites/teleporter");
            var animation = new Animation(2, 32, 32, 0, 0, 2);
            Sprite = new AnimatedSprite(spriteTexture, animation) { IsAnimating = true };

            _isAlive = true;
        }

        #endregion

        #region XNA Method Region

        public void Update(GameTime gameTime)
        {
            Sprite.Update(gameTime);
        }

        public void Draw(GameTime gameTime, Camera2D camera)
        {
            if (camera.IsVisible(Position))
                Sprite.Draw(gameTime, FinalBomber.Instance.SpriteBatch, Position);
        }

        #endregion

        #region Public Method Region

        public void ChangeEntityPosition(DynamicEntity entity, Map map)
        {
            bool allTeleporterCellTaken = true;

            foreach (Teleporter t in map.TeleporterList)
            {
                Point position = t.CellPosition;
                if (position != CellPosition && map.Board[position.X, position.Y] is Teleporter)
                    allTeleporterCellTaken = false;
            }

            if (!allTeleporterCellTaken)
            {
                Point position = CellPosition;
                while (position == CellPosition)
                {
                    position = FinalBomber.Instance.GamePlayScreen.TeleporterList[
                        GamePlayScreen.Random.Next(FinalBomber.Instance.GamePlayScreen.TeleporterList.Count)].
                        CellPosition;
                }

                var bomb = entity as Bomb;
                if (bomb != null)
                {
                    bomb.ChangeSpeed(bomb.Speed + Config.BombSpeedIncrementeur);
                    bomb.ResetTimer();
                    bomb.ChangePosition(position);
                }
                else
                    entity.ChangePosition(position);
            }
        }

        #endregion

        #region Override Method Region

        public override void Destroy()
        {
        }

        public override void Remove()
        {
            _isAlive = false;
        }

        #endregion
    }
}