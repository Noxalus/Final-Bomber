using System;
using System.Collections.Generic;
using System.Diagnostics;
using FBLibrary.Core;
using Microsoft.Xna.Framework;

namespace Final_Bomber.Entities.AI
{
    public static class AIFunction
    {
        public static int[,] CostMatrix(Point origin, bool[,] collisionLayer, int[,] hazardMap, Point mapSize)
        {
            var costMatrix = new int[mapSize.X, mapSize.Y];

            // We put all cells at the infinity
            for (int x = 0; x < mapSize.X; x++)
                for (int y = 0; y < mapSize.Y; y++)
                    costMatrix[x, y] = mapSize.X * mapSize.Y;

            var pos = new Point();
            int id = 0;
            var queue = new Queue<Point>();

            costMatrix[origin.X, origin.Y] = id;
            queue.Enqueue(origin);

            while (queue.Count > 0)
            {
                id++;
                int counter = queue.Count;
                for (int i = 0; i < counter; i++)
                {
                    pos = queue.Dequeue();
                    // Haut
                    if (pos.Y - 1 >= 0 && !collisionLayer[pos.X, pos.Y - 1] && costMatrix[pos.X, pos.Y - 1] > id)
                    {
                        if (hazardMap[origin.X, origin.Y] > 0 || hazardMap[pos.X, pos.Y - 1] <= 1)
                        {
                            costMatrix[pos.X, pos.Y - 1] = id;
                            queue.Enqueue(new Point(pos.X, pos.Y - 1));
                        }
                    }
                    // Droite
                    if (pos.X + 1 < mapSize.X && !collisionLayer[pos.X + 1, pos.Y] && costMatrix[pos.X + 1, pos.Y] > id)
                    {
                        if (hazardMap[origin.X, origin.Y] > 0 || hazardMap[pos.X + 1, pos.Y] <= 1)
                        {
                            costMatrix[pos.X + 1, pos.Y] = id;
                            queue.Enqueue(new Point(pos.X + 1, pos.Y));
                        }
                    }
                    // Bas
                    if (pos.Y + 1 < mapSize.Y && !collisionLayer[pos.X, pos.Y + 1] && costMatrix[pos.X, pos.Y + 1] > id)
                    {
                        if (hazardMap[origin.X, origin.Y] > 0 || hazardMap[pos.X, pos.Y + 1] <= 1)
                        {
                            costMatrix[pos.X, pos.Y + 1] = id;
                            queue.Enqueue(new Point(pos.X, pos.Y + 1));
                        }
                    }
                    // Gauche
                    if (pos.X - 1 >= 0 && !collisionLayer[pos.X - 1, pos.Y] && costMatrix[pos.X - 1, pos.Y] > id)
                    {
                        if (hazardMap[origin.X, origin.Y] > 0 || hazardMap[pos.X - 1, pos.Y] <= 1)
                        {
                            costMatrix[pos.X - 1, pos.Y] = id;
                            queue.Enqueue(new Point(pos.X - 1, pos.Y));
                        }
                    }
                }
            }
            return costMatrix;
        }

