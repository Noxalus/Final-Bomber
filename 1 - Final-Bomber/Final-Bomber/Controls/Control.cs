using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FBClient.Controls
{
    public abstract class Control
    {
        #region Field Region

        private Vector2 _position;

        #endregion

        #region Event Region

        public event EventHandler Selected;

        #endregion

        #region Property Region

        public string Name { get; set; }

        public string Text { get; set; }

        public Vector2 Size { get; set; }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                _position.Y = (int) _position.Y;
            }
        }

        public object Value { get; set; }

        public virtual bool HasFocus { get; set; }

        public bool Enabled { get; private set; }

        public bool Visible { get; private set; }

        public bool TabStop { get; private set; }

        private SpriteFont SpriteFont { get; set; }

        private Color Color { get; set; }

        public string Type { get; set; }

        #endregion

        #region Constructor Region

        protected Control(bool tabStop)
        {
            TabStop = tabStop;
            Color = Color.White;
            Enabled = true;
            Visible = true;
            SpriteFont = ControlManager.SpriteFont;
        }

        #endregion

        #region Abstract Methods

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract void HandleInput(PlayerIndex playerIndex);

        #endregion

        #region Virtual Methods

        protected virtual void OnSelected(EventArgs e)
        {
            if (Selected != null)
            {
                Selected(this, e);
            }
        }

        #endregion
    }
}