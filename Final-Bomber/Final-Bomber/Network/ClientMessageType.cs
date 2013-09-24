using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Final_Bomber.Network
{
    partial class GameServer
    {
        public enum RMT
        {
            GameStartInfo = 0, //skickar namnet på mapen och PlayerID
            Map = 1, //skickar mapen
            StartGame = 9, //Säger åt clienterna att det är dags o starta
            PlayerInfo = 3, //Skickar en spelares information till de andra spelarna, används tex när man lägger dit nya players
            PlayerPosAndSpeed = 2, //skickar positionen och speeden på varje spelare
            RemovePlayer = 4, //Tar bort en disconnected spelare
            PlayerPlacingBomb = 5, //Säger att en spelare har placerat en bomb
            BombExploded = 6, //Säger att en bomb har exploderat
            Burn = 7, //Säger att spelaren blev bränd på en bomb
            ExplodeTile = 8, //Säger till spelarna att en tile har exploderat
            PowerupDrop = 11, //Säger till spelarna att en powerup har droppats
            PowerupPick = 12, //Säger till spelarna att en powerup har plockats upp
            SuddenDeath = 13, //Säger till spelarna när sudden death börjar
            SDExplosion = 14, //Säger till spelarna vart sudden death explosionen sker
            End = 10, //Säger till att banan är slut och berättar vem som vann
            RoundEnd = 15,
        }

        public enum SMT
        {
            NeedMap = 6, //Spelaren behöver banan
            Ready = 7, //Spelaren är färdig för att starta
            MoveLeft = 0,
            MoveUp = 1,
            MoveRight = 2,
            MoveDown = 3,
            Standing = 4,
            PlaceBomb = 5,
        }
    }
}
