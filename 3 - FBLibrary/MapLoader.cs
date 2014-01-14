using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace FBLibrary
{
    public static class MapLoader
    {
        public static readonly Dictionary<string, string> MapFileDictionary = new Dictionary<string, string>();

        public static void LoadMapFiles()
        {
            MapFileDictionary.Clear();

            using (var md5 = MD5.Create())
            {
                try
                {
                    //Get all the map files
                    foreach (var source in Directory.GetFiles(@"Content\Maps\", "*.map", SearchOption.AllDirectories))
                    {
                        using (var stream = File.OpenRead(source))
                        {
                            // Compute the MD5 hash of the file
                            string md5Hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                            string fileName = Path.GetFileName(source);

                            if (fileName != null)
                                MapFileDictionary.Add(fileName, md5Hash);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
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

            var enumerable = data as byte[] ?? data.ToArray();

            if (!Directory.Exists("Content/Maps"))
                Directory.CreateDirectory("Content/Maps");

            File.WriteAllBytes("Content/Maps/" + mapName + mapNr + ".map", enumerable.ToArray());

            return mapName + mapNr + ".map";
        }

        public static string GetMapNameFromMd5(string md5)
        {
            return MapFileDictionary.FirstOrDefault(map => map.Value == md5).Key;
        }
    }
}
