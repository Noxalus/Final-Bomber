using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Final_Bomber.WorldEngine
{
    public static class LevelParser
    {
        private static string MapPath = "Content/Maps/";

        public static Level Load(string mapName)
        {
            if (mapName == null) throw new ArgumentNullException("mapName");

            var xmlMap = XDocument.Load(MapPath + mapName + ".xml");

            /*
            var nodes = (from n in xmlMap.Descendants()
                         where n.Name == "middle"
                         select n).First();
            */

            return null;
        }
    }
}
