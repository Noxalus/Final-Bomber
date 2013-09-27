using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;


namespace Final_Bomber.Utils
{
    public static class StaticClassSerializer
    {
        public static bool Save(Type staticClass, string filename, bool binaryFormatter = true)
        {
            try
            {
                FieldInfo[] fields = staticClass.GetFields(BindingFlags.Static | BindingFlags.Public);
                var a = new object[fields.Length, 2];

                int i = 0;
                foreach (FieldInfo field in fields)
                {
                    a[i, 0] = field.Name;
                    a[i, 1] = field.GetValue(null);
                    i++;
                }

                Stream f = File.Open(filename, FileMode.Create);

                IFormatter formatter = (binaryFormatter ? (IFormatter)new BinaryFormatter() : new SoapFormatter());

                formatter.Serialize(f, a);
                f.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Load(Type staticClass, string filename, bool binaryFormatter = true)
        {
            try
            {
                FieldInfo[] fields = staticClass.GetFields(BindingFlags.Static | BindingFlags.Public);
                object[,] a;
                Stream f = File.Open(filename, FileMode.Open);

                IFormatter formatter;

                if (binaryFormatter)
                    formatter = new BinaryFormatter();
                else
                    formatter = new SoapFormatter();

                a = formatter.Deserialize(f) as object[,];

                f.Close();

                if (a != null && a.GetLength(0) != fields.Length) return false;

                int i = 0;
                foreach (FieldInfo field in fields)
                {
                    if (a != null && field.Name == (a[i, 0] as string))
                    {
                        field.SetValue(null, a[i, 1]);
                    }

                    i++;
                }

                return true;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                throw new Exception(ex.Message);
                return false;
            }
        }
    }
}