        public static List<Point> MakeAWay(Point origin, Point goal, bool[,] collisionLayer, int[,] hazardMap, Point mapSize)
        {
            int[,] costMatrix = CostMatrix(origin, collisionLayer, hazardMap, mapSize);
            if (goal.X == -1 && goal.Y == -1)
            {
                return null;
            }
            // We can't reach the goal
            if (costMatrix[goal.X, goal.Y] == mapSize.X * mapSize.Y)
            {
                //throw new Exception("Cette case ne peut pas être atteinte !");
                return null;
            }
            else
            {
                var path = new List<Point> {goal};
                int min = mapSize.X * mapSize.Y;
                var lookDirection = LookDirection.Idle;
                while (origin != goal && path.Count < 100)
                {
                    min = mapSize.X * mapSize.Y;
                    // Up
                    if (costMatrix[goal.X, goal.Y - 1] < min)
                    {
                        min = costMatrix[goal.X, goal.Y - 1];
                        lookDirection = LookDirection.Up;
                    }
                    // Down
                    if (costMatrix[goal.X, goal.Y + 1] < min)
                    {
                        min = costMatrix[goal.X, goal.Y + 1];
                        lookDirection = LookDirection.Down;
                    }
                    // Right
                    if (costMatrix[goal.X + 1, goal.Y] < min)
                    {
                        min = costMatrix[goal.X + 1, goal.Y];
                        lookDirection = LookDirection.Right;
                    }
                    // Left
                    if (costMatrix[goal.X - 1, goal.Y] < min)
                    {
                        min = costMatrix[goal.X - 1, goal.Y];
                        lookDirection = LookDirection.Left;
                    }

                    switch (lookDirection)
                    {
                        case LookDirection.Up:
                            goal.Y--;
                            break;
                        case LookDirection.Down:
                            goal.Y++;
                            break;
                        case LookDirection.Right:
                            goal.X++;
                            break;
                        case LookDirection.Left:
                            goal.X--;
                            break;
                    }

                    if(goal != origin)
                        path.Add(goal);
                }
                return path;
            }
        }

        public static bool HasReachNextPosition(Vector2 position, float speed, Vector2 nextPosition)
        {
            return (position.X <= nextPosition.X + speed && position.X >= nextPosition.X - speed) &&
                    (position.Y <= nextPosition.Y + speed && position.Y >= nextPosition.Y - speed);
        }

        public static Point SetNewGoal(Point position, Entity[,] map, bool[,] collisionLayer, int[,] hazardMap, Point mapSize) 
        {
            if (hazardMap[position.X, position.Y] < 2)
                return SetNewOffenseGoal(position, map, collisionLayer, hazardMap, mapSize);
            else
                return SetNewDefenseGoal(position, collisionLayer, hazardMap, mapSize);
        }

        public static Point SetNewDefenseGoal(Point position, bool[,] collisionLayer, int[,] hazardMap, Point mapSize)
        {
            return NearestSafeCell(position, collisionLayer, hazardMap, 0, mapSize);
        }

        private static Point SetNewOffenseGoal(Point position, Entity[,] map, bool[,] collisionLayer, int[,] hazardMap, Point mapSize)
        {
            /*
            int[,] costMatrix = CostMatrix(position, collisionLayer, hazardMap);
            Point cell = new Point(-1, -1);
            int max = 0;
            for (int x = 0; x < costMatrix.GetLength(0); x++)
            {
                for (int y = 0; y < costMatrix.GetLength(1); y++)
                {
                    if (hazardMap[x, y] == 0)
                    {
                        if (costMatrix[x, y] != mapSize.X * mapSize.Y && costMatrix[x, y] > max)
                        {
                            max = costMatrix[x, y];
                            cell.X = x;
                            cell.Y = y;
                        }
                    }
                }
            }
            return cell;
            */

            return BestCellInterest(position, map, collisionLayer, hazardMap, mapSize);
        }

        private static Point NearestSafeCell(Point position, bool[,] collisionLayer, int[,] hazardMap, int hazardLevel, Point mapSize)
        {
            var queue = new Queue<Point>();
            queue.Enqueue(position);

            int limit = 0;
            while (queue.Count > 0 && limit < 3)
            {
                int counter = queue.Count;
                for (int i = 0; i < counter; i++)
                {
                    Point pos = queue.Dequeue();
                    Point cell;

                    // Up
                    if (pos.Y - 1 >= 0 && !collisionLayer[pos.X, pos.Y - 1])
                    {
                        cell = new Point(pos.X, pos.Y - 1);
                        if (hazardMap[cell.X, cell.Y] <= hazardLevel)
                            return cell;
                        else
                            queue.Enqueue(cell);
                    }
                    // Down
                    if (pos.Y + 1 < mapSize.Y && !collisionLayer[pos.X, pos.Y + 1])
                    {
                        cell = new Point(pos.X, pos.Y + 1);
                        if (hazardMap[cell.X, cell.Y] <= hazardLevel)
                            return cell;
                        else
                            queue.Enqueue(cell);
                    }
                    // Right
                    if (pos.X + 1 < mapSize.X && !collisionLayer[pos.X + 1, pos.Y])
                    {
                        cell = new Point(pos.X + 1, pos.Y);
                        if (hazardMap[cell.X, cell.Y] <= hazardLevel)
                            return cell;
                        else
                            queue.Enqueue(cell);
                    }
                    // Left
                    if (pos.X - 1 >= 0 && !collisionLayer[pos.X - 1, pos.Y])
                    {
                        cell = new Point(pos.X - 1, pos.Y);
                        if (hazardMap[cell.X, cell.Y] <= hazardLevel)
                            return cell;
                        else
                            queue.Enqueue(cell);
                    }
                }
                limit++;
            }
            return new Point(-1, -1);
        }

