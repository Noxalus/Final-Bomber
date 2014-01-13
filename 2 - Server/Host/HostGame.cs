using FBServer.Core;
using FBServer.Core.Entities;
using System;

namespace FBServer.Host
{
    public class HostGame
    {
        int _playerId = 0;
        readonly Timer _beginGameTimer;

        public HostGame()
        {
            _beginGameTimer = new Timer(false);
        }

        public void Initialize()
        {
            GameServer.Instance.ConnectedClient += GameServer_ConnectedClient;
            GameServer.Instance.DisconnectedClient += GameServer_DisconnectedClient;
            GameServer.Instance.BombPlacing += GameServer_BombPlacing;
            GameServer.Instance.StartServer();
            
            _beginGameTimer.Start();
        }

        public void Update()
        {
            GameServer.Instance.Update();

            if (GameServer.Instance.GameManager.GameInitialized && _beginGameTimer != null)
            {
                if (!_beginGameTimer.IsStarted)
                    _beginGameTimer.Start();

                // Game really starts after 3 seconds
                if (!GameServer.Instance.GameManager.GameHasBegun && _beginGameTimer.Each(3000))
                {
                    _beginGameTimer.Stop();
                    GameServer.Instance.GameManager.GameHasBegun = true;
                }
            }
        }

        public void Dispose()
        {
            GameServer.Instance.EndServer("Ending game");
        }

        #region Server events

        private void GameServer_ConnectedClient(Client sender, EventArgs e)
        {
            if (true /*GameServer.Instance.clients.Count <= GameSettings.Get_gameManager.CurrentMap().playerAmount*/)
            {
                var player = new Player(_playerId);
                GameServer.Instance.GameManager.AddPlayer(sender, player);

                GameServer.Instance.SendGameInfo(sender);
                if (GameServer.Instance.GameManager.GameInitialized)
                {
                    sender.Player.IsAlive = false;
                    sender.Spectating = true;
                    sender.NewClient = true;
                }

                _playerId++;

                GameServer.Instance.SendClientInfo(sender);
                //MainServer.SendCurrentPlayerAmount();
            }
            else
            {
                GameServer.Instance.Clients.Remove(sender);
                Program.Log.Info("[FULLGAME] Client tried to connect");
                sender.ClientConnection.Disconnect("Full Server");
            }
        }

        private void GameServer_DisconnectedClient(Client sender, EventArgs e)
        {
            if (GameServer.Instance.GameManager.GameInitialized)
            {
                sender.Player.IsAlive = false;
                GameServer.Instance.SendRemovePlayer(sender.Player);
            }
            //MainServer.SendCurrentPlayerAmount();
        }

        // An evil player wants to plant a bomb
        private void GameServer_BombPlacing(Client sender)
        {
            if (GameServer.Instance.GameManager.GameHasBegun)
            {
                var player = sender.Player;

                if (player.CurrentBombAmount > 0)
                {
                    var bo = GameServer.Instance.GameManager.BombList.Find(b => b.CellPosition == player.CellPosition);

                    if (bo != null) return;

                    var bomb = new Bomb(player.Id, player.CellPosition, player.BombPower, player.BombTimer,
                        player.Speed);

                    bomb.Initialize(GameServer.Instance.GameManager.CurrentMap.Size, 
                                    GameServer.Instance.GameManager.CurrentMap.CollisionLayer, 
                                    GameServer.Instance.GameManager.HazardMap);

                    GameServer.Instance.GameManager.CurrentMap.Board[bomb.CellPosition.X, bomb.CellPosition.Y] = bomb;
                    GameServer.Instance.GameManager.CurrentMap.CollisionLayer[bomb.CellPosition.X, bomb.CellPosition.Y] = true;

                    GameServer.Instance.GameManager.AddBomb(bomb);
                    player.CurrentBombAmount--;

                    GameServer.Instance.SendPlayerPlacingBomb(sender.Player, bomb.CellPosition);
                }
            }
        }

        private void Bomb_IsExploded(Bomb bomb)
        {
            /*
            //_gameManager.CurrentMap.CalcBombExplosion(bomb);
            GameServer.Instance.SendBombExploded(bomb);
            bomb.Position.Bombed = null;

            //Kollar om bomben exploderar en annan bomb
            if (GameSettings.BombsExplodeBombs)
            {
                foreach (Explosion ex in bomb.Explosion)
                {
                    foreach (Bomb b in GameManager.BombList)
                    {
                        if (b.Position == ex.Position)
                        {
                            if (b.Exploded == false)
                                b.Explode();
                        }
                    }
                }
            }
            */
        }

        #endregion
    }
}
