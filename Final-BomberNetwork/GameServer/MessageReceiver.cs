using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Final_BomberNetwork.GameServer
{
    partial class Server
    {
        public void RecieveCreateAccount(Client client, string user, string pass)
        {
            if (!db.UserExist(user))
            {
                if (client.UserName == "")
                {
                    if (true/*keyHandler.CheckKey(key)*/)
                    {
                        db.CreateUser(user, pass);
                        client.UserName = user;
                        SendCreatedAccount(client.ClientConnection, 0);
                        WriteOutput("Client " + client.ClientId.ToString() + " has created an account with Username " + client.UserName);
                    }
                    else
                    {
                        SendCreatedAccount(client.ClientConnection, 2);
                    }
                }
                else
                {
                    WriteOutput("Error: " + user + " has already created an account");
                }
            }
            else
            {
                SendCreatedAccount(client.ClientConnection, 1);
            }
        }

        public void RecieveLogin(Client client, string user, string pass)
        {
            byte status = db.Login(user, pass);
            SendLoggedIn(client.ClientConnection, status);
            if (status == 0)
            {
                client.LoggedIn = true;
                client.UserName = user;
            }
        }

        public void RecieveHosting(ref Client client, string name, string map, int maxPlayers)
        {
            if (!client.Hosting)
            {
                client.Hosting = true;
                client.GameName = name;
                client.CurrentMap = map;
                client.MaxPlayers = maxPlayers;
                WriteOutput("New game hosted by client " + client.ClientId);
                SendSettings(client);
            }
            else
            {
                WriteOutput("Client " + client.ClientId + " is disconnected because of Error:0002");
                client.ClientConnection.Disconnect("Error:0002");
            }
        }

        public void RecieveGetHostedGames(Client client)
        {
            SendHostedGames(client.ClientConnection);
        }

        public void RecieveGetCurrentPlayerAmount(Client client, int players)
        {
            client.CurrentPlayers = players;
        }

        public void RecieveGetServerPort(Client client, int port)
        {
            client.Port = port;
        }

        public void RecieveCheckIfOnline(Client client, string username, string password)
        {
            if (db.GetData(username, Database.UserData.Password) == password && db.GetData(username, Database.UserData.LoggedIn) == "1")
            {
                SendCheckIfOnline(client, username, true);
            }
            else
            {
                SendCheckIfOnline(client, username, false);
            }
        }

        public void RecieveNextMap(Client client, string mapname)
        {
            client.CurrentMap = mapname;
        }

        private void addStat(string username, Database.UserData data, int amount)
        {
            string dt = db.GetData(username, data);
            if (dt != "")
            {
                db.SetData(username, data, (int.Parse(dt) + amount).ToString());
            }
            else
            {
                WriteOutput("Couldn't add stat for User: " + username + " and stat type: " + data.ToString());
            }
        }
        public void RecievePlayerStats()
        {
            EloCollection players = new EloCollection();
            int count = msg.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string username = msg.ReadString();
                bool lost = msg.ReadBoolean();
                if (!lost)
                {
                    addStat(username, Database.UserData.Wins, 1);
                }
                else
                {
                    addStat(username, Database.UserData.Defeats, 1);
                }
                addStat(username, Database.UserData.Burns, msg.ReadInt32());
                addStat(username, Database.UserData.ExplodeHits, msg.ReadInt32());
                addStat(username, Database.UserData.Kills, msg.ReadInt32());
                addStat(username, Database.UserData.PowerupsPicked, msg.ReadInt32());
                addStat(username, Database.UserData.SelfExplodeHits, msg.ReadInt32());
                addStat(username, Database.UserData.SelfKills, msg.ReadInt32());
                addStat(username, Database.UserData.TilesBlowned, msg.ReadInt32());
                addStat(username, Database.UserData.TileWalkDistance, msg.ReadInt32());
                players.Add(new Elo(username, !lost));
                Client client = clients.GetClientFromUsername(username);
            }
            players.CalculateElo(db);
        }
    }
}
