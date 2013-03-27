using Final_Bomber.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Final_Bomber.Sprites;
using Final_Bomber.WorldEngine;

namespace Final_Bomber.Components
{
    public class Teleporter : MapItem
    {
        #region Field Region
        public override sealed Sprites.AnimatedSprite Sprite { get; protected set; }
        private readonly FinalBomber _gameRef;
        private bool _isAlive;
        #endregion

        #region Property Region

        public bool IsAlive
        {
            get { return _isAlive; }
        }

        #endregion

        #region Constructor Region
        public Teleporter(FinalBomber game, Vector2 position)
        {
            this._gameRef = game;
            var spriteTexture = _gameRef.Content.Load<Texture2D>("Graphics/Characters/teleporter");
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
            Sprite.Draw(gameTime, _gameRef.SpriteBatch);
        }

        #endregion

        #region Public Method Region

        public void ChangePosition(MapItem mapItem)
        {
            bool allTeleporterCellTaken = true;
            Level level = _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel];
            foreach (Teleporter t in _gameRef.GamePlayScreen.TeleporterList)
            {
                Point position = t.Sprite.CellPosition;
                if (position != this.Sprite.CellPosition && level.Map[position.X, position.Y] is Teleporter)
                    allTeleporterCellTaken = false;
            }

            if (!allTeleporterCellTaken)
            {
                Point position = Sprite.CellPosition;
                while (position == Sprite.CellPosition)
                {
                    position = _gameRef.GamePlayScreen.TeleporterList[
                    GamePlayScreen.Random.Next(_gameRef.GamePlayScreen.TeleporterList.Count)].
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
