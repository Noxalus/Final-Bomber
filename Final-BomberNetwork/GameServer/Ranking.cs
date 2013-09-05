using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Final_BomberNetwork.GameServer
{
    class Ranking
    {
        public List<string> Usernames = new List<string>();

        public string GetUserFromRank(int rank)
        {
            rank -= 1;
            return Usernames[rank];
        }

        public void Sort(Database db)
        {
            Usernames.Sort(new RankSorter(db));
        }
    }

    class RankSorter : Comparer<string>
    {
        Database db;
        public RankSorter(Database db)
        {
            this.db = db;
        }

        public override int Compare(string x, string y)
        {
            if (x == y)
            {
                return 0;
            }
            else
            {
                int elox = int.Parse(db.GetData(x, Database.UserData.Elo));
                int eloy = int.Parse(db.GetData(y, Database.UserData.Elo));
                if (elox > eloy)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
        }
    }
}
