using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Final_BomberNetwork.GameServer
{
    class ClientCollection : List<Client>
    {
        string mapName;
        public string MapName
        {
            get { return mapName; }
            set { mapName = value; }
        }

        public void AddClient(Client client)
        {
            foreach (Client c in this)
            {
                if (client.ClientId == c.ClientId)
                    return;
            }
            this.Add(client);
            return;
        }

        public bool RemoveClient(Client client)
        {
            foreach (Client c in this)
            {
                if (client.ClientId == c.ClientId)
                {
                    this.Remove(client);
                    return true;
                }
            }
            return false;
        }

        public Client GetClientFromConnection(NetConnection connection)
        {
            foreach (Client c in this)
            {
                if (c.ClientConnection == connection)
                    return c;
            }
            return null;
        }

        public Client GetClientFromUsername(string username)
        {
            foreach (Client c in this)
            {
                if (c.UserName == username)
                    return c;
            }
            return null;
        }

    }

    class EloCollection : List<Elo>
    {
        const int K = 16;

        public void CalculateElo(Database db)
        {
            foreach (Elo player in this)
            {
                #region CalculateEnemyAverage
                int eloavrg = 0;
                foreach (Elo pl in this)
                {
                    if (pl != player)
                    {
                        string elo = db.GetData(pl.Username, Database.UserData.Elo);
                        eloavrg += int.Parse(elo);
                    }
                }
                eloavrg /= (this.Count - 1);
                #endregion

                int myElo = int.Parse(db.GetData(player.Username, Database.UserData.Elo));
                double expectedScore = 1 / (1 + Math.Pow(10, (eloavrg - myElo) / 400));
                double newElo = 0;
                if (player.Won)
                {
                    newElo = myElo + K * (GetWinningScore(player) - expectedScore);
                }
                else
                {
                    newElo = myElo + (K * (GetWinningScore(player) - expectedScore) / (this.Count - 1));
                }
                newElo = Math.Round(newElo, 0);
                db.SetData(player.Username, Database.UserData.Elo, newElo.ToString());
            }
        }

        public double GetWinningScore(Elo player)
        {
            foreach (Elo pl in this)
            {
                if (pl.Won)
                {
                    if (player.Won)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            return 0.5;
        }
    }
}
