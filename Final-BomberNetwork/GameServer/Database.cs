using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Final_BomberNetwork.GameServer
{
    public class Database
    {
        db_FileIO db = new db_FileIO();

        public Database()
        {
            if (!db.DirectoryExist("Database"))
                db.MakeDirectory("Database");
            db.dir += "\\Database";
        }

        public byte Login(string user, string pass)
        {
            if (UserExist(user))
            {
                if (GetData(user, UserData.Password) == pass)
                {
                    if (GetData(user, UserData.LoggedIn) != "1")
                    {
                        SetData(user, UserData.LoggedIn, "1");
                        return 0; // Can login
                    }
                    else
                    {
                        return 3; // Is already connected
                    }
                }
                else
                {
                    return 2; // Wrong password
                }
            }
            else
            {
                return 1; // This username already exists
            }
        }

        public bool UserExist(string user)
        {
            return db.FileExist(user + ".ini");
        }

        public void CreateUser(string user, string pass)
        {
            if (!UserExist(user))
            {
                db.MakeFile(user + ".ini", false);
                SetData(user, UserData.LoggedIn, "0");
                SetData(user, UserData.Password, pass);
                SetData(user, UserData.Elo, "1000");
                SetData(user, UserData.Wins, "0");
                SetData(user, UserData.Defeats, "0");
                SetData(user, UserData.Burns, "0");
                SetData(user, UserData.ExplodeHits, "0");
                SetData(user, UserData.Kills, "0");
                SetData(user, UserData.PowerupsPicked, "0");
                SetData(user, UserData.SelfExplodeHits, "0");
                SetData(user, UserData.SelfKills, "0");
                SetData(user, UserData.TilesBlowned, "0");
                SetData(user, UserData.TileWalkDistance, "0");
            }
        }

        public List<string> GetUsers()
        {
            List<string> users = new List<string>();
            foreach (string user in db.GetFiles("", ".ini"))
            {
                for (int i = user.Length - 1; i >= 0; i--)
                {
                    if (user[i] == '\\')
                    {
                        users.Add(user.Substring(i + 1, user.Length - i - 5));
                        i = -1;
                    }
                }
            }
            return users;
        }

        public enum UserData
        {
            LoggedIn,
            Password,
            Kills,
            ExplodeHits,
            Burns,
            SelfExplodeHits,
            SelfKills,
            TilesBlowned,
            PowerupsPicked,
            TileWalkDistance,
            Wins,
            Defeats,
            Elo,
        }

        public void SetData(string user, UserData type, string data)
        {
            if (UserExist(user))
            {
                switch (type)
                {
                    case UserData.LoggedIn:
                        db.IniWrite(user, "Info", "LoggedIn", data);
                        break;
                    case UserData.Password:
                        db.IniWrite(user, "Info", "Password", data);
                        break;
                    case UserData.Elo:
                        db.IniWrite(user, "Rank", "Elo", data);
                        break;
                    case UserData.Wins:
                        db.IniWrite(user, "Stats", "Wins", data);
                        break;
                    case UserData.Defeats:
                        db.IniWrite(user, "Stats", "Defeats", data);
                        break;
                    case UserData.Burns:
                        db.IniWrite(user, "Stats", "Burns", data);
                        break;
                    case UserData.ExplodeHits:
                        db.IniWrite(user, "Stats", "ExplodeHits", data);
                        break;
                    case UserData.Kills:
                        db.IniWrite(user, "Stats", "Kills", data);
                        break;
                    case UserData.PowerupsPicked:
                        db.IniWrite(user, "Stats", "PowerupsPicked", data);
                        break;
                    case UserData.SelfExplodeHits:
                        db.IniWrite(user, "Stats", "SelfExplodeHits", data);
                        break;
                    case UserData.SelfKills:
                        db.IniWrite(user, "Stats", "SelfKills", data);
                        break;
                    case UserData.TilesBlowned:
                        db.IniWrite(user, "Stats", "TilesBlowned", data);
                        break;
                    case UserData.TileWalkDistance:
                        db.IniWrite(user, "Stats", "TileWalkDistance", data);
                        break;
                }
            }
        }


        public string GetData(string user, UserData type)
        {
            switch (type)
            {
                case UserData.LoggedIn:
                    return db.IniRead(user, "Info", "LoggedIn");
                case UserData.Password:
                    return db.IniRead(user, "Info", "Password");
                case UserData.Elo:
                    return db.IniRead(user, "Rank", "Elo");
                case UserData.Wins:
                    return db.IniRead(user, "Stats", "Wins");
                case UserData.Defeats:
                    return db.IniRead(user, "Stats", "Defeats");
                case UserData.Burns:
                    return db.IniRead(user, "Stats", "Burns");
                case UserData.ExplodeHits:
                    return db.IniRead(user, "Stats", "ExplodeHits");
                case UserData.Kills:
                    return db.IniRead(user, "Stats", "Kills");
                case UserData.PowerupsPicked:
                    return db.IniRead(user, "Stats", "PowerupsPicked");
                case UserData.SelfExplodeHits:
                    return db.IniRead(user, "Stats", "SelfExplodeHits");
                case UserData.SelfKills:
                    return db.IniRead(user, "Stats", "SelfKills");
                case UserData.TilesBlowned:
                    return db.IniRead(user, "Stats", "TilesBlowned");
                case UserData.TileWalkDistance:
                    return db.IniRead(user, "Stats", "TileWalkDistance");
            }
            return "";
        }
    }
}
