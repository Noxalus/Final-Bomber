using System;
using System.Collections.Generic;
using System.Diagnostics;
using FBLibrary.Core;
using FBLibrary.Core.BaseEntities;
using Final_Bomber.Controls;
using Final_Bomber.Core;
using Final_Bomber.Core.Entities;
using Final_Bomber.Entities;
using Final_Bomber.Screens;
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

        private readonly AnimatedSprite _playerDeathAnimation;
        protected TimeSpan BombTimerSaved;

        protected bool IsMoving;
        protected LookDirection PreviousLookDirection;

        // Bad item
        protected float SpeedSaved;
        private bool _cellTeleporting;
        private float _invincibleBlinkFrequency;
        private TimeSpan _invincibleBlinkTimer;
        private TimeSpan _invincibleTimer;

        public AnimatedSprite Sprite { get; protected set; }

        #endregion

        #region Property Region

        public Camera Camera { get; private set; }

        public bool InDestruction { get; private set; }

        public bool IsInvincible { get; private set; }

        public bool HasBadItemEffect { get; private set; }

        public BadItemEffect BadItemEffect { get; private set; }

        public TimeSpan BadItemTimer { get; private set; }

        public TimeSpan BadItemTimerLenght { get; private set; }

        #endregion

        #region Constructor Region

        protected Player(int id)
            : base(id)
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

            IsMoving = false;
            InDestruction = false;
            IsInvincible = true;

            _invincibleTimer = GameConfiguration.PlayerInvincibleTimer;
            _invincibleBlinkFrequency = Config.InvincibleBlinkFrequency;
            _invincibleBlinkTimer = TimeSpan.FromSeconds(_invincibleBlinkFrequency);

            PreviousLookDirection = CurrentDirection;

            // Bad item
            HasBadItemEffect = false;
            BadItemTimer = TimeSpan.Zero;
            BadItemTimerLenght = TimeSpan.Zero;
        }

        #endregion

        #region XNA Method Region

        public virtual void Update(GameTime gameTime, Map map, int[,] hazardMap)
        {
            if (IsAlive && !InDestruction)
            {
                PreviousLookDirection = CurrentDirection;

                Sprite.Update(gameTime);

                #region Invincibility

                if (!GameConfiguration.Invincible && IsInvincible)
                {
                    if (_invincibleTimer >= TimeSpan.Zero)
                    {
                        _invincibleTimer -= gameTime.ElapsedGameTime;
                        if (_invincibleBlinkTimer >= TimeSpan.Zero)
                            _invincibleBlinkTimer -= gameTime.ElapsedGameTime;
                        else
                            _invincibleBlinkTimer = TimeSpan.FromSeconds(_invincibleBlinkFrequency);
                    }
                    else
                    {
                        _invincibleTimer = GameConfiguration.PlayerInvincibleTimer;
                        IsInvincible = false;
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

                #region Item

                /*
                if (FinalBomber.Instance.GamePlayScreen.World.Levels[FinalBomber.Instance.GamePlayScreen.World.CurrentLevel].
                    Map[CellPosition.X, CellPosition.Y] is Item)
                {
                    var item = (Item)(FinalBomber.Instance.GamePlayScreen.World.Levels[FinalBomber.Instance.GamePlayScreen.World.CurrentLevel].
                        Map[CellPosition.X, CellPosition.Y]);
                    if (!item.InDestruction)
                    {
                        if (!HasBadItemEffect || (HasBadItemEffect && item.Type != PowerUpType.BadItem))
                        {
                            item.ApplyItem(this);
                            FinalBomber.Instance.GamePlayScreen.ItemPickUpSound.Play();
                            item.Remove();
                        }
                    }
                }
                */
                // Have caught a bad item
                if (HasBadItemEffect)
                {
                    BadItemTimer += gameTime.ElapsedGameTime;
                    if (BadItemTimer >= BadItemTimerLenght)
                    {
                        RemoveBadItem();
                    }
                }

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

            else if (_playerDeathAnimation.IsAnimating)
            {
                _playerDeathAnimation.Update(gameTime);

                if (_playerDeathAnimation.Animation.CurrentFrame == _playerDeathAnimation.Animation.FrameCount - 1)
                    Remove();
            }
            #endregion

            #region Edge wall gameplay

            else if (OnEdge &&
                     (!Config.ActiveSuddenDeath ||
                      (Config.ActiveSuddenDeath && !FinalBomber.Instance.GamePlayScreen.SuddenDeath.HasStarted)))
            {
                Sprite.Update(gameTime);

                MoveFromEdgeWall();
            }

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
                        FinalBomber.Instance.SpriteBatch.DrawString(ControlManager.SpriteFont, Name, playerNamePosition,
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
            _invincibleTimer = GameConfiguration.PlayerInvincibleTimer;
            _invincibleBlinkFrequency = Config.InvincibleBlinkFrequency;
            _invincibleBlinkTimer = TimeSpan.FromSeconds(_invincibleBlinkFrequency);
        }

        #endregion

        #region Protected Method Region

        protected virtual void Move(GameTime gameTime, Map map, int[,] hazardMap)
        {
            ComputeWallCollision(map);
        }

        protected void UpdatePlayerPosition()
        {
            #region Update player's position

            if (IsChangingCell())
            {
                if (FinalBomber.Instance.GamePlayScreen.World.Levels[
                    FinalBomber.Instance.GamePlayScreen.World.CurrentLevel].
                    Board[PreviousCellPosition.X, PreviousCellPosition.Y] == this)
                {
                    FinalBomber.Instance.GamePlayScreen.World.Levels[
                        FinalBomber.Instance.GamePlayScreen.World.CurrentLevel].
                        Board[PreviousCellPosition.X, PreviousCellPosition.Y] = null;
                }

                if (_cellTeleporting)
                    _cellTeleporting = false;
            }
            else
            {
                if (FinalBomber.Instance.GamePlayScreen.World.Levels[
                    FinalBomber.Instance.GamePlayScreen.World.CurrentLevel].
                    Board[CellPosition.X, CellPosition.Y] == null)
                {
                    FinalBomber.Instance.GamePlayScreen.World.Levels[
                        FinalBomber.Instance.GamePlayScreen.World.CurrentLevel].
                        Board[CellPosition.X, CellPosition.Y] = this;
                }
            }

            #endregion
        }

        #endregion

        #region Public Method Region

        public void IncreaseTotalBombNumber(int incr)
        {
            if (TotalBombAmount + incr > Config.MaxBombNumber)
            {
                TotalBombAmount = Config.MaxBombNumber;
                CurrentBombAmount = TotalBombAmount;
            }
            else if (TotalBombAmount + incr < Config.MinBombNumber)
            {
                TotalBombAmount = Config.MinBombNumber;
                CurrentBombAmount = TotalBombAmount;
            }
            else
            {
                TotalBombAmount += incr;
                CurrentBombAmount += incr;
            }
        }

        public void IncreasePower(int incr)
        {
            if (BombPower + incr > Config.MaxBombPower)
                BombPower = Config.MaxBombPower;
            else if (BombPower + incr < Config.MinBombPower)
                BombPower = Config.MinBombPower;
            else
                BombPower += incr;
        }

        public void IncreaseSpeed(float incr)
        {
            Speed += incr;
        }

        public virtual void ApplyBadItem(BadItemEffect effect)
        {
            HasBadItemEffect = true;
            BadItemEffect = effect;
            BadItemTimerLenght =
                TimeSpan.FromSeconds(GamePlayScreen.Random.Next(Config.BadItemTimerMin, Config.BadItemTimerMax));
        }

        protected virtual void RemoveBadItem()
        {
            HasBadItemEffect = false;
            BadItemTimer = TimeSpan.Zero;
            BadItemTimerLenght = TimeSpan.Zero;
        }

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
            PreviousLookDirection = CurrentDirection;
            CurrentDirection = (LookDirection)newLookDirection;
            Debug.Print("New look direction: " + (LookDirection)newLookDirection);
        }

        #endregion

        #region Override Method Region

        public override void Destroy()
        {
            if (!InDestruction)
            {
                Sprite.IsAnimating = false;
                InDestruction = true;
                FinalBomber.Instance.GamePlayScreen.PlayerDeathSound.Play();
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