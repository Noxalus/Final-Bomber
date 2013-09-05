using Final_BomberServer.Core;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_BomberServer.Host
{
    partial class GameServer
    {
        public void SendGameInfo(Client client) // skickar kartans namn till clienterna
        {
            try
            {
                if (client.ClientConnection.Status == NetConnectionStatus.Connected)
                {
                    NetOutgoingMessage send = server.CreateMessage();
                    send.Write((byte)SMT.GameStartInfo);
                    send.Write(GameSettings.GetCurrentMap().id);
                    Console.WriteLine("\nSended game info map id " + GameSettings.GetCurrentMap().id.ToString());
                    server.SendMessage(send, client.ClientConnection, NetDeliveryMethod.ReliableOrdered);
                }
            }
            catch (NetException e)
            {
                WriteOutput("EXCEPTION: " + e.ToString());
            }
        }

        public void SendCurrentMap(Client client)
        {
            if (client.ClientConnection.Status == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage send = server.CreateMessage();
                send.Write((byte)SMT.Map);
                send.Write(GameSettings.GetCurrentMap().mapName);
                send.Write(GameSettings.GetCurrentMap().mapData.Count);
                foreach (byte bt in GameSettings.GetCurrentMap().mapData)
                {
                    send.Write(bt);
                }
                server.SendMessage(send, client.ClientConnection, NetDeliveryMethod.ReliableOrdered);
            }
        }

        public void SendStartGame(Client client, bool GameinProgress)
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMT.StartGame);
            send.Write(GameinProgress);
            if (!GameinProgress)
            {
                send.Write(client.Player.PlayerId);
                send.Write(client.Player.MoveSpeed);
                send.Write(GameSettings.GetCurrentMap().suddenDeathTime);
            }
            server.SendMessage(send, client.ClientConnection, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendPlayersToNew(Client client) //Skicka alla spelare till denna
        {
            foreach (Client player in clients)
            {
                if (client != player)
                {
                    NetOutgoingMessage send = GetPlayerInfo(player);
                    server.SendMessage(send, client.ClientConnection, NetDeliveryMethod.ReliableOrdered);
                    SendPlayerPosition(player.Player, false);
                }
            }
        }

        public void SendPlayerInfo(Client client) //Skicka denna spelare till alla andra som finns
        {
            if (client.ClientConnection.Status == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage send = GetPlayerInfo(client);
                server.SendToAll(send, client.ClientConnection, NetDeliveryMethod.ReliableOrdered, 0);
                SendPlayerPosition(client.Player, false);
            }
        }

        public void SendRemovePlayer(Player removePlayer)
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMT.RemovePlayer);
            send.Write(removePlayer.PlayerId);
            server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);
        }

        #region GetPlayerInfo
        private NetOutgoingMessage GetPlayerInfo(Client client)
        {
            NetOutgoingMessage rtn = server.CreateMessage();
            rtn.Write((byte)SMT.PlayerInfo);
            rtn.Write(client.Player.PlayerId);
            rtn.Write(client.Player.MoveSpeed);
            rtn.Write(client.Username);
            return rtn;
        }
        #endregion

        public void SendPlayerPosition(Player player, bool notDir)//Skicka spelarens rörelse till alla andra spelare
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMT.PlayerPosAndSpeed);
            send.Write(player.Position.X);
            send.Write(player.Position.Y);
            if (!notDir)
            {
                send.Write((byte)player.nextDirection);
            }
            else
            {
                send.Write((byte)255);
            }
            send.Write(player.PlayerId);
            server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);
            //GameSettings.rtxt_debug.Text += "\nSend Position - Player id; " + player.PlayerId.ToString();
        }

        public void SendPlayerPlacingBomb(Player player, float xPos, float yPos)//Skickar till alla spelare att denna spelare har placerat en bomb
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMT.PlayerPlacingBomb);
            send.Write(player.PlayerId);
            send.Write(xPos);
            send.Write(yPos);
            server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendBombExploded(Bomb bomb)//Skickar till alla spelare att bomben på denna position har exploderat och hur den exploderar
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMT.BombExploded);
            send.Write(bomb.Position.GetMapPos().X);
            send.Write(bomb.Position.GetMapPos().Y);
            send.Write(bomb.Explosion.Count);
            foreach (Explosion ex in bomb.Explosion)
            {
                send.Write(ex.Position.GetMapPos().X);
                send.Write(ex.Position.GetMapPos().Y);
                send.Write((byte)ex.explosionType);
            }
            server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendPlayerGotBurned(Player player)
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMT.Burn);
            send.Write(player.PlayerId);
            server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendExplodeTile(int tilePos)
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMT.ExplodeTile);
            send.Write(tilePos);
            server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendPowerupDrop(MapTile tile)
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMT.PowerupDrop);
            send.Write((byte)tile.Poweruped.Type);
            Vector2 pos = tile.GetMapPos();
            send.Write(pos.X);
            send.Write(pos.Y);
            server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendPowerupPick(Player player, MapTile tile)
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMT.PowerupPick);
            Vector2 pos = tile.GetMapPos();
            send.Write(pos.X);
            send.Write(pos.Y);
            send.Write(player.PlayerId);
            send.Write(tile.Poweruped.GetPowerupValue());
            server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendSuddenDeath()
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMT.SuddenDeath);
            server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);
            WriteOutput("SUDDEN DEATH!");
        }

        public void SendSDExplosion(Explosion ex)
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMT.SDExplosion);
            send.Write(ex.Position.GetListPos());
            server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendEnd(Client client)
        {
            if (client.ClientConnection.Status == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage send = server.CreateMessage();
                send.Write((byte)SMT.End);
                send.Write(!client.Player.IsDead);
                server.SendMessage(send, client.ClientConnection, NetDeliveryMethod.ReliableOrdered);
            }
        }
    }
}
