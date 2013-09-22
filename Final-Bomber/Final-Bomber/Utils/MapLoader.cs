using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Final_Bomber.WorldEngine;

namespace Final_Bomber.Utils
{
    class MapLoader
    {
        public static string NewMap(string mapName, IEnumerable<byte> data)
        {
            mapName = Path.GetFileNameWithoutExtension(mapName);
            string mapNr = "";
            int counter = 1;
            while (File.Exists("Content\\Maps\\" + mapName + mapNr + ".map"))
            {
                mapNr = " ~ " + counter;
                counter++;
            }
            
            /*
            var utf8WithoutBom = new UTF8Encoding(false);
            var sw = new StreamWriter("Content\\Maps\\" + mapName + mapNr + ".map", false, utf8WithoutBom);
            */

            var enumerable = data as byte[] ?? data.ToArray();
            File.WriteAllBytes("Content\\Maps\\" + mapName + mapNr + ".map", enumerable.ToArray());

            return mapName + mapNr + ".map";
        }
    }
}
