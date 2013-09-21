using FBLibrary.Core;
using Final_Bomber.Screens;
using Final_Bomber.Sprites;
using Final_Bomber.WorldEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Bomber.Core.Entities
{
    public class Teleporter : BaseEntity, IEntity
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

        public Teleporter(Point position)
        {
            var spriteTexture = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Characters/teleporter");
            var animation = new Animation(2, 32, 32, 0, 0, 2);
            Sprite = new AnimatedSprite(spriteTexture, animation, position) {IsAnimating = true};

            _isAlive = true;
        }

        #endregion

        #region XNA Method Region

        public void Update(GameTime gameTime)
        {
            Sprite.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            Sprite.Draw(gameTime, FinalBomber.Instance.SpriteBatch);
        }

        #endregion

        #region Public Method Region

        public void ChangePosition(IEntity mapItem, Map map)
        {
            bool allTeleporterCellTaken = true;
         
            foreach (Teleporter t in map.TeleporterList)
            {
                Point position = t.Sprite.CellPosition;
                if (position != Sprite.CellPosition && map.Board[position.X, position.Y] is Teleporter)
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
                    bomb.ChangeSpeed(bomb.Sprite.Speed + Config.BombSpeedIncrementeur);
                    bomb.ResetTimer();
                    bomb.Sprite.ChangePosition(position);
                }
                /* TODO: Fix this modelisation problem !
                else
                    mapItem.ChangePosition(position);
                */
            }
        }

        #endregion

        #region Override Method Region

        public void Destroy()
        {
        }

        public void Remove()
        {
            _isAlive = false;
        }

        #endregion
    }
}