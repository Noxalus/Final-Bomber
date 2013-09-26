
using System;
using System.Reflection;
using System.Drawing;
using System.Windows;


namespace UpdateLibrary
{
    public interface IUpdatable
    {
        string ApplicationName { get; }
        string ApplicationID { get; }
        Assembly ApplicationAssembly { get; }
        Icon ApplicationIcon { get; }
        Uri UpdateXmlLocation { get; }
        Window Context { get; }
    }
}
