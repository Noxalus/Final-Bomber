namespace Final_Bomber.Controls
{
    public static class Keyboarding
    {
        public static string SpecialChar(string s, bool shift, bool altGr)
        {
            string st = null;
            if (altGr)
            {
                switch (s)
                {
                    case "D0":
                        st = "@";
                        break;
                }
            }

            else if (!shift)
            {
                switch (s)
                {
                    case "D0":
                        st = "à";
                        break;
                    case "D1":
                        st = "&";
                        break;
                    case "D2":
                        st = "é";
                        break;
                    case "D3":
                        st = "\"";
                        break;
                    case "D4":
                        st = "'";
                        break;
                    case "D5":
                        st = "(";
                        break;
                    case "D6":
                        st = "-";
                        break;
                    case "D7":
                        st = "è";
                        break;
                    case "D8":
                        st = "_";
                        break;
                    case "D9":
                        st = "ç";
                        break;
                    case "Oem8":
                        st = "!";
                        break;
                    case "OemQuestion":
                        st = ":";
                        break;
                    case "OemPeriod":
                        st = ";";
                        break;
                    case "OemComma":
                        st = ",";
                        break;
                    case "OemOpenBrackets":
                        st = ")";
                        break;
                    case "OemPlus":
                        st = "=";
                        break;
                    case "OemCloseBrackets":
                        st = "^";
                        break;
                    case "OemSemicolon":
                        st = "$";
                        break;
                    case "OemTilde":
                        st = "ù";
                        break;
                    case "OemPipe":
                        st = "*";
                        break;
                }
            }
            else
            {
                switch (s)
                {
                    case "Oem8":
                        st = "§";
                        break;
                    case "OemQuestion":
                        st = "/";
                        break;
                    case "OemPeriod":
                        st = ".";
                        break;
                    case "OemComma":
                        st = "?";
                        break;
                    case "OemOpenBrackets":
                        st = "°";
                        break;
                    case "OemPlus":
                        st = "+";
                        break;
                    case "OemCloseBrackets":
                        st = "¨";
                        break;
                    case "OemSemicolon":
                        st = "£";
                        break;
                    case "OemTilde":
                        st = "%";
                        break;
                    case "OemPipe":
                        st = "µ";
                        break;
                }
            }
            return st;
        }
    }
}