        private static bool EntityNear(Point position, int power, Entity[,] map, int[,] hazardMap, Point mapSize)
        {
            power = 1; // We have to compute the bomb action field !
            // Up
            if (position.Y - power > 0 && (map[position.X, position.Y - power] is Wall || map[position.X, position.Y - power] is Player))
                return true;
            // Down
            else if (position.Y + power < mapSize.Y - 2 && (map[position.X, position.Y + power] is Wall || map[position.X, position.Y + power] is Player))
                return true;
            // Right
            else if (position.X + power < mapSize.X - 2 && (map[position.X + power, position.Y] is Wall || map[position.X + power, position.Y] is Player))
                return true;
            // Left
            else if (position.X - power > 0 && (map[position.X - power, position.Y] is Wall || map[position.X - power, position.Y] is Player))
                return true;

            return false;
        }

        private static int ProximityItemNumber(Point position, int power, Entity[,] map, int[,] hazardMap, Point mapSize)
        {
            power = 1; // We have to compute the bomb action field !
            int number = 0;
            // Up
            if (position.Y - power > 0)
            {
                if(map[position.X, position.Y - power] is PowerUp)
                    number++;
                else if (map[position.X, position.Y - power] is Wall)
                {
                    var w = map[position.X, position.Y - power] as Wall;
                    Debug.Assert(w != null, "w != null");
                    if (hazardMap[position.X, position.Y - power] > 0 || w.InDestruction)
                        number++;
                }
            }
            // Down
            if (position.Y + power < mapSize.Y - 2)
            {
                if(map[position.X, position.Y + power] is PowerUp)
                    number++;
                else if (map[position.X, position.Y + power] is Wall)
                {
                    var w = map[position.X, position.Y + power] as Wall;
                    Debug.Assert(w != null, "w != null");
                    if (hazardMap[position.X, position.Y + power] > 0 || w.InDestruction)
                        number++;
                }
            }
            // Right
            if (position.X + power < mapSize.X - 2)
            {
                if (map[position.X + power, position.Y] is PowerUp)
                    number++;
                else if (map[position.X + power, position.Y] is Wall)
                {
                    var w = map[position.X + power, position.Y] as Wall;
                    Debug.Assert(w != null, "w != null");
                    if (hazardMap[position.X + power, position.Y] > 0 || w.InDestruction)
                        number++;
                }
            }
            // Left
            if (position.X - power > 0)
            {
                if (map[position.X - power, position.Y] is PowerUp)
                    number++;
                else if (map[position.X - power, position.Y] is Wall)
                {
                    var w = map[position.X - power, position.Y] as Wall;
                    Debug.Assert(w != null, "w != null");
                    if (hazardMap[position.X - power, position.Y] > 0 || w.InDestruction)
                        number++;
                }
            }

            return number;
        }

