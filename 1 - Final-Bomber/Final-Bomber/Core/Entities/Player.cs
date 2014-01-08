using System;
using System.Collections.Generic;
using System.Diagnostics;
using FBLibrary;
using FBLibrary.Core;
using FBLibrary.Core.BaseEntities;
using Final_Bomber.Controls;
using Final_Bomber.Core;
using Final_Bomber.Core.Entities;
using Final_Bomber.Entities;
using Final_Bomber.Screens;
using Final_Bomber.Screens.GameScreens;
using Final_Bomber.Sprites;
using Final_Bomber.TileEngine;
using Final_Bomber.WorldEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Bomber.Entities
{
    public abstract class Player : BasePlayer
    {
        #region Field Region

        private AnimatedSprite _playerDeathAnimation;
        protected TimeSpan BombTimerSaved;

        // Bad item
        protected float SpeedSaved;
        private bool _cellTeleporting;
        private float _invincibleBlinkFrequency;
        private TimeSpan _invincibleBlinkTimer;

        // Reference on GameTime
        private GameTime _gameTime;

        public AnimatedSprite Sprite { get; protected set; }

        #endregion

        #region Property Region

        #endregion

        #region Constructor Region

        protected Player(int id)
            : base(id)
        {
            Initiliaze();
        }

        protected Player(int id, PlayerStats stats)
            : base(id, stats)
        {
            Initiliaze();
        }

        #endregion

        private void Initiliaze()
        {
            IsMoving = false;

            _invincibleBlinkFrequency = Config.InvincibleBlinkFrequency;
            _invincibleBlinkTimer = TimeSpan.FromSeconds(_invincibleBlinkFrequency);

            _gameTime = new GameTime();
        }

        #region XNA Method Region

        public void LoadContent()
        {
            const int animationFramesPerSecond = 10;
            var animations = new Dictionary<AnimationKey, Animation>();

            var animation = new Animation(4, 23, 23, 0, 0, animationFramesPerSecond);
            animations.Add(AnimationKey.Down, animation);

            animation = new Animation(4, 23, 23, 0, 23, animationFramesPerSecond);
            animations.Add(AnimationKey.Left, animation);

            animation = new Animation(4, 23, 23, 0, 46, animationFramesPerSecond);
            animations.Add(AnimationKey.Right, animation);

            animation = new Animation(4, 23, 23, 0, 69, animationFramesPerSecond);
            animations.Add(AnimationKey.Up, animation);

            var spriteTexture = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Characters/player1");

            Sprite = new AnimatedSprite(spriteTexture, animations);
            Sprite.ChangeFramesPerSecond(animationFramesPerSecond);

            var playerDeathTexture = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Characters/player1Death");
            animation = new Animation(8, 23, 23, 0, 0, 4);
            _playerDeathAnimation = new AnimatedSprite(playerDeathTexture, animation)
            {
                IsAnimating = false
            };
        }

        public virtual void Update(GameTime gameTime, Map map, int[,] hazardMap)
        {
            _gameTime = gameTime;

            if (IsAlive && !InDestruction)
            {
                PreviousDirection = CurrentDirection;

                Sprite.Update(gameTime);

                #region Invincibility

                if (!GameConfiguration.Invincible && IsInvincible)
                {
                    if (InvincibleTimer >= TimeSpan.Zero)
                    {
                        if (_invincibleBlinkTimer >= TimeSpan.Zero)
                            _invincibleBlinkTimer -= gameTime.ElapsedGameTime;
                        else
                            _invincibleBlinkTimer = TimeSpan.FromSeconds(_invincibleBlinkFrequency);
                    }
                }

                #endregion

                #region Moving

                Move(gameTime, map, hazardMap);

                #endregion

                #region Bomb

                #region Push a bomb

                if (Config.PlayerCanPush)
                {
                    if (CurrentDirection != LookDirection.Idle)
                    {
                        Point direction = Point.Zero;
                        switch (CurrentDirection)
                        {
                            case LookDirection.Up:
                                direction = new Point(CellPosition.X, CellPosition.Y - 1);
                                break;
                            case LookDirection.Down:
                                direction = new Point(CellPosition.X, CellPosition.Y + 1);
                                break;
                            case LookDirection.Left:
                                direction = new Point(CellPosition.X - 1, CellPosition.Y);
                                break;
                            case LookDirection.Right:
                                direction = new Point(CellPosition.X + 1, CellPosition.Y);
                                break;
                        }
                        Bomb bomb = BombAt(direction);
                        if (bomb != null)
                            bomb.ChangeDirection(CurrentDirection, Id);
                    }
                }

                #endregion

                #endregion

                #region Teleporter

                /*
                if (!_cellTeleporting && FinalBomber.Instance.GamePlayScreen.World.Levels[FinalBomber.Instance.GamePlayScreen.World.CurrentLevel].
                    Map[CellPosition.X, CellPosition.Y] is Teleporter)
                {
                    var teleporter = (Teleporter)(FinalBomber.Instance.GamePlayScreen.World.Levels[FinalBomber.Instance.GamePlayScreen.World.CurrentLevel].
                        Map[CellPosition.X, CellPosition.Y]);

                    teleporter.ChangePosition(this);
                    _cellTeleporting = true;
                }
                */

                #endregion
            }

            #region Death

            else if (InDestruction)
            {
                _playerDeathAnimation.Update(gameTime);
            }
            #endregion

            #region Edge wall gameplay
            /*
            else if (OnEdge &&
                     (!Config.ActiveSuddenDeath ||
                      (Config.ActiveSuddenDeath && !FinalBomber.Instance.GamePlayScreen.SuddenDeath.HasStarted)))
            {
                Sprite.Update(gameTime);

                MoveFromEdgeWall();
            }
            */
            #endregion

            #region Camera
            /*
            Camera.Update(gameTime);
            
            if (Config.Debug)
            {
                if (InputHandler.KeyDown(Microsoft.Xna.Framework.Input.Keys.PageUp) ||
                    InputHandler.ButtonReleased(Buttons.LeftShoulder, PlayerIndex.One))
                {
                    Camera.ZoomIn();
                    if (Camera.CameraMode == CameraMode.Follow)
                        Camera.LockToSprite(Sprite);
                }
                else if (InputHandler.KeyDown(Microsoft.Xna.Framework.Input.Keys.PageDown))
                {
                    Camera.ZoomOut();
                    if (Camera.CameraMode == CameraMode.Follow)
                        Camera.LockToSprite(Sprite);
                }
                else if (InputHandler.KeyDown(Microsoft.Xna.Framework.Input.Keys.End))
                {
                    Camera.ZoomReset();
                }

                if (InputHandler.KeyReleased(Microsoft.Xna.Framework.Input.Keys.F))
                {
                    Camera.ToggleCameraMode();
                    if (Camera.CameraMode == CameraMode.Follow)
                        Camera.LockToSprite(Sprite);
                }

                if (Camera.CameraMode != CameraMode.Follow)
                {
                    if (InputHandler.KeyReleased(Microsoft.Xna.Framework.Input.Keys.L))
                    {
                        Camera.LockToSprite(Sprite);
                    }
                }
            }

            if (IsMoving && Camera.CameraMode == CameraMode.Follow)
                Camera.LockToSprite(Sprite);
            */

            #endregion

            // Call DynamicEntity Update method
            base.Update();
        }

        public void Draw(GameTime gameTime)
        {
            var playerNamePosition = new Vector2(
                Position.X + Engine.Origin.X + Sprite.Width / 2f -
                ControlManager.SpriteFont.MeasureString(Name).X / 2 + 5,
                Position.Y + Engine.Origin.Y - 25 -
                ControlManager.SpriteFont.MeasureString(Name).Y / 2);

            if ((IsAlive && !InDestruction) || OnEdge)
            {
                if (IsInvincible)
                {
                    if (_invincibleBlinkTimer > TimeSpan.FromSeconds(_invincibleBlinkFrequency * 0.5f))
                    {
                        Sprite.Draw(gameTime, FinalBomber.Instance.SpriteBatch, Position);
                        FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, Name,
                            playerNamePosition,
                            Color.Black);
                    }
                }
                else
                {
                    Sprite.Draw(gameTime, FinalBomber.Instance.SpriteBatch, Position);
                    FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, Name, playerNamePosition,
                        Color.Black);
                }
            }
            else
            {
                _playerDeathAnimation.Draw(gameTime, FinalBomber.Instance.SpriteBatch, Position);
                FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, Name, playerNamePosition,
                    Color.Black);
            }
        }

        #endregion

        #region Method Region

        #region Private Method Region

        private Bomb BombAt(Point cell)
        {
            if (cell.X >= 0 && cell.Y >= 0 &&
                cell.X <
                FinalBomber.Instance.GamePlayScreen.World.Levels[FinalBomber.Instance.GamePlayScreen.World.CurrentLevel]
                    .Size.X &&
                cell.Y <
                FinalBomber.Instance.GamePlayScreen.World.Levels[FinalBomber.Instance.GamePlayScreen.World.CurrentLevel]
                    .Size.Y)
            {
                Bomb bomb = FinalBomber.Instance.GamePlayScreen.BombList.Find(b => b.CellPosition == cell);
                return (bomb);
            }
            return null;
        }

        private void Invincibility()
        {
            IsInvincible = true;
            _invincibleBlinkFrequency = Config.InvincibleBlinkFrequency;
            _invincibleBlinkTimer = TimeSpan.FromSeconds(_invincibleBlinkFrequency);
        }

        #endregion

        #region Protected Method Region

        protected virtual void Move(GameTime gameTime, Map map, int[,] hazardMap)
        {
            if (IsMoving)
                ComputeWallCollision(map);
        }

        protected void UpdatePlayerPosition(Map map)
        {
            #region Update player's position

            if (IsChangingCell())
            {
                if (map.Board[PreviousCellPosition.X, PreviousCellPosition.Y] == this)
                {
                    map.Board[PreviousCellPosition.X, PreviousCellPosition.Y] = null;
                }

                if (_cellTeleporting)
                    _cellTeleporting = false;
            }
            else
            {
                if (map.Board[CellPosition.X, CellPosition.Y] == null)
                {
                    map.Board[CellPosition.X, CellPosition.Y] = this;
                }
            }

            #endregion
        }

        protected override float GetMovementSpeed()
        {
            var dt = (float)_gameTime.ElapsedGameTime.TotalSeconds;
            float rtn = (Speed * dt);
            return rtn;
        }

        #endregion

        #region Override Method Region

        public override void Destroy()
        {
            if (!InDestruction)
            {
                Sprite.IsAnimating = false;
                InDestruction = true;
                NetworkTestScreen.GameManager.PlayerDeathSound.Play();
                _playerDeathAnimation.IsAnimating = true;
            }
        }

        public override void Remove()
        {
            _playerDeathAnimation.IsAnimating = false;
            InDestruction = false;
            IsAlive = false;

            // Replacing for the gameplay on the edges
            // Right side
            if (Config.MapSize.X - CellPosition.X < Config.MapSize.X / 2)
            {
                Sprite.CurrentAnimation = AnimationKey.Left;
                Position = new Vector2((Config.MapSize.X * Engine.TileWidth) - Engine.TileWidth, Position.Y);
            }
            // Left side
            else
            {
                Sprite.CurrentAnimation = AnimationKey.Right;
                Position = new Vector2(0, Position.Y);
            }
        }

        protected virtual void MoveFromEdgeWall()
        {
        }

        #endregion

        #endregion


        public void Rebirth(Vector2 position)
        {
            IsAlive = true;
            Sprite.IsAnimating = true;
            InDestruction = false;
            Position = position;
            Sprite.CurrentAnimation = AnimationKey.Down;
            _playerDeathAnimation.IsAnimating = false;

            Invincibility();
        }

        public virtual void ChangeLookDirection(byte newLookDirection)
        {
            PreviousDirection = CurrentDirection;
            CurrentDirection = (LookDirection)newLookDirection;
            Debug.Print("New look direction: " + (LookDirection)newLookDirection);
        }

        protected override int GetTime()
        {
            return _gameTime.ElapsedGameTime.Milliseconds;
        }
    }
}

public class PlayerCollection : List<Player>
{
    public Player GetPlayerByID(int playerID)
    {
        foreach (Player player in this)
        {
            if (player.Id == playerID)
                return player;
        }
        return null;
    }
}