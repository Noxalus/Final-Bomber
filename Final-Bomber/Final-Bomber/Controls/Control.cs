using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Bomber.Controls
{
    public abstract class Control
    {
        #region Field Region

        protected Color color;
        protected bool enabled;
        protected bool hasFocus;
        protected string name;
        protected Vector2 position;
        protected Vector2 size;
        protected SpriteFont spriteFont;
        protected bool tabStop;
        protected string text;
        protected string type;
        protected object value;
        protected bool visible;

        #endregion

        #region Event Region

        public event EventHandler Selected;

        #endregion

        #region Property Region

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public Vector2 Size
        {
            get { return size; }
            set { size = value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                position.Y = (int) position.Y;
            }
        }

        public object Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public virtual bool HasFocus
        {
            get { return hasFocus; }
            set { hasFocus = value; }
        }

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        public bool TabStop
        {
            get { return tabStop; }
            set { tabStop = value; }
        }

        public SpriteFont SpriteFont
        {
            get { return spriteFont; }
            set { spriteFont = value; }
        }

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        #endregion

        #region Constructor Region

        protected Control()
        {
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