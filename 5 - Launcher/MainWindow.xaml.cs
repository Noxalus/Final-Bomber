using System;
using System.Drawing;
using System.Reflection;
using System.Windows;
using UpdateLibrary;

namespace FBLauncher
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IUpdatable
    {
        public MainWindow()
        {
            InitializeComponent();

            this.label.Content = this.ApplicationAssembly.GetName().Version.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        public string ApplicationName
        {
            get { return "FBUpdater"; }
        }

        public string ApplicationID
        {
            get { return "FBUpdater"; }
        }

        public Assembly ApplicationAssembly
        {
            get { return Assembly.GetExecutingAssembly(); }
        }

        public Icon ApplicationIcon
        {
            get { return new Icon(""); }
        }

        public Uri UpdateXmlLocation
        {
            get { return new Uri(""); }
        }

        public Window Context
        {
            get { return this; }
        }
    }
}
