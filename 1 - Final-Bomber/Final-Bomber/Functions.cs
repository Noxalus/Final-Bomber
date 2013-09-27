using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Final_Bomber
{
    public class Functions
    {
        const string USABLECHARS = "abcdefghijklmnopqrstuvwxyz_-1234567890";

        public static string HidePassword(string password)
        {
            string hiddenPass = "";
            for (int i = 0; i < password.Length; i++)
                hiddenPass += "*";
            return hiddenPass;
        }

        public static bool CheckChars(string str)
        {
            bool check = false;
            foreach (char chr in str)
            {
                foreach (char usable in USABLECHARS)
                {
                    if (chr.ToString().ToUpper() == usable.ToString().ToUpper())
                        check = true;
                }
                if (!check)
                {
                    return false;
                }
                else
                {
                    check = false;
                }
            }
            return true;
        }

        // To shake the screen
        public static Vector3 Shake(Random random)
        {
            return new Vector3(
                (float)((random.NextDouble() * 25) - 1),
                (float)((random.NextDouble() * 25) - 1), 0);
        } 
    }
}
