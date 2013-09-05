using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Final_BomberNetwork.GameServer
{
    partial class Server
    {
        public void SendLoggedIn(NetConnection sender, byte status)
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMTClient.LoggedIn);
            send.Write(status);
            server.SendMessage(send, sender, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendCreatedAccount(NetConnection sender, int status)
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMTClient.CreatedAccount);
            send.Write(status);
            server.SendMessage(send, sender, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendHostedGames(NetConnection sender)
        {
            foreach (Client client in hoster)
            {
                NetOutgoingMessage send = server.CreateMessage();
                send.Write((byte)SMTClient.SendHostedGames);
                send.Write(client.ClientConnection.RemoteEndPoint.ToString().Split(new char[] { ':' })[0] + ":" + client.Port.ToString());
                send.Write(client.GameName);
                send.Write(client.CurrentMap);
                send.Write(client.ClientConnection.AverageRoundtripTime * 1000);
                send.Write(client.MaxPlayers);
                send.Write(client.CurrentPlayers);
                server.SendMessage(send, sender, NetDeliveryMethod.ReliableOrdered);
            }
            NetOutgoingMessage send2 = server.CreateMessage();
            send2.Write((byte)SMTClient.SendHostedGames);
            send2.Write("END");
            server.SendMessage(send2, sender, NetDeliveryMethod.ReliableOrdered);
        }

        private void CheckSettingsCorrect(NetOutgoingMessage buffer, string section, string key)
        {
            db_FileIO db = new db_FileIO();
            string value = db.IniRead("Settings", section, key);
            if (value != "")
            {
                buffer.Write(value);
            }
            else
            {
                WriteOutput("Error in getting setting, failure in values with Section: " + section + " Key: " + key);
            }
        }

        public void SendSettings(Client client)
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMTServer.SendSettings);
            CheckSettingsCorrect(send, "Powerup", "Speed");
            CheckSettingsCorrect(send, "Powerup", "Tickrate");
            CheckSettingsCorrect(send, "Powerup", "Life");
            CheckSettingsCorrect(send, "Powerup", "Range");
            CheckSettingsCorrect(send, "Powerup", "Bomb");
            //Droprate är baserad på "av hundra" = droprate 33 är 33%
            CheckSettingsCorrect(send, "DropRate", "Powerup");
            //Dessa är baserade på skillnaden mellan varandra, kolla i HUDDS om du är osäker
            CheckSettingsCorrect(send, "DropRate", "Speed");
            CheckSettingsCorrect(send, "DropRate", "Tickrate");
            CheckSettingsCorrect(send, "DropRate", "Life");
            CheckSettingsCorrect(send, "DropRate", "Range");
            CheckSettingsCorrect(send, "DropRate", "Bomb");
            server.SendMessage(send, client.ClientConnection, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendCurrentVersion(Client client)
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMTClient.CurrentVersion);
            CheckSettingsCorrect(send, "HUD", "Version");
            server.SendMessage(send, client.ClientConnection, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendCheckIfOnline(Client client, string username, bool isOnline)
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMTServer.AnswerCheckIfOnline);
            send.Write(username);
            send.Write(isOnline);
            server.SendMessage(send, client.ClientConnection, NetDeliveryMethod.ReliableOrdered);
            if (isOnline)
            {
                WriteOutput("User " + username + " is online");
            }
            else
            {
                WriteOutput("User " + username + " is not online or wrong password");
            }
        } //Till server

        private int getStats(string username, Database.UserData data)
        {
            string rtn = db.GetData(username, data);
            if (rtn != "")
            {
                return int.Parse(rtn);
            }
            else
            {
                WriteOutput("Couldn't get stat for User: " + username + " and stat type: " + data.ToString());
                return 0;
            }
        }
        public void SendStats(Client client)
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMTClient.SendStats);
            send.Write(getStats(client.UserName, Database.UserData.Burns));
            send.Write(getStats(client.UserName, Database.UserData.Defeats));
            send.Write(getStats(client.UserName, Database.UserData.ExplodeHits));
            send.Write(getStats(client.UserName, Database.UserData.Kills));
            send.Write(getStats(client.UserName, Database.UserData.PowerupsPicked));
            send.Write(getStats(client.UserName, Database.UserData.SelfExplodeHits));
            send.Write(getStats(client.UserName, Database.UserData.SelfKills));
            send.Write(getStats(client.UserName, Database.UserData.TilesBlowned));
            send.Write(getStats(client.UserName, Database.UserData.TileWalkDistance));
            send.Write(getStats(client.UserName, Database.UserData.Wins));
            server.SendMessage(send, client.ClientConnection, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendRanking(Client client)
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMTClient.SendRanking);
            send.Write(ranker.Usernames.Count);
            foreach (string user in ranker.Usernames)
            {
                send.Write(user);
                send.Write(int.Parse(db.GetData(user, Database.UserData.Elo)));
            }
            server.SendMessage(send, client.ClientConnection, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
