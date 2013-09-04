using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace Final_BomberNetwork.DataBase
{
    class PostHTTPRequest
    {
        
        public static string dictionaryToPostString(Dictionary<string, string> postVariables)
        {
            string postString = "";
            int counter = 0;

            foreach (KeyValuePair<string, string> pair in postVariables)
            {
                if (counter > 0)
                    postString += "&";
                postString += /*HttpUtility.UrlEncode(*/pair.Key/*)*/ + "=" +
                    /*HttpUtility.UrlEncode(*/pair.Value/*)*/;
                counter++;
            }

            return postString;
        }

        public static Dictionary<string, string> postStringToDictionary(string postString)
        {
            char[] delimiters = { '&' };
            string[] postPairs = postString.Split(delimiters);

            var postVariables = new Dictionary<string, string>();
            foreach (string pair in postPairs)
            {
                char[] keyDelimiters = { '=' };
                string[] keyAndValue = pair.Split(keyDelimiters);
                if (keyAndValue.Length > 1)
                {
                    postVariables.Add(/*HttpUtility.UrlDecode(*/keyAndValue[0]/*)*/,
                        /*HttpUtility.UrlDecode(*/keyAndValue[1]/*)*/);
                }
            }

            return postVariables;
        }
        
        public static string postSynchronous(string url, Dictionary<string, string> postVariables)
        {
            string result = null;
            try
            {
                string postString = dictionaryToPostString(postVariables);
                byte[] postBytes = Encoding.ASCII.GetBytes(postString);

                var webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.ContentLength = postBytes.Length;

                Stream postStream = webRequest.GetRequestStream();
                postStream.Write(postBytes, 0, postBytes.Length);
                postStream.Close();

                var webResponse = (HttpWebResponse)webRequest.GetResponse();

                Console.WriteLine(webResponse.StatusCode);
                Console.WriteLine(webResponse.Server);

                Stream responseStream = webResponse.GetResponseStream();
                var responseStreamReader = new StreamReader(responseStream);
                result = responseStreamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de l'envoie de la requête HTTP !\n{0}", ex);
            }
            return result;
        }

    }
}