using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Final_Bomber.Sprites;
using Final_Bomber.WorldEngine;
using Final_Bomber.Controls;
using Microsoft.Xna.Framework.Input;
using Final_Bomber.TileEngine;
using Microsoft.Xna.Framework.Media;

namespace Final_Bomber.Components
{
    public class Bomb : MapItem
    {
        #region Field Region
        private enum ExplosionDirection { DOWN, LEFT, RIGHT, UP, MIDDLE, HORIZONTAL, VERTICAL };

        private FinalBomber gameRef;

        private int id;

        public override AnimatedSprite Sprite { get; protected set; }

        private bool inDestruction;
        private bool isAlive;
        private LookDirection lookDirection;

        private List<Point> actionField;

        private Animation[] explosionAnimations;
        private Dictionary<Point, ExplosionDirection> explosionAnimationsDirection;
        private Texture2D explosionSpriteTexture;
        private SoundEffect explosionSound;
        private bool willExplode;

        private int power;

        TimeSpan timer;
        TimeSpan timerLenght;

        private Point previousCellPosition;
        private bool cellChanging;
        private bool cellTeleporting;

        private int lastPlayerThatPushIt;
        #endregion

        #region Propery Region

        public int Id
        {
            get { return id; }
        }

        public bool IsAlive
        {
            get { return isAlive; }
        }

        public bool InDestruction
        {
            get { return inDestruction; }
        }

        public List<Point> ActionField
        {
            get { return actionField; }
        }

        #endregion

        #region Constructor Region
        public Bomb(FinalBomber game, int id, Point position, int pow, TimeSpan timerLenght, float playerSpeed)
        {
            // The game => you lose :D
            this.gameRef = game;

            // ID => to know whom it is
            this.id = id;

            // Bomb Sprite
            Texture2D spriteTexture = gameRef.Content.Load<Texture2D>("Graphics/Characters/bomb");
            Animation animation = new Animation(3, 32, 32, 0, 0, 3);
            this.Sprite = new AnimatedSprite(spriteTexture, animation, Engine.CellToVector(position));
            this.Sprite.IsAnimating = true;
            this.Sprite.Speed = Config.BaseBombSpeed + playerSpeed;

            // Bomb's explosion animations
            explosionSpriteTexture = gameRef.Content.Load<Texture2D>("Graphics/Characters/explosion");
            int explosionAnimationsFramesPerSecond = Config.BombLatency;
            explosionAnimations = new Animation[]
            {
                new Animation(4, 32, 32, 0, 0, explosionAnimationsFramesPerSecond),
                new Animation(4, 32, 32, 0, 32, explosionAnimationsFramesPerSecond),
                new Animation(4, 32, 32, 0, 64, explosionAnimationsFramesPerSecond),
                new Animation(4, 32, 32, 0, 96, explosionAnimationsFramesPerSecond),
                new Animation(4, 32, 32, 0, 128, explosionAnimationsFramesPerSecond),
                new Animation(4, 32, 32, 0, 160, explosionAnimationsFramesPerSecond),
                new Animation(4, 32, 32, 0, 192, explosionAnimationsFramesPerSecond)
            };
            explosionAnimationsDirection = new Dictionary<Point, ExplosionDirection>();

            // Sound Effect
            this.explosionSound = gameRef.GamePlayScreen.BombExplosionSound;

            // Bomb's timer
            timer = TimeSpan.Zero;
            this.timerLenght = timerLenght;

            // Bomb's states
            this.inDestruction = false;
            this.isAlive = true;
            this.willExplode = false;
            this.power = pow;
            this.lookDirection = LookDirection.Idle;
            this.previousCellPosition = Sprite.CellPosition;
            this.cellChanging = false;
            this.cellTeleporting = false;

            this.lastPlayerThatPushIt = -1;

            // Action field
            this.actionField = new List<Point>();
            ComputeActionField(1);
        }
        #endregion

        #region XNA Method

        public override void Update(GameTime gameTime)
        {
            Sprite.Update(gameTime);

            #region Is moving ?
            if (Sprite.CellPosition != previousCellPosition)
            {
                cellChanging = true;

                if (gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].
                    Map[previousCellPosition.X, previousCellPosition.Y] == this)
                {
                    gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].
                        Map[previousCellPosition.X, previousCellPosition.Y] = null;
                }

                if (gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].
                    Map[Sprite.CellPosition.X, Sprite.CellPosition.Y] == null)
                {
                    gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].
                    CollisionLayer[Sprite.CellPosition.X, Sprite.CellPosition.Y] = true;
                    
