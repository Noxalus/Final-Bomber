using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FBClient.Controls
{
    public abstract partial class GameState : DrawableGameComponent
    {
        #region Fields and Properties

        readonly List<GameComponent> _childComponents;

        public List<GameComponent> Components
        {
            get { return _childComponents; }
        }

        GameState tag;

        public GameState Tag
        {
            get { return tag; }
        }

        protected readonly GameStateManager StateManager;

        #endregion

        #region Constructor Region

        protected GameState(Game game, GameStateManager manager)
            : base(game)
        {
            StateManager = manager;

            _childComponents = new List<GameComponent>();
            tag = this;
        }

        #endregion

        #region XNA Drawable Game Component Methods

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (GameComponent component in _childComponents)
            {
                if (component.Enabled)
                    component.Update(gameTime);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (GameComponent component in _childComponents)
            {
                if (component is DrawableGameComponent)
                {
                    var drawComponent = component as DrawableGameComponent;

                    if (drawComponent.Visible)
                        drawComponent.Draw(gameTime);
                }
            }

            base.Draw(gameTime);
        }

        #endregion

        #region GameState Method Region

        internal protected virtual void StateChange(object sender, EventArgs e)
        {
            if (StateManager.CurrentState == Tag)
                Show();
            else
                Hide();
        }

        protected virtual void Show()
        {
            Visible = true;
            Enabled = true;

            foreach (GameComponent component in _childComponents)
            {
                component.Enabled = true;
                var gameComponent = component as DrawableGameComponent;
                if (gameComponent != null)
                    gameComponent.Visible = true;
            }
        }

        protected virtual void Hide()
        {
            Visible = false;
            Enabled = false;

            foreach (GameComponent component in _childComponents)
            {
                component.Enabled = false;
                var gameComponent = component as DrawableGameComponent;
                if (gameComponent != null)
                    gameComponent.Visible = false;
            }
        }

        #endregion
    }
}