        public static bool TryToPutBomb(Point position, int power, Entity[,] map, bool[,] collisionLayer, int[,] hM, Point mapSize)
        {
            if (ProximityItemNumber(position, power, map, hM, mapSize) == 0)
            {
                if (EntityNear(position, power, map, hM, mapSize))
                {
                    #region Compute the bomb's action field
                    const int dangerType = 1;

                    // We put the bomb in its action field
                    var actionField = new List<Point> {new Point(position.X, position.Y)};

                    var hazardMap = new int[mapSize.X, mapSize.Y];

                    for (int x = 0; x < mapSize.X; x++)
                        for (int y = 0; y < mapSize.Y; y++)
                            hazardMap[x, y] = hM[x, y];

                    if (hazardMap[position.X, position.Y] < dangerType)
                        hazardMap[position.X, position.Y] = dangerType;

                    // 0 => Top, 1 => Bottom, 2 => Left, 3 => Right
                    var obstacles = new Dictionary<String, bool> 
                    { 
                        {"up", false}, 
                        {"down", false}, 
                        {"right",false}, 
                        {"left", false}
                    };

                    int tempPower = power - 1;
                    Point addPosition;
                    while (tempPower >= 0)
                    {
                        // Directions
                        int up = position.Y - (power - tempPower);
                        int down = position.Y + (power - tempPower);
                        int right = position.X + (power - tempPower);
                        int left = position.X - (power - tempPower);

                        // Up
                        if (up >= 0 && !obstacles["up"])
                        {
                            if (collisionLayer[position.X, up])
                                obstacles["up"] = true;
                            // We don't count the outside walls
                            if (!(map[position.X, up] is EdgeWall))
                            {
                                addPosition = new Point(position.X, up);
                                actionField.Add(addPosition);
                                if (hazardMap[addPosition.X, addPosition.Y] < dangerType)
                                    hazardMap[addPosition.X, addPosition.Y] = dangerType;
                            }
                        }

                        // Down
                        if (down < mapSize.Y - 1 && !obstacles["down"])
                        {
                            if (collisionLayer[position.X, down])
                                obstacles["down"] = true;
                            // We don't count the outside walls
                            if (!(map[position.X, down] is EdgeWall))
                            {
                                addPosition = new Point(position.X, down);
                                actionField.Add(addPosition);
                                if (hazardMap[addPosition.X, addPosition.Y] < dangerType)
                                    hazardMap[addPosition.X, addPosition.Y] = dangerType;
                            }
                        }

                        // Right
                        if (right < mapSize.X - 1 && !obstacles["right"])
                        {
                            if (collisionLayer[right, position.Y])
                                obstacles["right"] = true;
                            // We don't count the outside walls
                            if (!(map[right, position.Y] is EdgeWall))
                            {
                                addPosition = new Point(right, position.Y);
                                actionField.Add(addPosition);
                                if (hazardMap[addPosition.X, addPosition.Y] < dangerType)
                                    hazardMap[addPosition.X, addPosition.Y] = dangerType;
                            }
                        }

                        // Left
                        if (left >= 0 && !obstacles["left"])
                        {
                            if (collisionLayer[left, position.Y])
                                obstacles["left"] = true;
                            // We don't count the outside walls
                            if (!(map[left, position.Y] is EdgeWall))
                            {
                                addPosition = new Point(left, position.Y);
                                actionField.Add(addPosition);
                                if (hazardMap[addPosition.X, addPosition.Y] < dangerType)
                                    hazardMap[addPosition.X, addPosition.Y] = dangerType;
                            }
                        }

                        tempPower--;
                    }
                    #endregion

                    // Watch if NearestSafeCell returns a Point(-1, -1)
                    if (NearestSafeCell(position, collisionLayer, hazardMap, 0, mapSize) == new Point(-1, -1))
                        return false;
                    else
                        return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public static int[,] MakeInterestMatrix(Point position, Entity[,] map, bool[,] collisionLayer, int[,] hazardMap, Point mapSize)
        {
            var interestMatrix = new int[mapSize.X, mapSize.Y];
            interestMatrix[position.X, position.Y] = -1;
            int id = 0;
            var queue = new Queue<Point>();
            queue.Enqueue(position);

            while (queue.Count > 0)
            {
                int counter = queue.Count;
                for (int i = 0; i < counter; i++)
                {
                    Point pos = queue.Dequeue();
                    // Up
                    Point cell;
                    if (pos.Y - 1 >= 0 && !collisionLayer[pos.X, pos.Y - 1] && interestMatrix[pos.X, pos.Y - 1] == 0)
                    {
                        cell = new Point(pos.X, pos.Y - 1);
                        if (hazardMap[cell.X, cell.Y] <= 1)
                        {
                            interestMatrix[cell.X, cell.Y] = CellInterest(cell, id, map, hazardMap, mapSize);
                            queue.Enqueue(cell);
                        }
                    }
                    // Droite
                    if (pos.X + 1 < mapSize.X && !collisionLayer[pos.X + 1, pos.Y] && interestMatrix[pos.X + 1, pos.Y] == 0)
                    {
                        cell = new Point(pos.X + 1, pos.Y);
                        if (hazardMap[cell.X, cell.Y] <= 1)
                        {
                            interestMatrix[cell.X, cell.Y] = CellInterest(cell, id, map, hazardMap, mapSize);
                            queue.Enqueue(cell);
                        }
                    }
                    // Bas
                    if (pos.Y + 1 < mapSize.Y && !collisionLayer[pos.X, pos.Y + 1] && interestMatrix[pos.X, pos.Y + 1] == 0)
                    {
                        cell = new Point(pos.X, pos.Y + 1);
                        if (hazardMap[cell.X, cell.Y] <= 1)
                        {
                            interestMatrix[cell.X, cell.Y] = CellInterest(cell, id, map, hazardMap, mapSize);
                            queue.Enqueue(cell);
                        }
                    }
                    // Gauche
                    if (pos.X - 1 >= 0 && !collisionLayer[pos.X - 1, pos.Y] && interestMatrix[pos.X - 1, pos.Y] == 0)
                    {
                        cell = new Point(pos.X - 1, pos.Y);
                        if (hazardMap[cell.X, cell.Y] <= 1)
                        {
                            interestMatrix[cell.X, cell.Y] = CellInterest(cell, id, map, hazardMap, mapSize);
                            queue.Enqueue(cell);
                        }
                    }
                }
                id++;
            }
            return interestMatrix;
        }

        private static int CellInterest(Point position, int id, Entity[,] map, int[,] hazardMap, Point mapSize)
        {
            int interest = 0;
            if (map[position.X, position.Y] == null)
            {
                int proximityWallNumber = ProximityWallNumber(position, map);
                if (proximityWallNumber > 0)
                {
                    interest = (int)MathHelper.Clamp(
                        (float)(((mapSize.X * mapSize.Y) / 2) - id + proximityWallNumber),
                        (float)proximityWallNumber,
                        (float)((mapSize.X * mapSize.Y) / 2 + 4));
                }
                else
                    interest = (int)MathHelper.Clamp((float)id, 1f, (float)((mapSize.X * mapSize.Y) - id - 10 - 1));
            }
            else if (map[position.X, position.Y] is PowerUp)
                interest = (mapSize.X * mapSize.Y) - id;
            else if (map[position.X, position.Y] is Player)
            {
                interest = (mapSize.X * mapSize.Y) - id - 10;
            }
            return interest;
        }

        private static int ProximityWallNumber(Point position, Entity[,] map)
        {
            int wallNumber = 0;
            // Up
            if (map[position.X, position.Y - 1] is Wall)
                wallNumber++;
            // Down
            if (map[position.X, position.Y + 1] is Wall)
                wallNumber++;
            // Right
            if (map[position.X + 1, position.Y] is Wall)
                wallNumber++;
            // Left
            if (map[position.X - 1, position.Y] is Wall)
                wallNumber++;
            return wallNumber;
        }

        private static Point BestCellInterest(Point position, Entity[,] map, bool[,] collisionLayer, int[,] hazardMap, Point mapSize)
        {
            int[,] interestMatrix = MakeInterestMatrix(position, map, collisionLayer, hazardMap, mapSize);
            var cell = new Point(-1, -1);
            int max = 0;
            for (int x = 0; x < interestMatrix.GetLength(0); x++)
            {
                for (int y = 0; y < interestMatrix.GetLength(1); y++)
                {
                    if (interestMatrix[x, y] > max)
                    {
                        max = interestMatrix[x, y];
                        cell.X = x;
                        cell.Y = y;
                    }
                }
            }
            return cell;
        }
    }
}