                    gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].
                    Map[Sprite.CellPosition.X, Sprite.CellPosition.Y] = this;
                }

                gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].
                    CollisionLayer[previousCellPosition.X, previousCellPosition.Y] = false;

                if (cellTeleporting)
                    cellTeleporting = false;

                this.Sprite.Position = Engine.CellToVector(this.Sprite.CellPosition);
            }
            else
                cellChanging = false;
            #endregion

            if (timer >= timerLenght)
            {
                timer = TimeSpan.FromSeconds(-1);
                Destroy();
            }
            else if (timer >= TimeSpan.Zero)
            {
                timer += gameTime.ElapsedGameTime;

                // The bomb will explode soon
                if (lookDirection == LookDirection.Idle && 
                    !willExplode && timerLenght.TotalSeconds - timer.TotalSeconds < 1)
                {
                    ComputeActionField(2);
                    willExplode = true;
                }
            }

            if (inDestruction)
            {
                foreach (Animation a in explosionAnimations)
                    a.Update(gameTime);
                if (explosionAnimations[4].CurrentFrame == explosionAnimations[4].FrameCount - 1)
                    Remove();
            }

            #region When the bomb moves

            // Control
            if (InputHandler.KeyDown(Keys.NumPad8))
                lookDirection = LookDirection.Up;
            else if (InputHandler.KeyDown(Keys.NumPad5))
                lookDirection = LookDirection.Down;
            else if (InputHandler.KeyDown(Keys.NumPad4))
                lookDirection = LookDirection.Left;
            else if (InputHandler.KeyDown(Keys.NumPad6))
                lookDirection = LookDirection.Right;

            if (lookDirection != LookDirection.Idle)
            {
                Level level = gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel];
                switch (lookDirection)
                {
                    case LookDirection.Up:
                        if (Sprite.Position.Y > Engine.TileHeight)
                            Sprite.PositionY = Sprite.Position.Y - Sprite.Speed;
                        else
                            lookDirection = LookDirection.Idle;
                        
                        Point upCell = new Point(this.Sprite.CellPosition.X, this.Sprite.CellPosition.Y - 1);
                        if (level.Map[upCell.X, upCell.Y] is Player || WallAt(upCell))
                        {
                            // Top collision
                            if (lookDirection == LookDirection.Up && MoreTopSide())
                            {
                                this.Sprite.PositionY = this.Sprite.CellPosition.Y * Engine.TileHeight;
                                lookDirection = LookDirection.Idle;
                            }
                        }
                        
                        break;
                    case LookDirection.Down:
                        if (Sprite.Position.Y < (level.Size.Y - 2) * Engine.TileHeight)
                            Sprite.PositionY = Sprite.Position.Y + Sprite.Speed;
                        else
                            lookDirection = LookDirection.Idle;
                        
                        Point bottomCell = new Point(this.Sprite.CellPosition.X, this.Sprite.CellPosition.Y + 1);
                        if (level.Map[bottomCell.X, bottomCell.Y] is Player || WallAt(bottomCell))
                        {
                            // Bottom collision
                            if (lookDirection == LookDirection.Down && MoreBottomSide())
                            {
                                this.Sprite.PositionY = this.Sprite.CellPosition.Y * Engine.TileHeight;
                                lookDirection = LookDirection.Idle;
                            }
                        }
                        
                        break;
                    case LookDirection.Left:
                        if (Sprite.Position.X > Engine.TileWidth)
                            Sprite.PositionX = Sprite.Position.X - Sprite.Speed;
                        else
                            lookDirection = LookDirection.Idle;
                        
                        Point leftCell = new Point(this.Sprite.CellPosition.X - 1, this.Sprite.CellPosition.Y);
                        if (level.Map[leftCell.X, leftCell.Y] is Player || WallAt(leftCell))
                        {
                            // Left collision
                            if (this.lookDirection == LookDirection.Left && MoreLeftSide())
                            {
                                this.Sprite.PositionX = this.Sprite.CellPosition.X * Engine.TileWidth - Engine.TileWidth / 2 + Engine.TileWidth / 2;
                                lookDirection = LookDirection.Idle;
                            }
                         }
                        break;
                    case LookDirection.Right:
                        if (Sprite.Position.X < (level.Size.X - 2) * Engine.TileWidth)
                            Sprite.PositionX = Sprite.Position.X + Sprite.Speed;
                        else
                            lookDirection = LookDirection.Idle;
                        
                        Point rightCell = new Point(this.Sprite.CellPosition.X + 1, this.Sprite.CellPosition.Y);
                        if (level.Map[rightCell.X, rightCell.Y] is Player || WallAt(rightCell))
                        {
                            // Right collision
                            if (this.lookDirection == LookDirection.Right && MoreRightSide())
                            {
                                this.Sprite.PositionX = this.Sprite.CellPosition.X * Engine.TileWidth - Engine.TileWidth / 2 + Engine.TileWidth / 2;
                                lookDirection = LookDirection.Idle;
                            }
                         }
                        break;
                }
                if (lookDirection == LookDirection.Idle)
                    this.Sprite.Position = Engine.CellToVector(this.Sprite.CellPosition);
            }
            #endregion

            #region Teleporter

            if (!cellTeleporting && gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].
                Map[Sprite.CellPosition.X, Sprite.CellPosition.Y] is Teleporter)
            {
                Teleporter teleporter = (Teleporter)(gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].
                    Map[Sprite.CellPosition.X, Sprite.CellPosition.Y]);

                teleporter.ChangePosition(this);
                cellTeleporting = true;
            }

            #endregion

            #region Arrow

            if (!cellTeleporting && gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].
                Map[Sprite.CellPosition.X, Sprite.CellPosition.Y] is Arrow)
            {
                Arrow arrow = (Arrow)(gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].
                    Map[Sprite.CellPosition.X, Sprite.CellPosition.Y]);

                arrow.ChangeDirection(this);
            }

            previousCellPosition = Sprite.CellPosition;
            #endregion

        }

        public override void Draw(GameTime gameTime)
        {
            if (inDestruction)
            {
                Level level = gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel];

                if (explosionAnimationsDirection.Count == 0)
                {
                    foreach (Point p in actionField)
                    {
                        // Is this a wall ? => we don't like wall !
                        if (!level.CollisionLayer[p.X, p.Y] || p == this.Sprite.CellPosition)
                        {
                            // We choose the sprite of explosion for each cell
                            explosionAnimationsDirection[p] = ComputeExplosionSpriteDirections(p, level);
                        }
                    }
                }
                else
                {
                    foreach (Point p in explosionAnimationsDirection.Keys)
                    {
                        gameRef.SpriteBatch.Draw(explosionSpriteTexture, new Vector2(Engine.Origin.X + p.X * Engine.TileWidth, Engine.Origin.Y + p.Y * Engine.TileHeight),
                            explosionAnimations[(int) explosionAnimationsDirection[p]].CurrentFrameRect, Color.White);
                    }
                }
            }
            else
                Sprite.Draw(gameTime, gameRef.SpriteBatch);
        }

        #endregion

        #region Method Region

        #region Private Method Region

        private bool WallAt(Point cell)
        {
            if (cell.X >= 0 && cell.Y >= 0 && cell.X < gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].Size.X &&
                cell.Y < gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].Size.Y)
                return (this.gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].CollisionLayer[cell.X, cell.Y]);
            else
                return false;
        }

        private bool MoreTopSide()
        {
            return this.Sprite.Position.Y < ((this.Sprite.CellPosition.Y * Engine.TileHeight) - (this.Sprite.Speed / 2));
        }

        private bool MoreBottomSide()
        {
            return this.Sprite.Position.Y > ((Sprite.CellPosition.Y * Engine.TileHeight) + (Sprite.Speed / 2));
        }

        private bool MoreLeftSide()
        {
            return this.Sprite.Position.X < ((this.Sprite.CellPosition.X * Engine.TileWidth) - (this.Sprite.Speed / 2));
        }

        private bool MoreRightSide()
        {
            return this.Sprite.Position.X > ((this.Sprite.CellPosition.X * Engine.TileWidth) + (this.Sprite.Speed / 2));
        }

        // Compute bomb's effect field
        private void ComputeActionField(int dangerType)
        {
            // Reset the actionField list
            this.actionField = new List<Point>();

            // We put the bomb in its action field
            this.actionField.Add(new Point(this.Sprite.CellPosition.X, this.Sprite.CellPosition.Y));

            if (gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].
                        HazardMap[this.Sprite.CellPosition.X, this.Sprite.CellPosition.Y] < dangerType)
            {
                gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].
                HazardMap[this.Sprite.CellPosition.X, this.Sprite.CellPosition.Y] = dangerType;
            }

            // 0 => Top, 1 => Bottom, 2 => Left, 3 => Right
            Dictionary<String, bool> obstacles = new Dictionary<String, bool> 
            { 
                {"up", false}, 
                {"down", false}, 
                {"right",false}, 
                {"left", false}
            };

            int tempPower = this.power - 1;
            Level level = gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel];
            Point addPosition = Point.Zero;
            while (tempPower >= 0)
            {
                // Directions
                int up = this.Sprite.CellPosition.Y - (this.power - tempPower);
                int down = this.Sprite.CellPosition.Y + (this.power - tempPower);
                int right = this.Sprite.CellPosition.X + (this.power - tempPower);
                int left = this.Sprite.CellPosition.X - (this.power - tempPower);

                // Up
                if (up >= 0 && !obstacles["up"])
                {
                    if (level.CollisionLayer[this.Sprite.CellPosition.X, up])
                        obstacles["up"] = true;
                    // We don't count the outside walls
                    if (!(level.Map[this.Sprite.CellPosition.X, up] is EdgeWall))
                    {
                        addPosition = new Point(this.Sprite.CellPosition.X, up);
                        this.actionField.Add(addPosition);
                        if (gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].
                        HazardMap[addPosition.X, addPosition.Y] < dangerType)
                        {
                            gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].
                            HazardMap[addPosition.X, addPosition.Y] = dangerType;
                        }
                    }
                }

                // Down
                if (down < level.Size.Y - 1 && !obstacles["down"])
                {
                    if (level.CollisionLayer[this.Sprite.CellPosition.X, down])
                        obstacles["down"] = true;
                    // We don't count the outside walls
                    if (!(level.Map[this.Sprite.CellPosition.X, down] is EdgeWall))
                    {
                        addPosition = new Point(this.Sprite.CellPosition.X, down);
                        this.actionField.Add(addPosition);
                        if (gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].
                        HazardMap[addPosition.X, addPosition.Y] < dangerType)
                        {
                            gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].
                            HazardMap[addPosition.X, addPosition.Y] = dangerType;
                        }
                    }
                }

                // Right
                if (right < level.Size.X - 1 && !obstacles["right"])
                {
                    if (level.CollisionLayer[right, this.Sprite.CellPosition.Y])
                        obstacles["right"] = true;
                    // We don't count the outside walls
                    if (!(level.Map[right, this.Sprite.CellPosition.Y] is EdgeWall))
                    {
                        addPosition = new Point(right, this.Sprite.CellPosition.Y);
                        this.actionField.Add(addPosition);
                        if (gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].
                        HazardMap[addPosition.X, addPosition.Y] < dangerType)
                        {
                            gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].
                            HazardMap[addPosition.X, addPosition.Y] = dangerType;
                        }
                    }
                }

                // Left
                if (left >= 0 && !obstacles["left"])
                {
                    if (level.CollisionLayer[left, this.Sprite.CellPosition.Y])
                        obstacles["left"] = true;
                    // We don't count the outside walls
                    if (!(level.Map[left, this.Sprite.CellPosition.Y] is EdgeWall))
                    {
                        addPosition = new Point(left, this.Sprite.CellPosition.Y);
                        this.actionField.Add(addPosition);
                        if (gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].
                        HazardMap[addPosition.X, addPosition.Y] < dangerType)
                        {
                            gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].
                            HazardMap[addPosition.X, addPosition.Y] = dangerType;
                        }
                    }
                }

                tempPower--;
            }
        }

        // 0 => Down, 1 => Left, 2 => Right, 3 => Up, 4 => Middle, 5 => Horizontal, 6 => Vertical 
        private ExplosionDirection ComputeExplosionSpriteDirections(Point cell, Level level)
        {
            int downCell = cell.Y + 1, leftCell = cell.X - 1, rightCell = cell.X + 1, upCell = cell.Y - 1;

            // ~ The middle ~ //
            if (cell.X == this.Sprite.CellPosition.X && cell.Y == this.Sprite.CellPosition.Y)
                return ExplosionDirection.MIDDLE;
 
            // ~ Vertical axis ~ //

            // Top extremity
            if (level.HazardMap[cell.X, upCell] == 0 &&
                (this.actionField.Find(c => c.X == cell.X && c.Y == downCell) != Point.Zero))
                return ExplosionDirection.UP;
            // Vertical
            else if ((this.actionField.Find(c => c.X == cell.X && c.Y == downCell) != Point.Zero) &&
                (this.actionField.Find(c => c.X == cell.X && c.Y == upCell) != Point.Zero))
                return ExplosionDirection.VERTICAL;
            // Bottom extremity
            else if (level.HazardMap[cell.X, downCell] == 0 &&
                (this.actionField.Find(c => c.X == cell.X && c.Y == upCell) != Point.Zero))
                return ExplosionDirection.DOWN;

            // ~ Horizontal axis ~ //

            // Left extremity
            else if (level.HazardMap[leftCell, cell.Y] == 0 &&
                (this.actionField.Find(c => c.X == rightCell && c.Y == cell.Y) != Point.Zero))
                return ExplosionDirection.LEFT;
            // Left - Right
            else if ((this.actionField.Find(c => c.X == rightCell && c.Y == cell.Y) != Point.Zero) &&
                (this.actionField.Find(c => c.X == leftCell && c.Y == cell.Y) != Point.Zero))
                return ExplosionDirection.HORIZONTAL;
            // Right extremity
            else if (level.HazardMap[rightCell, cell.Y] == 0 &&
                (this.actionField.Find(c => c.X == leftCell && c.Y == cell.Y) != Point.Zero))
                return ExplosionDirection.RIGHT;

            // ~ Corners ~ //

            // Corner Top - Left
            else if (cell.Y == 1 && cell.X == 1)
            {
                // Left extremity
                if (this.actionField.Find(c => c.X == rightCell && c.Y == cell.Y) != Point.Zero)
                    return ExplosionDirection.LEFT;
                // Top extremity
                else
                    return ExplosionDirection.UP;
            }
            // Corner Bottom - Left
            else if (cell.Y == level.Size.Y - 2 && cell.X == 1)
            {
                // Left extremity
                if (this.actionField.Find(c => c.X == rightCell && c.Y == cell.Y) != Point.Zero)
                    return ExplosionDirection.LEFT;
                // Bottom extremity
                else
                    return ExplosionDirection.DOWN;
            }

            // Corner Top - Right
            else if (cell.X == level.Size.X - 2 && cell.Y == 1)
            {
                // Right extremity
                if (this.actionField.Find(c => c.X == leftCell && c.Y == cell.Y) != Point.Zero)
                    return ExplosionDirection.RIGHT;
                // Top extremity
                else
                    return ExplosionDirection.UP;
            }
            // Corner Bottom - Right
            else if (cell.Y == level.Size.Y - 2 && cell.X == level.Size.X - 2)
            {
                // Right extremity
                if (this.actionField.Find(c => c.X == leftCell && c.Y == cell.Y) != Point.Zero)
                    return ExplosionDirection.RIGHT;
                // Bottom extremity
                else
                    return ExplosionDirection.DOWN;
            }
            // Error case
            else
                return ExplosionDirection.MIDDLE;

        }

        #endregion

        #region Public Method Region

        public void ChangeDirection(LookDirection lD, int playerId)
        {
            Point pos = Point.Zero;
            switch(lD)
            {
                case LookDirection.Up:
                    pos = new Point(Sprite.CellPosition.X, Sprite.CellPosition.Y - 1);
                    break;
                case LookDirection.Down:
                    pos = new Point(Sprite.CellPosition.X, Sprite.CellPosition.Y + 1);
                    break;
                case LookDirection.Left:
                    pos = new Point(Sprite.CellPosition.X - 1, Sprite.CellPosition.Y);
                    break;
                case LookDirection.Right:
                    pos = new Point(Sprite.CellPosition.X + 1, Sprite.CellPosition.Y);
                    break;
            }

            if (!gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].
                CollisionLayer[pos.X, pos.Y])
            {
                this.lookDirection = lD;
                this.lastPlayerThatPushIt = playerId;
                foreach (Point p in this.actionField)
                {
                    gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].
                        HazardMap[p.X, p.Y] = 0;
                }
            }
            else
                lookDirection = LookDirection.Idle;
        }

        public void ChangeSpeed(float changing)
        {
            this.Sprite.Speed = changing;
        }

        public void ResetTimer()
        {
            this.timer = TimeSpan.Zero;
        }

        public override void Destroy()
        {
            if (!this.inDestruction)
            {
                this.explosionSound.Play();
                this.inDestruction = true;
                ComputeActionField(3);
            }
        }

        public override void Remove()
        {
            this.isAlive = false;
        }
        #endregion

        #endregion
    }
}
